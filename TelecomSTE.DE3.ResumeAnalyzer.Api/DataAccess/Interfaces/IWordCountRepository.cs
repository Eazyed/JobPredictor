﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Model;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.DataAccess.Interfaces
{
    public interface IWordCountRepository : IMongoRepositoryBase<WordCount> 
    {
        IEnumerable<WordCount> GetByCategoryPredict(string category);
    }
}
