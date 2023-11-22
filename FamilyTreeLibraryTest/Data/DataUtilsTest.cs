using FamilyTreeLibrary.Data;
using Serilog;
namespace FamilyTreeLibraryTest.Data
{
    public class DataUtilsTest
    {
        [Test]
        public void TestPfingstenCollectionConnection()
        {
            Exception problem;
            try
            {
                string familyName = "Pfingsten";
                DataUtils.GetCollection(familyName);
                Log.Information("Connection Successful.");
                problem = null;
            }
            catch (Exception ex)
            {
                Log.Fatal($"Unable to Connect.\n{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                problem = ex;
            }
            Assert.That(problem, Is.Null);
        }

    }
}