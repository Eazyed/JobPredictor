using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.Model
{
    public class WordCount : MongoEntityBase
    {
        public string Category { get;set; }
        public Dictionary<string,int> CountByWord { get; set; }
    }
}
