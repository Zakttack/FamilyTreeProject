using System.Collections;
using System.Reflection;
using FamilyTreeLibrary.Models;
using MongoDB.Bson;
using MongoDB.Driver;


namespace FamilyTreeLibrary.Data
{
    public class FamilyTree : IFamilyTree
    {
        private readonly FamilyEnumerator familyTraversal;
        public FamilyTree(string name, Family initialRoot = null)
        {
            Name = name;
            familyTraversal = new FamilyEnumerator(DataUtils.GetCollection(Name), initialRoot);
        }
        public long Count
        {
            get
            {
                long count = 0;
                familyTraversal.Reset();
                while (familyTraversal.MoveNext())
                {
                    count++;
                    familyTraversal.Dispose();
                }
                return count;
            }
        }

        public IEnumerable<Family> this[Person member]
        {
            get
            {
                ICollection<Family> results = new List<Family>();
                familyTraversal.Reset();
                while (familyTraversal.MoveNext())
                {
                    if (familyTraversal.Current.Member == member)
                    {
                        results.Add(familyTraversal.Current);
                    }
                    familyTraversal.Dispose();
                }
                return results;
            }
        }

        public int Height
        {
            get
            {
                familyTraversal.Reset();
                return familyTraversal.MoveNext() ? GetHeight(familyTraversal.Current) : 0;
            }
        }

        public void Add(Family node)
        {
            Person parent = node.Parent.Member;
            familyTraversal.Reset();
            while (familyTraversal.MoveNext() && familyTraversal.Current.Member != parent)
            {
                familyTraversal.Dispose();
            }
            if (familyTraversal.MoveNext())
            {
                Family temp = familyTraversal.Current;
                FilterDefinition<BsonDocument> filter = new BsonDocumentFilterDefinition<BsonDocument>(BsonDocument.Parse(temp.ToString()));
                familyTraversal.Current.Children.Add(node);
                UpdateDefinition<BsonDocument> update = new BsonDocumentUpdateDefinition<BsonDocument>(BsonDocument.Parse(familyTraversal.Current.ToString()));
                familyTraversal.Collection.UpdateOne(filter, update);
            }
            familyTraversal.Collection.InsertOne(BsonDocument.Parse(node.ToString()));
        }

        public bool Contains(Family node)
        {
            familyTraversal.Reset();
            while (familyTraversal.MoveNext())
            {
                if (familyTraversal.Current == node)
                {
                    return true;
                }
                familyTraversal.Dispose();
            }
            return false;
        }

        public int Depth(Family node)
        {
            familyTraversal.Reset();
            if (!familyTraversal.MoveNext())
            {
                return 0;
            }
            int depth = 0;
            Family currentNode = node;
            while (currentNode != familyTraversal.Current)
            {
                depth++;
                currentNode = currentNode.Parent;
            }
            return depth;
        }

        public IEnumerator<Family> GetEnumerator()
        {
            return familyTraversal;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return familyTraversal;
        }

        public IFamilyTree Subtree(Family root)
        {
            return new FamilyTree(Name, root);
        }

        private string Name
        {
            get;
        }

        private int GetHeight(Family current)
        {
            if (current.Children.Count == 0)
            {
                return 0;
            }
            int maxChildHeight = 0;
            foreach (Family child in current.Children)
            {
                maxChildHeight = Math.Max(maxChildHeight, GetHeight(child));
            }
            return maxChildHeight + 1;
        }

        private class FamilyEnumerator : IEnumerator<Family>
        {
            private readonly ICollection<IList<Family>> vistedNodes;
            private readonly Family initialRoot;

            public FamilyEnumerator(IMongoCollection<BsonDocument> collection, Family initialRoot)
            {
                Collection = collection;
                vistedNodes = new List<IList<Family>>();
                this.initialRoot = initialRoot;
                Reset();
            }

            public Family Current
            {
                get
                {
                    return vistedNodes.Last().Last();
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public IMongoCollection<BsonDocument> Collection
            {
                get;
            }

            public void Dispose()
            {
                Family previous = Current;
                InsertNode(FindNext(previous));
            }

            public bool MoveNext()
            {
                return Current is not null;
            }

            public void Reset()
            {
                vistedNodes.Clear();
                IList<Family> firstPart = new List<Family>();
                List<BsonDocument> results;
                if (initialRoot is null)
                {
                    results = Collection.Find((record) => IsFirst(record)).ToList();
                }
                else
                {
                    Func<BsonDocument,bool> rootInitializeQuery = (record) => {
                        Family currentNode = DataUtils.ParseRecord(record);
                        return currentNode == initialRoot;
                    };
                    results = Collection.Find((record) => rootInitializeQuery(record)).ToList();
                }
                if (results.Count > 0)
                {
                    Family rootValue = DataUtils.ParseRecord(results.First());
                    if (rootValue.Parent is not null)
                    {
                        rootValue.Parent.Children.Remove(rootValue);
                        rootValue.Parent = null;
                    }
                    firstPart.Add(rootValue);
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
                    Func<BsonDocument,bool> firstChildQuery = (record) => {
                        Family currentNode = DataUtils.ParseRecord(record);
                        return currentNode.Parent == tempNext && tempNext.Children.Count > 0 && currentNode == tempNext.Children.First() && !vistedNodes.Where((part) => part.Contains(currentNode)).Any();
                    };
                    List<BsonDocument> firstChildResults = Collection.Find((record) => firstChildQuery(record)).ToList();
                    if (firstChildResults.Count > 0)
                    {
                        return DataUtils.ParseRecord(firstChildResults.First());
                    }
                    else if (tempNext != Root)
                    {
                        int sibilingIndex = tempNext.Parent.Children.ToList().IndexOf(tempNext);
                        if (sibilingIndex > -1 && sibilingIndex < tempNext.Parent.Children.Count)
                        {
                            Func<BsonDocument,bool> sibilingQuery = (record) => {
                                Family currentNode = DataUtils.ParseRecord(record);
                                return tempNext.Parent.Children.ToList().IndexOf(currentNode) == sibilingIndex + 1;
                            };
                            List<BsonDocument> sibilingResults = Collection.Find((record) => sibilingQuery(record)).ToList();
                            if (sibilingResults.Count > 0)
                            {
                                return DataUtils.ParseRecord(sibilingResults.First());
                            }
                        }
                        return FindNext(tempNext.Parent);
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

            private static bool IsFirst(BsonDocument record)
            {
                return DataUtils.ParseRecord(record).Parent is null;
            }
        }
    }
}