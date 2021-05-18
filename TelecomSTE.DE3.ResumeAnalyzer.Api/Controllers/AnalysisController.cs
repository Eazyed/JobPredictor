using Microsoft.AspNetCore.Cors;
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
    [EnableCors]
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

        /// <summary>
        /// Méthode pour obtenir les résultat d'analyses de type <see cref="AnalysisResultDto"/>
        /// </summary>
        /// <returns>Tous les résultats d'analyses en base</returns>
        [HttpGet("/analysis")]
        public IEnumerable<AnalysisResultDto> GetAnalysisResult()
        {
            try
            {
                return this.analysisDataService.GetResults();
            }catch(Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// Méthode pour lancer la mise à jour des données en base
        /// </summary>
        /// <returns>La date de rafraichissement</returns>
        [HttpGet("/update")]
        public async Task<DateTime> UpdateResult()
        {
            return await this.analysisDataService.UpdateAnalysisData();
        }

        /// <summary>
        /// Méthode pour récupérer les analyses WordCount Par catégorie
        /// </summary>
        /// <param name="category"> le label de la catégorie</param>
        /// <returns>Le <see cref="AnalysisByCategoryDto"/> correspondant au label</returns>
        [HttpGet("/wordcount")]
        public AnalysisByCategoryDto GetWordCount(string category)
        {
            return this.analysisDataService.GetResultsByCategory(category);
        }

        /// <summary>
        /// Méthode renvoyant la date du dernier fichier pris en compte
        /// </summary>
        /// <returns> Date du dernier fichier pris en compte</returns>
        [HttpGet("/lastupdate")]
        public DateTime GetLastUpdate()
        {
            return this.analysisDataService.GetLastUpdated();
        }
    }
}
