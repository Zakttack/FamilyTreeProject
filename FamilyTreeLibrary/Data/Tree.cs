using FamilyTreeLibrary.Data.Enumerators;
using FamilyTreeLibrary.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections;


namespace FamilyTreeLibrary.Data
{
    public class Tree : ITree
    {
        private readonly IMongoCollection<BsonDocument> mongoCollection;

        public Tree(string name)
        {
            Name = name;
            mongoCollection = DataUtils.GetCollection(Name);
            BsonDocument rootDocument = mongoCollection.Find(FilterDefinition<BsonDocument>.Empty).FirstOrDefault();
            Root = new(rootDocument);
        }

        private Tree(string name, FamilyNode initialRoot)
        {
            Name = name;
            mongoCollection = DataUtils.GetCollection(Name);
            Root = initialRoot;
        }

        public long Count
        {
            get
            {
                IEnumerable<Family> families = this;
                return families.LongCount();
            }
        }

        public string Name
        {
            get;
        }

        public int Height
        {
            get
            {
                return Root is null ? 0 : GetHeight(Root);
            }
        }

        private FamilyNode Root
        {
            get;
        }

        public void Add(Family parent, Family child)
        {
            if (Root is not null)
            {
                FamilyNode parentNode = DataUtils.GetNodeOf(parent, mongoCollection);
                FamilyNode childNode = DataUtils.GetNodeOf(child, mongoCollection);
                if (parentNode is null && childNode is not null)
                {
                    FilterDefinition<BsonDocument> initialParentFilter = Builders<BsonDocument>.Filter.Eq("Element", childNode.Parent.Document);
                    UpdateDefinition<BsonDocument> initialParentUpdate = Builders<BsonDocument>.Update.Pull("Children", child.Document);
                    mongoCollection.UpdateOne(initialParentFilter, initialParentUpdate);
                    FilterDefinition<BsonDocument> childFilter = Builders<BsonDocument>.Filter.Eq("Element", child.Document);
                    FieldDefinition<BsonDocument,BsonValue> parentField = new StringFieldDefinition<BsonDocument,BsonValue>("Parent");
                    UpdateDefinition<BsonDocument> childParentUpdate = Builders<BsonDocument>.Update.Set(parentField, parent is null ? BsonNull.Value : parent.Document);
                    mongoCollection.UpdateOne(childFilter, childParentUpdate);
                }
                else if (parentNode is not null && childNode is not null)
                {
                    FilterDefinition<BsonDocument> initialParentFilter = Builders<BsonDocument>.Filter.Eq("Element", childNode.Parent.Document);
                    UpdateDefinition<BsonDocument> initialParentUpdate = Builders<BsonDocument>.Update.Pull("Children", child.Document);
                    mongoCollection.UpdateOne(initialParentFilter, initialParentUpdate);
                    FilterDefinition<BsonDocument> finalParentFilter = Builders<BsonDocument>.Filter.Eq("Element", parent.Document);
                    UpdateDefinition<BsonDocument> finalParentUpdate = Builders<BsonDocument>.Update.Push("Children", child.Document);
                    mongoCollection.UpdateOne(finalParentFilter, finalParentUpdate);
                    FilterDefinition<BsonDocument> childFilter = Builders<BsonDocument>.Filter.Eq("Element", child.Document);
                    FieldDefinition<BsonDocument,BsonDocument> parentField = new StringFieldDefinition<BsonDocument,BsonDocument>("Parent");
                    UpdateDefinition<BsonDocument> childParentUpdate = Builders<BsonDocument>.Update.Set(parentField, parent.Document);
                    mongoCollection.UpdateOne(childFilter, childParentUpdate);
                }
                else if (parentNode is not null && childNode is null)
                {
                    FilterDefinition<BsonDocument> parentFilter = Builders<BsonDocument>.Filter.Eq("Element", parent.Document);
                    UpdateDefinition<BsonDocument> parentUpdate = Builders<BsonDocument>.Update.Push("Children", child.Document);
                    mongoCollection.UpdateOne(parentFilter, parentUpdate);
                }
            }
            FamilyNode newNode = new(parent, child);
            mongoCollection.InsertOne(newNode.Document);
        }

