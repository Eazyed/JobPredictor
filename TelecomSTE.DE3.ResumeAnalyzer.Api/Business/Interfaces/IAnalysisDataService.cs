using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Model;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Transport;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.Business.Interfaces
{
    public interface IAnalysisDataService
    {
        Task<DateTime> UpdateAnalysisData();

        DateTime GetLastUpdated();

        IEnumerable<string> GetCategories();

        IEnumerable<AnalysisResultDto> GetResults();

        AnalysisByCategoryDto GetResultsByCategory(string category);
    }
}
