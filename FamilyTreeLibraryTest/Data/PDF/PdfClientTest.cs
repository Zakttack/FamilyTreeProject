using FamilyTreeLibrary;
using FamilyTreeLibrary.Data.PDF;

namespace FamilyTreeLibraryTest.Data.PDF
{
    public class PdfClientTest
    {
        private PdfClient client;
        private Exception problem;
        [SetUp]
        public void Setup()
        {
            try
            {
                FamilyTreeUtils.InitializeLogger();
                string filePath = FamilyTreeUtils.GetFileNameFromResources(Directory.GetCurrentDirectory(), "2023PfingstenBookAlternate.pdf");
                client = new(filePath);
                problem = null;
            }
            catch (Exception ex)
            {
                problem = ex;
            }
        }

        [Test]
        public void TestLoadNodes()
        {
            try
            {
                if (problem is not null)
                {
                    throw problem;
                }
                client.LoadNodes();
                problem = null;
            }
            catch (Exception ex)
            {
                problem = ex;
                FamilyTreeUtils.WriteError(problem);
            }
            finally
            {
                Assert.That(problem, Is.Null);
            }
        }
    }
}