        public bool Contains(Family parent, Family child)
        {
            if (Root is null)
            {
                return false;
            }
            else if (parent is null && child == Root.Element)
            {
                return true;
            }
            else if (parent == Root.Element && DataUtils.GetNodeOf(child, mongoCollection) != null)
            {
                return true;
            }
            foreach (Family fam in this)
            {
                FamilyNode node = DataUtils.GetNodeOf(fam, mongoCollection);
                if (node.Parent == parent && node.Element == child || DataUtils.GetParentOf(node,mongoCollection).Children.Contains(child))
                {
                    return true;
                }
            }
            return false;
        }

        public int Depth(Family element)
        {
            FamilyNode current = DataUtils.GetNodeOf(element, mongoCollection);
            int depth = 0;
            while (current != Root)
            {
                depth++;
                FamilyNode temp = current;
                current = DataUtils.GetParentOf(temp, mongoCollection);
            }
            return depth;
        }

        public IEnumerable<Family> GetChildren(Family element)
        {
            FamilyNode node = DataUtils.GetNodeOf(element,mongoCollection);
            return node.Children;
        }

        public IEnumerator<Family> GetEnumerator()
        {
            ICollection<ICollection<ICollection<FamilyNode>>> families = new HashSet<ICollection<ICollection<FamilyNode>>>();
            Traverse(families, Root);
            return new FamilyEnumerator(families);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Family GetParent(Family element)
        {
            FamilyNode node = DataUtils.GetNodeOf(element, mongoCollection);
            return node.Parent;
        }

        public ITree Subtree(Family element)
        {
            FamilyNode node = DataUtils.GetNodeOf(element, mongoCollection);
            return new Tree(Name, node);
        }

        public void Update(Family initial, Family final)
        {
            FilterDefinition<BsonDocument> parentFilter = Builders<BsonDocument>.Filter.ElemMatch("Children",
                Builders<BsonDocument>.Filter.Eq("Element", initial.Document));
            UpdateDefinition<BsonDocument> parentUpdate = Builders<BsonDocument>.Update.Set("Children.Element", final.Document);
            mongoCollection.UpdateOne(parentFilter, parentUpdate);
            FilterDefinition<BsonDocument> elementFilter = Builders<BsonDocument>.Filter.Eq("Element", initial.Document);
            UpdateDefinition<BsonDocument> elementUpdate = Builders<BsonDocument>.Update.Set("Element", final.Document);
            mongoCollection.UpdateOne(elementFilter, elementUpdate);
            FilterDefinition<BsonDocument> childrenFilter = Builders<BsonDocument>.Filter.Eq("Parent", initial.Document);
            UpdateDefinition<BsonDocument> childUpdate = Builders<BsonDocument>.Update.Set("Parent", final.Document);
            mongoCollection.UpdateMany(childrenFilter, childUpdate);
        }

        private int GetHeight(FamilyNode start)
        {
            IEnumerable<FamilyNode> children = DataUtils.GetChildrenOf(start, mongoCollection);
            int height = 0;
            foreach (FamilyNode child in children)
            {
                height = Math.Max(height, GetHeight(child));
            }
            return height + 1;
        }

        private void Traverse(ICollection<ICollection<ICollection<FamilyNode>>> nodes, FamilyNode parent)
        {
            if (parent is not null)
            {
                if (!nodes.Any() || nodes.Last().Count == int.MaxValue)
                {
                    ICollection<ICollection<FamilyNode>> subCollection = new List<ICollection<FamilyNode>>()
                    {
                        new List<FamilyNode>()
                        {
                            parent
                        }
                    };
                    nodes.Add(subCollection);
                }
                else if (!nodes.Last().Any() || nodes.Last().Last().Count == int.MaxValue)
                {
                    ICollection<FamilyNode> subCollection = new List<FamilyNode>()
                    {
                        parent
                    };
                    nodes.Last().Add(subCollection);
                }
                else
                {
                    nodes.Last().Last().Add(parent);
                }
                foreach (Family child in parent.Children)
                {
                    Traverse(nodes, DataUtils.GetNodeOf(child, mongoCollection));
                }
            }
        }
    }
}