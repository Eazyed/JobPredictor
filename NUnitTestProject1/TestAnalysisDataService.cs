using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Business;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Configuration;
using TelecomSTE.DE3.ResumeAnalyzer.Api.DataAccess;
using TelecomSTE.DE3.ResumeAnalyzer.Api.DataAccess.Interfaces;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Model;

namespace NUnitTestProject1
{
    public class TestAnalysisDataService
    {
        private readonly Settings settings;
        private readonly AnalysisRepository analysisRepository;
        private readonly CategoryRepository categoryRepository;
        private readonly UpdateTimeRepository updateTimeRepository;
        private readonly AnalysisDataService analysisDataService;
        public TestAnalysisDataService()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                 .Build();
            var settings = new Settings();
            var aws = config.GetAWSOptions();
            ConfigurationBinder.Bind(config, settings);
            this.settings = settings;
            this.analysisRepository = new AnalysisRepository(this.settings);
            this.categoryRepository = new CategoryRepository(this.settings);
            this.updateTimeRepository = new UpdateTimeRepository(this.settings);
            this.analysisDataService = new AnalysisDataService(analysisRepository, aws.CreateServiceClient<IAmazonS3>(), settings, categoryRepository, updateTimeRepository);
        }

        [SetUp]
        public void Setup()
        {
            var date = DateTime.UtcNow.AddYears(-5);
            this.analysisDataService.SetLastUpdated(date);
        }

        [TearDown]
        public void TearDown()
        {
            this.analysisRepository.DropCollection();
            this.categoryRepository.DropCollection();
            this.updateTimeRepository.DropCollection();            
        }


        [Test]
        public void InitDate()
        {
            var date = DateTime.UtcNow.AddYears(-1);
            this.analysisDataService.SetLastUpdated(date);
            var time = this.updateTimeRepository.Get().First();
            // ---- Problèmes d'arrondi avec Mongo
            Assert.AreEqual(date.Date, time.LastUpdated.Date);
            Assert.AreEqual(date.Second, time.LastUpdated.Second);
            Assert.AreEqual(date.Hour, time.LastUpdated.Hour);
            Assert.AreEqual(date.Minute, time.LastUpdated.Minute);
        }

        [Test]
        public void CreateCategory()
        {
            for(int i = 0; i < 29; i++)
            {
                Category cat = new Category() { Number = i, Label = "Test"+i.ToString() };
                this.categoryRepository.Create(cat);
            }

            bool a = this.categoryRepository.Get().Count == 29;
            Assert.IsTrue(a);
        }

        [Test]
        public void CreateAnalysisResult()
        {
            for(int i = 0; i < 200; i++)
            {
                AnalysisResult analysisResult = new AnalysisResult()
                {
                    OriginalId = i.ToString(),
                    BatchTimestamp = DateTime.Now.ToString(),
                    Category= (i%29).ToString(),
                    CategoryPredict = i%2 ==0 ? (i % 29).ToString() : (i+1).ToString(),
                    Text = i % 2 == 0 ? "Test Test Test Bleu Bleu Bleu Bleu Rouge Bleu Violet Jaune Vert Vert Marron Jambon" : "Tonnerre Air Eau Feu Psy Sol Sol Psy Eau Eau Eau Electrique Plante Plante Plante"
                };
                this.analysisRepository.Create(analysisResult);
            }

            bool a = this.analysisRepository.Get().Count == 200;
            Assert.IsTrue(a);
        }

        [Test]
        public async Task UpdateData()
        {
            await this.analysisDataService.UpdateAnalysisData();
        }

        [Test]
        public void GetResult()
        {
            CreateCategory();
            CreateAnalysisResult();
            var a = this.analysisDataService.GetResults();
            var b = this.analysisDataService.GetResultsByCategory("Test1");
            Assert.IsTrue(b.WeightByWords.ContainsKey("Bleu"));
            Assert.IsTrue(a.Any());
        }
    }
}