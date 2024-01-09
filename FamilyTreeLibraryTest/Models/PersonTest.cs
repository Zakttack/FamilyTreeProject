using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyTreeLibrary.Models;
using NUnit.Framework;

namespace FamilyTreeLibraryTest.Models
{
    public class PersonTest
    {
        [Test]
        public void TestCompareTo1()
        {
            Person p1 = new("Margaret Ann Lass", new FamilyTreeDate("5 Apr 1947"), FamilyTreeDate.DefaultDate);
            Person p2 = new("Margaret Ann Lass Merrigan", new FamilyTreeDate("5 Apr 1947"), FamilyTreeDate.DefaultDate);
            Assert.That(p1.CompareTo(p2), Is.EqualTo(0));
        }
    }
}