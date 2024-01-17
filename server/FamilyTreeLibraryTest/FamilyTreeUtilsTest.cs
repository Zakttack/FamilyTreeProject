using FamilyTreeLibrary;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace FamilyTreeLibraryTest
{
    public class FamilyTreeUtilsTest
    {
        internal const string PDF_FILE = @"C:\Users\zakme\Documents\FamilyTreeProject\resources\PfingstenBook2023.pdf";

        [Test]
        public void TestGetConfiguration()
        {
            object result;
            try
            {
                string appSettingsFilePath = FamilyTreeUtils.GetFilePathOf("appsettings.json");
                result = FamilyTreeUtils.GetConfiguration(appSettingsFilePath);
            }
            catch (Exception ex)
            {
                result = null;
                Assert.Fail($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            }
            Assert.Pass(result.ToString());
        }
        
        [Test]
        public void TestGetFilePathOf()
        {
            string actaul = FamilyTreeUtils.GetFilePathOf(@"resources\PfingstenBook2023.pdf");
            Assert.That(actaul, Is.EqualTo(PDF_FILE));
        }

        [Test]
        public void TestInitializeLogger()
        {
            try
            {
                string appSettingsFilePath = FamilyTreeUtils.GetFilePathOf("appsettings.json");
                IConfiguration configuration = FamilyTreeUtils.GetConfiguration(appSettingsFilePath);
                FamilyTreeUtils.InitializeLogger(configuration);
                Log.Information("Logging Something.");
                string expectedLogPath = @"C:\Users\zakme\Documents\FamilyTreeProject\resources\Logs\log20240116.txt";
                if (!File.Exists(expectedLogPath))
                {
                    throw new FileNotFoundException("The logger was unable to generate a text file.");
                }
            }
            catch (Exception)
            {
                Assert.Fail("Unable to initialize logger.");
            }
            Assert.Pass("Logger detected.");
        }
    }
}