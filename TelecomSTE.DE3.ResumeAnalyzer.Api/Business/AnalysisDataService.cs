using Amazon.S3;
using Amazon.S3.Model;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Business.Interfaces;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Configuration;
using TelecomSTE.DE3.ResumeAnalyzer.Api.DataAccess.Interfaces;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Model;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Transport;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.Business
{
    public class AnalysisDataService : IAnalysisDataService
    {
        #region Props & ctor
        private readonly IAnalysisRepository analysisRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IUpdateTimeRepository updateTimeRepository;
        private readonly IAmazonS3 amazonS3Client;
        private readonly Settings settings;
        private Dictionary<int, string> numberByCategory;

        public AnalysisDataService(IAnalysisRepository analysisRepository, IAmazonS3 amazonS3Client, Settings settings, ICategoryRepository categoryRepository, IUpdateTimeRepository updateTimeRepository)
        {
            this.analysisRepository = analysisRepository;
            this.amazonS3Client = amazonS3Client;
            this.settings = settings;
            this.updateTimeRepository = updateTimeRepository;
            this.categoryRepository = categoryRepository;
        }

        #endregion

        #region Public Methods
        public AnalysisByCategoryDto GetResultsByCategory(string category)
        {
            this.numberByCategory = GetCategoryDictionary();
            var number = this.numberByCategory.FirstOrDefault(x => x.Value == category).Key;
            var list = this.analysisRepository.GetByCategoryPredict(category);
            string text = "";
            foreach(var result in list)
            {
                text += result.Text;
            }

            var words = text.Split(" ").ToList();
            var dico = words.GroupBy(x => x).ToDictionary(x => x.Key, y => y.Count());

            return new AnalysisByCategoryDto() {Category = category, WeightByWords=dico };
        }


        public DateTime GetLastUpdated()
        {
            return this.updateTimeRepository.Get().First().LastUpdated;
        }

        public IEnumerable<AnalysisResultDto> GetResults()
        {
            var a = this.analysisRepository.Get().Select(x => AnalysisResultDto.FromModel(x,this.numberByCategory));
            return a;
        }

        public async Task<DateTime> UpdateAnalysisData()
        {
            var lastUpdated = GetLastUpdated();
            var keys = await GetLatestFileKeys(lastUpdated);
            var files = await GetFiles(keys);
            UpdateAnalysisMongo(files);
            DateTime latestUpdate = files.OrderBy(x => x.LastModified).First().LastModified;
            SetLastUpdated(latestUpdate);
            return latestUpdate;
        }

        #endregion

        #region Private Methods

        private Dictionary<int, string> GetCategoryDictionary()
        {
            var categories = this.categoryRepository.Get();
            return categories.ToDictionary(x => x.Number, y => y.Label);
        }

        private void UpdateAnalysisMongo(IEnumerable<PredictFile> files)
        {
            var newResults = new List<AnalysisResult>();
            foreach(var file in files)
            {
                using (TextReader reader = new StringReader(file.Content))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<AnalysisResult>();
                    newResults.AddRange(records);
                }
            }

            foreach(var result in newResults)
            {
                this.analysisRepository.Create(result);
            }
        }


        public void SetLastUpdated(DateTime date)
        {
            var updateTime = this.updateTimeRepository.Get().FirstOrDefault();
            if(updateTime != null)
            {
                updateTime.LastUpdated = date;
                this.updateTimeRepository.Update(updateTime.Id,updateTime);
            }
            else
            {
                this.updateTimeRepository.Create(new UpdateTime() { LastUpdated = date });
            }

        }

        private async Task<IEnumerable<PredictFile>> GetFiles(IEnumerable<string> keys)
        {
            List<PredictFile> files = new List<PredictFile>();
            foreach (var key in keys)
            {
                string responseBody = "";
                try
                {
                    GetObjectRequest request = new GetObjectRequest
                    {
                        BucketName = settings.BucketName,
                        Key = key
                    };
                    using (GetObjectResponse response = await this.amazonS3Client.GetObjectAsync(request))
                    using (Stream responseStream = response.ResponseStream)
                    using (StreamReader reader = new StreamReader(responseStream))
                    {

                        responseBody = reader.ReadToEnd();
                        PredictFile file = new PredictFile()
                        {
                            Content = responseBody,
                            Title = response.Metadata["x-amz-meta-title"],
                            LastModified = response.LastModified
                        };
                        files.Add(file);
                    }
                }
                catch (AmazonS3Exception e)
                {

                }
                catch (Exception e)
                {
                }
            }
            return files;
        }

        private async Task<IEnumerable<string>> GetLatestFileKeys(DateTime lastUpdated)
        {
            List<string> fileKeys = new List<string>();
            try
            {
                ListObjectsV2Request request = new ListObjectsV2Request
                {
                    BucketName = settings.BucketName,
                    MaxKeys = 10
                };
                ListObjectsV2Response response;
                do
                {
                    response = await amazonS3Client.ListObjectsV2Async(request);
                    // Récupération des clés des fichiers modifiés après la dernière date d'update
                    fileKeys.AddRange(response.S3Objects.Where(x => x.LastModified > lastUpdated).Select(x => x.Key));
                    request.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated);
            }
            catch (Exception e)
            {
                throw;
            }

            return fileKeys;
        }

        #endregion

    }
}

