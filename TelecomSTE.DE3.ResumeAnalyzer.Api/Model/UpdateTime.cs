using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.Model
{
    public class UpdateTime : MongoEntityBase
    {
        public DateTime LastUpdated { get; set; }
    }
}
