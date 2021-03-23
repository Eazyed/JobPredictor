using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Model;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.DataAccess.Interfaces
{
    public interface IMongoRepositoryBase<T> 
        where T : IMongoEntity
    {
        T Create(T entity);
        List<T> Get();
        T Get(string id);
        void Remove(string id);
        void Remove(T entityIn);
        void Update(string id, T entityIn);
        void DropCollection();
    }
}
