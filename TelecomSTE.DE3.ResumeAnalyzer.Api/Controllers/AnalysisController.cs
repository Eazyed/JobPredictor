using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Business.Interfaces;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Model;

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

        [HttpGet]
        public IEnumerable<AnalysisResult> Get()
        {
            IEnumerable<AnalysisResult> a = new List<AnalysisResult>();
            return a;
        }
    }
}
