using FamilyTreeLibrary;
using FamilyTreeLibrary.Service;

namespace FamilyTreeLibraryTest.Service
{
    public class FamilyTreeServiceTest
    {
        private FamilyTreeService service;
        private Exception problem;
        [SetUp]
        public void Setup()
        {
            try
            {
                string familyName = "Pfingsten";
                service = new(familyName);
                problem = null;
            }
            catch (Exception ex)
            {
                FamilyTreeUtils.WriteError(ex);
                problem = ex;
            }
        }

        [Test]
        public void TestAppendTree()
        {
            try
            {
                if (problem is not null)
                {
                    throw problem;
                }
                string templateFilePath = FamilyTreeUtils.GetFileNameFromResources(Directory.GetCurrentDirectory(), "2023PfingstenBookAlternate.pdf");
                service.AppendTree(templateFilePath);
            }
            catch (Exception ex)
            {
                if (ex != problem)
                {
                    FamilyTreeUtils.WriteError(ex);
                    problem = ex;
                }
            }
            finally
            {
                Assert.That(problem, Is.Null);
            }
        }
    }
}