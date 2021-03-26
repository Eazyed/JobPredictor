using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.Model
{
    public class MongoEntityBase : IMongoEntity
    {
        [CsvHelper.Configuration.Attributes.Ignore]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
