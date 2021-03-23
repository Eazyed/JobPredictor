using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.Transport
{
    public class AnalysisByCategoryDto
    {
        public string Category { get; set; }

        public Dictionary<string,int> WeightByWords { get; set; }
    }
}
