using FamilyTreeLibrary;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Service;
using Microsoft.Extensions.Configuration;

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
                string appSettingsFilePath = FamilyTreeUtils.GetFilePathOf("appsettings.json");
                IConfiguration configuration = FamilyTreeUtils.GetConfiguration(appSettingsFilePath);
                FamilyTreeUtils.InitializeLogger(configuration);
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
        public void TestGetNumberOfGenerations()
        {
            Assert.That(service.NumberOfGenerations, Is.EqualTo(6));
        }

        [Test]
        public void TestGetNumberOfFamilies()
        {
            Assert.That(service.NumberOfFamilies, Is.EqualTo(777));
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
                string templateFilePath = FamilyTreeUtils.GetFilePathOf(@"resources\2023PfingstenBookAlternate.pdf");
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

        [Test]
        public void TestReportMarried()
        {
            try
            {
                if (problem is not null)
                {
                    throw problem;
                }
                Person member = new("Cade Alan Merrigan", new FamilyTreeDate("29 Oct 2000"), FamilyTreeDate.DefaultDate);
                Person inLaw = new("Abigail Faleide", new FamilyTreeDate("25 Dec 2001"), FamilyTreeDate.DefaultDate);
                FamilyTreeDate marriageDate = new("18 Aug 2023");
                service.ReportMarried(member, inLaw, marriageDate);
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