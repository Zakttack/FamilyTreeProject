using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary.Data.Databases
{
    public interface IContainer<T>
    {
        public T this[Guid id]
        {
            get;
            set;
        }

        public void Remove(T item);
    }
}