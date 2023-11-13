using FamilyTreeLibrary.Data;
namespace FamilyTreeLibraryTest.Data
{
    public class DataUtilsTest
    {
        [Test]
        public void TestPfingstenCollectionConnection()
        {
            object result;
            try
            {
                string familyName = "Pfingsten";
                result = DataUtils.GetCollection(familyName);
            }
            catch (Exception ex)
            {
                result = null;
                Assert.Fail($"{ex.GetType().Name}: {ex.Message}\n {ex.StackTrace}");
            }
            Assert.Pass(result.ToString());
        }

    }
}