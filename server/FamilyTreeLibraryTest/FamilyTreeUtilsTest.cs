using FamilyTreeLibrary;

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
                string appSettingsFilePath = FamilyTreeUtils.GetFilePathOf(@"server\FamilyTreeLibrary\appsettings.json");
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
        public void TestGetFilePathOfConfiguration()
        {
            string actual = FamilyTreeUtils.GetFilePathOf(@"server\FamilyTreeLibrary\appsettings.json");
            string expected = @"C:\Users\zakme\Documents\FamilyTreeProject\server\FamilyTreeLibrary\appsettings.json";
            Assert.That(actual, Is.EqualTo(expected));
        }
        
        [Test]
        public void TestGetFilePathOfPfingstenBook()
        {
            string actaul = FamilyTreeUtils.GetFilePathOf(@"resources\PfingstenBook2023.pdf");
            Assert.That(actaul, Is.EqualTo(PDF_FILE));
        }

        [Test]
        public void TestInitializeLogger()
        {
            try
            {
                FamilyTreeUtils.InitializeLogger();
                FamilyTreeUtils.LogMessage(LoggingLevels.Information,"Logging Something.");
                string expectedLogPath = @"C:\Users\zakme\Documents\FamilyTreeProject\resources\Logs\log20240302.txt";
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

        [Test]
        public void TestGetRootDirectory()
        {
            string expected = @"C:\Users\zakme\Documents\FamilyTreeProject";
            Assert.That(FamilyTreeUtils.GetRootDirectory(), Is.EqualTo(expected));
        }
    }
}