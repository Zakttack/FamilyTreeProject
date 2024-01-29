using FamilyTreeLibrary;
using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Models;

namespace FamilyTreeLibraryTest.Data
{
    public class TreeTest
    {
        private ITree familyTree;
        [SetUp]
        public void Setup()
        {
            FamilyTreeUtils.InitializeLogger();
            familyTree = new Tree("Pfingsten");
        }

        [Test]
        public void TestContains()
        {
            Family parent = new(new Person("Brent Alan Merrigan", new FamilyTreeDate("24 Dec 1970"), FamilyTreeDate.DefaultDate),
            new Person("Jolene Joyce Rants", new FamilyTreeDate("12 Jun 1968"), FamilyTreeDate.DefaultDate),
            new FamilyTreeDate("30 Aug 1997"));
            Family child = new(new Person("Cade Alan Merrigan", new FamilyTreeDate("29 Oct 2000"), FamilyTreeDate.DefaultDate), null, FamilyTreeDate.DefaultDate);
            Assert.That(familyTree.Contains(parent, child), Is.True);
        }
    }
}