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
        private readonly IAnalysisResultRepository analysisRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IUpdateTimeRepository updateTimeRepository;
        private readonly IWordCountRepository wordCountRepository;
        private readonly IAmazonS3 amazonS3Client;
        private readonly Settings settings;
        private Dictionary<int, string> numberByCategory;

        public AnalysisDataService(IAnalysisResultRepository analysisRepository, IAmazonS3 amazonS3Client, Settings settings, ICategoryRepository categoryRepository, IUpdateTimeRepository updateTimeRepository, IWordCountRepository wordCountRepository)
        {
            this.analysisRepository = analysisRepository;
            this.amazonS3Client = amazonS3Client;
            this.wordCountRepository = wordCountRepository;
            this.settings = settings;
            this.updateTimeRepository = updateTimeRepository;
            this.categoryRepository = categoryRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Récupération des wordcount par catégorie
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public AnalysisByCategoryDto GetResultsByCategory(string category)
        {
            this.numberByCategory = GetCategoryDictionary();
            var number = this.numberByCategory.FirstOrDefault(x => x.Value == category).Key;
            var wordCount = this.wordCountRepository.GetByCategoryPredict(number.ToString()).FirstOrDefault();
            if (wordCount != null)
            {
                return new AnalysisByCategoryDto() { Category = wordCount.Category, WeightByWords = wordCount.CountByWord };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Méthode de récupération de la date de dernier update
        /// </summary>
        /// <returns></returns>
        public DateTime GetLastUpdated()
        {
            var date = this.updateTimeRepository.Get().FirstOrDefault();
            if(date == default(UpdateTime))
            {
                return default(DateTime);
            }
            else
            {
                return date.LastUpdated;
            }
        }

        /// <summary>
        /// Méthode de récupération des prédictions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AnalysisResultDto> GetResults()
        {
            this.numberByCategory = GetCategoryDictionary();
            var a = this.analysisRepository.Get().Select(x => AnalysisResultDto.FromModel(x,this.numberByCategory));
            return a;
        }

        /// <summary>
        /// Mise à jour des données dans Mongo
        /// </summary>
        /// <returns></returns>
        public async Task<DateTime> UpdateAnalysisData()
        {
            // Récupération de la date du dernier fichier de S3 rapatrié en local
            var lastUpdated = GetLastUpdated();
            // Récupération des clés (noms) des fichiers postérieurs à la date précédente
            var keys = await GetLatestFileKeys(lastUpdated);
            // Récupération des fichiers à partir de leur nom
            var files = await GetFiles(keys);
            // Mise à jour des données d'analyses dans Mongo
            UpdateAnalysisMongo(files);
            // Récupération de la date du dernier fichier S3 rapatrié en local
            DateTime latestUpdate = files.OrderBy(x => x.LastModified).First().LastModified;
            // Stockage de cette date
            SetLastUpdated(latestUpdate);
            // On retourne la date pour affichage côté front
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
            CsvHelper.Configuration.CsvConfiguration config = 
                new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture) { 
                    Delimiter = "|" ,
                    HeaderValidated = null
                };

            foreach (var file in files)
            {
                using (TextReader reader = new StringReader(file.Content))
                using (var csv = new CsvReader(reader, config))
                {
                    var records = csv.GetRecords<AnalysisResult>();
                    newResults.AddRange(records);
                }
            }

            if(newResults.Count > 0)
            {
                foreach (var result in newResults)
                {
                    this.analysisRepository.Create(result);
                }

                UpdateWordCount(newResults);
            }

        }

        private void UpdateWordCount(List<AnalysisResult> newResults)
        {
            Dictionary<string, int> countByWord = new Dictionary<string, int>();

            var resultsByCategory = newResults.GroupBy(x => x.CategoryPredict).ToDictionary( x => x.Key,y => y.ToList());
            foreach(var categoryPair in resultsByCategory)
            {
                foreach (var result in categoryPair.Value)
                {
                    var words = result.Text.Split(" ").ToList();
                    var currentCount = words.GroupBy(x => x).ToDictionary(x => x.Key, y => y.Count());
                    countByWord = MergeCounts(countByWord, currentCount);
                }

                var oldCounts = this.wordCountRepository.GetByCategoryPredict(categoryPair.Key);

                if (oldCounts.Count() > 0)
                {
                    var oldCount = oldCounts.First();
                    oldCount.CountByWord = MergeCounts(oldCount.CountByWord, countByWord);
                    this.wordCountRepository.Update(oldCount.Id, oldCount);
                }
                else
                {
                    this.wordCountRepository.Create(new WordCount() { Category = categoryPair.Key, CountByWord = countByWord });
                }
            }

            
        }

        private Dictionary<string, int> MergeCounts(Dictionary<string, int> countByWord, Dictionary<string, int> currentCount)
        {
            foreach(var count in currentCount)
            {
                if (countByWord.ContainsKey(count.Key))
                {
                    countByWord[count.Key] += count.Value;
                }
                else
                {
                    countByWord.Add(count.Key, count.Value);
                }
            }
            return countByWord;
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
                catch (Exception)
                {
                    // Le log serait ici
                    throw;
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
                    fileKeys.AddRange(response.S3Objects.Where(x => x.LastModified.ToUniversalTime() > lastUpdated).Select(x => x.Key));
                    request.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated);
            }
            catch (Exception e)
            {
                // Log
                throw;
            }

            return fileKeys;
        }

        #endregion

    }
}

