using System.Data;
using FamilyTreeLibrary.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections;


namespace FamilyTreeLibrary.Data
{
    public class FamilyTree : IFamilyTree
    {
        private readonly IMongoCollection<Family> mongoCollection;
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

        public IEnumerable<Family> this[Person member]
        {
            get
            {
                return this.Where((node) => node.Member == member);
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

        public void Add(Family node)
        {
            mongoCollection.InsertOne(node);
            Family parentInitial = DataUtils.GetParentOf(node, mongoCollection);
            Family parentFinal = parentInitial;
            parentFinal.Children.Add(node.Member);
            mongoCollection.UpdateOne(parentInitial.ToBsonDocument(), parentFinal.ToBsonDocument());
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
            mongoCollection.UpdateOne(initialNode.ToBsonDocument(), finalNode.ToBsonDocument());
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

            private readonly IMongoCollection<Family> mongoCollection;

            public FamilyEnumerator(IMongoCollection<Family> collection, Family initialRoot)
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
                IEnumerable<Family> results;
                Func<Family,bool> rootInitializeQuery;
                if (initialRoot is null)
                {
                    rootInitializeQuery = (record) => {
                        return record.Parent is null;
                    };
                    results = mongoCollection.AsQueryable().Where(record => rootInitializeQuery(record)).AsEnumerable();
                }
                else
                {
                    rootInitializeQuery = (record) => {
                        return record == initialRoot;
                    };
                    results = mongoCollection.AsQueryable().Where((record) => rootInitializeQuery(record)).AsEnumerable();
                }
                if (results.Any())
                {
                    Family rootValue = results.First();
                    if (rootValue.Parent is not null)
                    {
                        Family parent = DataUtils.GetParentOf(rootValue, mongoCollection);
                        parent.Children.Remove(rootValue.Member);
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