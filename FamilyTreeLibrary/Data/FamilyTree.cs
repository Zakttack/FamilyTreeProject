using FamilyTreeLibrary.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections;


namespace FamilyTreeLibrary.Data
{
    public class FamilyTree : IFamilyTree
    {
        private readonly IMongoCollection<BsonDocument> mongoCollection;
        private readonly Family initialRoot;

        public FamilyTree(string name)
        {
            Name = name;
            mongoCollection = DataUtils.GetCollection(Name);
            initialRoot = null;
        }

        private FamilyTree(string name, Family initialRoot)
        {
            Name = name;
            mongoCollection = DataUtils.GetCollection(Name);
            this.initialRoot = initialRoot;
        }
        public long Count
        {
            get
            {
                IEnumerable<Family> families = this;
                return families.LongCount();
            }
        }

        public Family this[Person member]
        {
            get
            {
                return this.Where((node) => node.Member == member).FirstOrDefault();
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

        public void Add(Family parentNode, Family childNode)
        {
            mongoCollection.InsertOne(childNode.Document);
            if (parentNode is not null)
            {
                IEnumerable<Person> commonChildren = parentNode.Children.Intersect(childNode.Children);
                foreach (Person commonChild in commonChildren)
                {
                    Family finalParent = parentNode;
                    finalParent.Children.Remove(commonChild);
                    Family initialChild = this[commonChild];
                    Family finalChild = initialChild;
                    finalChild.Parent = finalParent.Member;
                    Update(parentNode, finalParent);
                    Update(initialChild, finalChild);
                }
                BsonDocument parentDoc = parentNode.Document;
                BsonArray children = parentDoc["Children"].AsBsonArray;
                children.Add(childNode.Member.Document);
                FilterDefinition<BsonDocument> findParentFilter = Builders<BsonDocument>.Filter.Eq("Id", parentDoc["Id"]);
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("Children", children);
                mongoCollection.UpdateOne(findParentFilter, update);
            }
        }

        public bool Contains(Family node)
        {
            IEnumerable<Family> families = this;
            return families.Contains(node);
        }

        public int Depth(Family node)
        {
            if (node is null)
            {
                return 0;
            }
            int depth = 0;
            Family currentNode = node;
            while (currentNode.Parent is not null)
            {
                depth++;
                Family parentNode = currentNode;
                currentNode = DataUtils.GetParentOf(parentNode, mongoCollection);
            }
            return depth;
        }

        public IEnumerator<Family> GetEnumerator()
        {
            return new FamilyEnumerator(mongoCollection, initialRoot);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new FamilyEnumerator(mongoCollection, initialRoot);
        }

        public IFamilyTree Subtree(Family root)
        {
            return new FamilyTree(Name, root);
        }

        public void Update(Family initialNode, Family finalNode)
        {
            ObjectId id = initialNode.Id;
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("Id", id);
            mongoCollection.FindOneAndReplace(filter, finalNode.Document);
        }

        private int GetHeight(Family current)
        {
            if (current.Children.Count == 0)
            {
                return 0;
            }
            int maxChildHeight = 0;
            IEnumerable<Family> children = DataUtils.GetChildrenOf(current, mongoCollection);
            foreach (Family child in children)
            {
                maxChildHeight = Math.Max(maxChildHeight, GetHeight(child));
            }
            return maxChildHeight + 1;
        }

        private class FamilyEnumerator : IEnumerator<Family>
        {
            private readonly ICollection<IList<Family>> vistedNodes;
            private readonly Family initialRoot;

            private readonly IMongoCollection<BsonDocument> mongoCollection;

            public FamilyEnumerator(IMongoCollection<BsonDocument> collection, Family initialRoot)
            {
                mongoCollection = collection;
                vistedNodes = new List<IList<Family>>();
                this.initialRoot = initialRoot;
                Reset();
            }

            public Family Current
            {
                get
                {
                    Family currentNode = vistedNodes.Last().Last();
                    InsertNode(FindNext(currentNode));
                    return currentNode;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
                Reset();
            }

            public bool MoveNext()
            {
                return Current is not null;
            }

            public void Reset()
            {
                vistedNodes.Clear();
                IList<Family> firstPart = new List<Family>();
                FilterDefinition<BsonDocument> rootFilter;
                if (initialRoot is null)
                {
                    rootFilter = Builders<BsonDocument>.Filter.Eq("Parent", BsonNull.Value);
                }
                else
                {
                    ObjectId id = initialRoot.Id;
                    rootFilter = Builders<BsonDocument>.Filter.Eq("Id", id);
                }
                IFindFluent<BsonDocument,BsonDocument> queryResult = mongoCollection.Find(rootFilter);
                if (queryResult.Any())
                {
                    firstPart.Add(new(queryResult.First()));
                }
                else
                {
                    firstPart.Add(null);
                }
                vistedNodes.Add(firstPart);
            }

            private Family Root
            {
                get
                {
                    return vistedNodes.First().First();
                }
            }

            private Family FindNext(Family tempNext)
            {
                if (tempNext is not null)
                {
                    IEnumerable<Family> children = DataUtils.GetChildrenOf(tempNext, mongoCollection);
                    Family firstChild = children.FirstOrDefault();
                    if (firstChild is not null && !vistedNodes.Where((part) => part.Contains(firstChild)).Any())
                    {
                        return firstChild;
                    }
                    else if (tempNext != Root)
                    {
                        Family parent = DataUtils.GetParentOf(tempNext, mongoCollection);
                        IList<Family> sibilings = DataUtils.GetChildrenOf(parent, mongoCollection).ToList();
                        int sibilingIndex = sibilings.IndexOf(tempNext);
                        return sibilingIndex > -1 && sibilingIndex < parent.Children.Count - 1 ? sibilings[sibilingIndex + 1] : FindNext(parent);
                    }
                }
                return null;
            }

            private void InsertNode(Family node)
            {
                if (vistedNodes.Last().Count == int.MaxValue)
                {
                    vistedNodes.Add(new List<Family>());
                }
                vistedNodes.Last().Add(node);
            }
        }
    }
}