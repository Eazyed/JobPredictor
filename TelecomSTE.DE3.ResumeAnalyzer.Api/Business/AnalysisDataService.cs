using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Business.Interfaces;
using TelecomSTE.DE3.ResumeAnalyzer.Api.DataAccess.Interfaces;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.Business
{
    public class AnalysisDataService : IAnalysisDataService
    {
        private readonly IAnalysisRepository analysisRepository;

        public AnalysisDataService(IAnalysisRepository analysisRepository)
        {
            this.analysisRepository = analysisRepository;
        }
    }
}
