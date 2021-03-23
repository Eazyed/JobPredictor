using Amazon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.Configuration
{
    public class Settings
    {
        public string MongoAnalysisCollectionName { get; set; }
        public string MongoConnectionString { get; set; }
        public string MongoDataBaseName { get; set; }
        public string MongoCategoryCollectionName { get;  set; }
        public string MongoUpdateTimeCollectionName { get;  set; }
        public string AwsRegion { get; set; }
        public string BucketName { get; set; }
    }
}
