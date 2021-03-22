using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.Model
{

    public class AnalysisResult : MongoEntityBase
    {

        [BsonElement("Name")]
        public string BookName { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public string Author { get; set; }
    }
}
