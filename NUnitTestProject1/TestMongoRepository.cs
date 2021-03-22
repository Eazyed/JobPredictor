using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Configuration;
using TelecomSTE.DE3.ResumeAnalyzer.Api.DataAccess;
using TelecomSTE.DE3.ResumeAnalyzer.Api.DataAccess.Interfaces;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Model;

namespace NUnitTestProject1
{
    public class TestMongoRepository
    {
        private readonly Settings settings;
        private readonly AnalysisRepository analysisRepository;
        private readonly CategoryRepository categoryRepository;
        public TestMongoRepository()
        {
            this.settings = InitConfiguration();
            //this.analysisRepository = new AnalysisRepository(this.settings,"AnalysisResultTest");
            this.categoryRepository = new CategoryRepository(this.settings, "CategoryTest");
        }
        private Settings InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var settings = new Settings();
            ConfigurationBinder.Bind(config, settings);
            return settings;
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreateCategory()
        {
            Category cat = new Category() { Number= 0, Label = "Test" };
            this.categoryRepository.Create(cat);
            bool a = this.categoryRepository.Get().Count == 1;
            Assert.IsTrue(a);
        }
    }
}