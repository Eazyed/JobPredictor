using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Model;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.Transport
{
    public class AnalysisResultDto
    {
        public string OriginalId { get; set; }

        public string Category { get; set; }

        public string CategoryPredict { get; set; }

        public static AnalysisResultDto FromModel(AnalysisResult x, Dictionary<int, string> numberByCategory)
        {
            return new AnalysisResultDto() { 
                OriginalId = x.OriginalId,
                Category = numberByCategory[int.Parse(x.Category)],
                CategoryPredict = numberByCategory[int.Parse(x.CategoryPredict)] 
            };
        }
    }
}
