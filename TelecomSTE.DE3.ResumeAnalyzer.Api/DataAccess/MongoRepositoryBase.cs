using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Configuration;
using TelecomSTE.DE3.ResumeAnalyzer.Api.DataAccess.Interfaces;
using TelecomSTE.DE3.ResumeAnalyzer.Api.Model;

namespace TelecomSTE.DE3.ResumeAnalyzer.Api.DataAccess
{
    public abstract class MongoRepositoryBase<T> : IMongoRepositoryBase<T> where T : IMongoEntity
    {
        protected IMongoCollection<T> _collection;

        public MongoRepositoryBase(Settings settings, string collectionName)
        {
            var client = new MongoClient(settings.MongoConnectionString);
            var database = client.GetDatabase(settings.MongoDataBaseName);

            _collection = database.GetCollection<T>(collectionName);
        }

        public List<T> Get() =>
            _collection.Find(entity => true).ToList();

        public T Get(string id) =>
            _collection.Find<T>(entity => entity.Id == id).FirstOrDefault();

        public T Create(T entity)
        {
            _collection.InsertOne(entity);
            return entity;
        }

        public void Update(string id, T entityIn) =>
            _collection.ReplaceOne(entity => entity.Id == id, entityIn);

        public void Remove(T entityIn) =>
            _collection.DeleteOne(entity => entity.Id == entityIn.Id);

        public void Remove(string id) =>
            _collection.DeleteOne(entity => entity.Id == id);

    }
}
