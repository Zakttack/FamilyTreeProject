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
            Root = this.FirstOrDefault();
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
                IEnumerable<FamilyNode> families = this;
                return families.LongCount();
            }
        }

        public IEnumerable<Family> this[Person member]
        {
            get
            {
                ICollection<Family> families = new SortedSet<Family>();
                foreach (FamilyNode node in this)
                {
                    if (node.Element.Member == member)
                    {
                        families.Add(node.Element);
                    }
                }
                return families;
            }
        }

        public int Height
        {
            get
            {
                return this.Any() ? GetHeight(this.First()) : 0;
            }
        }

        public string Name
        {
            get;
        }

        private FamilyNode Root
        {
            get;
        }

        public void Add(FamilyNode node)
        {
            BsonDocument record = node.Document;
            if (Root is null)
            {
                mongoCollection.InsertOne(record);
            }
            else
            {
                FamilyNode initialParent = null;
                FamilyNode initialChild = null;
                BsonValue parentValue = record[1];
                if (parentValue is not null && parentValue.IsBsonDocument)
                {
                    FamilyNode tempInitialParent = Find(record[1].AsBsonDocument);
                    if (tempInitialParent is not null && !tempInitialParent.Children.Contains(node.Element))
                    {
                        FamilyNode finalParent = tempInitialParent;
                        finalParent.Children.Add(node.Element);
                        Update(tempInitialParent, finalParent, false, false);
                    }
                    initialParent = tempInitialParent;
                }
                BsonArray children = record[nameof(node.Children)].AsBsonArray;
                foreach (BsonValue child in children)
                {
                    initialChild = Find(child.AsBsonDocument);
                    initialParent = DataUtils.GetParentOf(initialChild, mongoCollection);
                    if (initialParent is not null)
                    {
                        FamilyNode finalParent = initialParent;
                        finalParent.Children.Remove(initialChild.Element);
                        FamilyNode finalChild = initialChild;
                        Update(initialParent, finalParent, false, false);
                        finalChild.Parent = node.Element;
                        Update(initialChild, finalChild, false, false);
                    }
                }
                if (initialChild is not null || initialParent is not null)
                {
                    mongoCollection.InsertOne(record);
                }
            }
        }

        public bool Contains(FamilyNode node)
        {
            return this.Any(n => n is not null && n.Element == node.Element);
        }

        public int Depth(FamilyNode node)
        {
            if (node is null)
            {
                return 0;
            }
            int depth = 0;
            FamilyNode currentNode = node;
            while (currentNode.Parent is not null)
            {
                depth++;
                FamilyNode parentNode = currentNode;
                currentNode = DataUtils.GetParentOf(parentNode, mongoCollection);
            }
            return depth;
        }

        public IEnumerable<Family> GetChildren(FamilyNode node)
        {
            return Contains(node) ? node.Children : null;
        }

        public Family GetParent(FamilyNode node)
        {
            return Contains(node) ? node.Parent : null;
        }

        public IEnumerator<FamilyNode> GetEnumerator()
        {
            return new FamilyEnumerator(mongoCollection, Root);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new FamilyEnumerator(mongoCollection, Root);
        }

        public ITree Subtree(FamilyNode root)
        {
            return new Tree(Name, root);
        }

        public void Update(FamilyNode initialNode, FamilyNode finalNode)
        {
            Update(initialNode, finalNode, true, true);
        }

        private FamilyNode Find(BsonDocument element)
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("Element", element);
            IFindFluent<BsonDocument,BsonDocument> results = mongoCollection.Find(filter);
            return results.Any() ? new(results.First()) : null;
        }

        private int GetHeight(FamilyNode current)
        {
            if (current.Children.Count == 0)
            {
                return 0;
            }
            int maxChildHeight = 0;
            IEnumerable<FamilyNode> children = DataUtils.GetChildrenOf(current, mongoCollection);
            foreach (FamilyNode child in children)
            {
                maxChildHeight = Math.Max(maxChildHeight, GetHeight(child));
            }
            return maxChildHeight + 1;
        }

        private void Update(FamilyNode initialNode, FamilyNode finalNode, bool updateParent, bool updateChildren)
        {
            ObjectId id = initialNode.Id;
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            mongoCollection.FindOneAndReplace(filter, finalNode.Document);
            FamilyNode initialParentNode = DataUtils.GetParentOf(initialNode, mongoCollection);
            IEnumerable<FamilyNode> children = DataUtils.GetChildrenOf(initialNode, mongoCollection);
            if (updateParent)
            {
                FamilyNode finalParentNode = initialParentNode;
                finalParentNode.Children.Remove(initialNode.Element);
                finalParentNode.Children.Add(finalNode.Element);
                Update(initialParentNode, finalParentNode, false, false);
            }
            if (updateChildren)
            {
                foreach (FamilyNode initialChild in children)
                {
                    FamilyNode finalChild = initialChild;
                    finalChild.Parent = initialNode.Element;
                    FamilyNode tempInitialChild = initialChild;
                    Update(tempInitialChild, finalChild, false, false);
                }
            }
        }

        private class FamilyEnumerator : IEnumerator<FamilyNode>
        {
            private readonly ICollection<IList<FamilyNode>> vistedNodes;
            private readonly FamilyNode initialRoot;

            private FamilyNode currentNode;
            private readonly IMongoCollection<BsonDocument> mongoCollection;

            public FamilyEnumerator(IMongoCollection<BsonDocument> collection, FamilyNode initialRoot)
            {
                mongoCollection = collection;
                vistedNodes = new List<IList<FamilyNode>>();
                this.initialRoot = initialRoot;
                Reset();
            }

            public FamilyNode Current
            {
                get
                {
                    FamilyNode result = currentNode;
                    currentNode = FindNext(result);
                    ApplyNode(currentNode);
                    return result;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    FamilyNode result = currentNode;
                    currentNode = FindNext(result);
                    ApplyNode(currentNode);
                    return result;
                }
            }

            public void Dispose()
            {
                Reset();
            }

            public bool MoveNext()
            {
                return currentNode is not null;
            }

            public void Reset()
            {
                vistedNodes.Clear();
                currentNode = Root;
                ApplyNode(currentNode);
            }

            private FamilyNode Root
            {
                get
                {
                    FilterDefinition<BsonDocument> rootFilter;
                    if (initialRoot is null)
                    {
                        rootFilter = Builders<BsonDocument>.Filter.Eq("Parent", BsonNull.Value);
                    }
                    else
                    {
                        ObjectId id = initialRoot.Id;
                        rootFilter = Builders<BsonDocument>.Filter.Eq("_id", id);
                    }
                    IFindFluent<BsonDocument,BsonDocument> rootResults = mongoCollection.Find(rootFilter);
                    return rootResults.Any() ? new(rootResults.First()) : null;
                }
            }

            private FamilyNode FindNext(FamilyNode tempNext)
            {
                if (tempNext is not null)
                {
                    IEnumerable<FamilyNode> children = DataUtils.GetChildrenOf(tempNext, mongoCollection);
                    FamilyNode firstChild = children.FirstOrDefault();
                    if (firstChild is not null && !vistedNodes.Where((part) => part.Contains(firstChild)).Any())
                    {
                        return firstChild;
                    }
                    else if (tempNext != Root)
                    {
                        FamilyNode parent = DataUtils.GetParentOf(tempNext, mongoCollection);
                        IList<FamilyNode> sibilings = DataUtils.GetChildrenOf(parent, mongoCollection).ToList();
                        int sibilingIndex = sibilings.IndexOf(tempNext);
                        return sibilingIndex > -1 && sibilingIndex < parent.Children.Count - 1 ? sibilings[sibilingIndex + 1] : FindNext(parent);
                    }
                    return null;
                }
                return Root;
            }

            private void ApplyNode(FamilyNode node)
            {
                if (node is not null && (!vistedNodes.Any() || vistedNodes.Last().Count == int.MaxValue))
                {
                    IList<FamilyNode> subCollection = new List<FamilyNode>()
                    {
                        node
                    };
                    vistedNodes.Add(subCollection);
                }
                else if (node is not null)
                {
                    vistedNodes.Last().Add(node);
                }
            }
        }
    }
}