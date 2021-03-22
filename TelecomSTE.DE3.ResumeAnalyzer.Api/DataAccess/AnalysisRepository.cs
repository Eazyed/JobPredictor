﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Configuration;
using TelecomSTE.DE3.ResumeAnalyzer.Api.DataAccess.Interfaces;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Model;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.DataAccess
{
    public class AnalysisRepository : MongoRepositoryBase<AnalysisResult>, IAnalysisRepository
    {
        public AnalysisRepository(Settings settings, string collectionName) :
            base(settings,collectionName)
        {

        }
    }
}