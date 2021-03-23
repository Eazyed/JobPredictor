using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.Model
{

    public class AnalysisResult : MongoEntityBase
    {
        public string OriginalId { get; set; }

        public string Text { get; set; }

        public string Category { get; set; }

        public string CategoryPredict { get; set; }

        public string BatchTimestamp { get; set; }
    }
}
