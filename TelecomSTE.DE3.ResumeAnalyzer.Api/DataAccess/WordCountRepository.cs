using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Configuration;
using TelecomSTE.DE3.ResumeAnalyzer.Api.DataAccess.Interfaces;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Model;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.DataAccess
{
    public class WordCountRepository : MongoRepositoryBase<WordCount>, IWordCountRepository
    {
        public WordCountRepository(Settings settings) :
            base(settings,nameof(WordCount))
        {

        }

        public IEnumerable<WordCount> GetByCategoryPredict(string category)
        {
            return _collection.Find(entity => entity.Category == category).ToList();
        }
    }
}
