using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
using FamilyTreeLibrary.PDF;
using Serilog;
namespace FamilyTreeLibraryTest.PDF
{
    public class PdfClientTest
    {
        [Test]
        public void TestLogging()
        {
            try
            {
                PdfClient client = new("2023PfingtenBookAlternate.pdf");
                client.LoadNodes();
            }
            catch (Exception ex)
            {
                Assert.Fail($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            }
            Assert.Pass();
        }

        [Test]
        public void VerifyDefaultValue()
        {
            
        }
    }
}