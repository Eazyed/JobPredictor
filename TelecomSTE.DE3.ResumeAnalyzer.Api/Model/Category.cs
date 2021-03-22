using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.Model
{
    public class Category : MongoEntityBase
    {
        public int Number { get; set; }
        public string Label { get; set; }
    }
}
