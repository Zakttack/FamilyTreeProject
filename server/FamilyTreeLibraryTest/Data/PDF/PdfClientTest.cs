using FamilyTreeLibrary;
using FamilyTreeLibrary.Data.PDF;
using Microsoft.Extensions.Configuration;

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
                string appSettingsFilePath = FamilyTreeUtils.GetFilePathOf("appsettings.json");
                IConfiguration configuration = FamilyTreeUtils.GetConfiguration(appSettingsFilePath);
                FamilyTreeUtils.InitializeLogger(configuration);
                string filePath = FamilyTreeUtils.GetFilePathOf(@"resources\2023PfingstenBookAlternate.pdf");
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

        [Test]
        public void TestAttachNodes()
        {
            try
            {
                if (problem is not null)
                {
                    throw problem;
                }
                client.LoadNodes();
                client.AttachNodes();
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