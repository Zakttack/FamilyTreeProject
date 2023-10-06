using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary.PDF.Models
{
    public class Line
    {
        public Line(string name = null, Queue<DateTime> dates = default)
        {
            Name = name;
            Dates = dates == default ? new() : dates;
        }
        public string Name
        {
            get;
            set;
        }

        public Queue<DateTime> Dates
        {
            get;
        }

        public Line Copy()
        {
            return new(Name,Dates);
        }
    }
}