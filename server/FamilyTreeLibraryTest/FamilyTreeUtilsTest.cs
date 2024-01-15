using FamilyTreeLibrary;

namespace FamilyTreeLibraryTest
{
    public class FamilyTreeUtilsTest
    {
        internal const string PDF_FILE = @"C:\Users\zakme\Documents\FamilyTreeProject\Resources\PfingstenBook2023.pdf";

        [Test]
        public void TestGetConfiguration()
        {
            object result;
            try
            {
                string appSettingsFilePath = FamilyTreeUtils.GetFileNameFromResources(Directory.GetCurrentDirectory(), "appsettings.json");
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
        public void TestGetFileNameFromResources()
        {
            string actaul = FamilyTreeUtils.GetFileNameFromResources(Directory.GetCurrentDirectory(), "PfingstenBook2023.pdf");
            Assert.That(actaul, Is.EqualTo(PDF_FILE));
        }

        [Test]
        public void TestInitializeLogger()
        {
            try
            {
                FamilyTreeUtils.InitializeLogger();
            }
            catch (Exception)
            {
                Assert.Fail("Unable to initialize logger.");
            }
            Assert.Pass("Logger detected.");
        }
    }
}