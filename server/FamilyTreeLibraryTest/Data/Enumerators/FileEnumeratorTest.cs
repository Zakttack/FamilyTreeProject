using FamilyTreeLibrary;
using FamilyTreeLibrary.Data.Enumerators;

namespace FamilyTreeLibraryTest.Data.Enumerators
{
    public class FileEnumeratorTest
    {
        private IEnumerator<string> fileEnum;
        private Exception problem;

        [SetUp]
        public void InitializeEnumerator()
        {
            FamilyTreeUtils.InitializeLogger();
            try
            {
                fileEnum = new FileEnumerator();
            }
            catch (Exception ex)
            {
                problem = ex;
            }
        }
        [Test]
        public void TestEnumerator()
        {
            try
            {
                while(fileEnum.MoveNext())
                {
                    FamilyTreeUtils.LogMessage(LoggingLevels.Debug, fileEnum.Current);
                }
                fileEnum.Dispose();
            }
            catch (Exception ex)
            {
                problem = ex;
            }
            Assert.That(problem, Is.Null);
        }
        [Test]
        public void TestRootDirectoryExistence()
        {
            Assert.That(problem, Is.Null);
        }
    }
}