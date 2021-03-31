using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Business.Interfaces;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Model;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Transport;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly IAnalysisDataService analysisDataService;

        private readonly ILogger<AnalysisController> _logger;

        public AnalysisController(ILogger<AnalysisController> logger, IAnalysisDataService analysisDataService)
        {
            _logger = logger;
            this.analysisDataService = analysisDataService;
        }

        [HttpGet("/analysis")]
        public IEnumerable<AnalysisResultDto> GetAnalysisResult()
        {
            return this.analysisDataService.GetResults(); 
        }


        [HttpGet("/update")]
        public async Task<DateTime> UpdateResult()
        {
            return await this.analysisDataService.UpdateAnalysisData();
        }

        [HttpGet("/wordcount")]
        public AnalysisByCategoryDto GetWordCount(string category)
        {
            return this.analysisDataService.GetResultsByCategory(category);
        }

        [HttpGet("/lastupdate")]
        public DateTime GetLastUpdate()
        {
            return this.analysisDataService.GetLastUpdated();
        }
    }
}
