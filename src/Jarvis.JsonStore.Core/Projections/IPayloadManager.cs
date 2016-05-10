using Castle.Core.Logging;
using Jarvis.JsonStore.Core.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonStore.Core.Projections
{
   public interface IPayloadManager
    {
        /// <summary>
        /// Ensure an index is created on the projection of a given type, needed to 
        /// speedup searches.
        /// </summary>
        /// <param name="type">Type of the object</param>
        /// <param name="indexName">Name of the index </param>
        Task<Boolean> EnsureIndex(string type, string indexName, IEnumerable<IndexPropertyDefinition> properties);

        /// <summary>
        /// Delete an index for the projection
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Boolean> DeleteIndex(string type, string name);
    }

    public class MongoPayloadManager : IPayloadManager {

        PayloadProjectionCollectionManager _collectionManager;
        IMongoDatabase _db;
        ILogger _logger;

        public MongoPayloadManager(
            IMongoDatabase db,
            PayloadProjectionCollectionManager collectionManager,
            ILogger logger)
        {
            _db = db;
            _collectionManager = collectionManager;
            _logger = logger;
        }

        public async Task<bool> DeleteIndex(string type, string name)
        {
            var collection = _collectionManager.GetProjectionCollectionFromName(_db, type);
            await collection.Indexes.DropOneAsync(name);
            return true;
        }

        public async Task<Boolean> EnsureIndex(string type, string indexName, IEnumerable<IndexPropertyDefinition> properties)
        {
            try
            {
                await InnerCreateIndex(type, indexName, properties);
                return true;
            }
            catch (MongoCommandException cmd)
            {
                _logger.ErrorFormat(cmd, "Error creating index {0} [{1}]", indexName, cmd.Message);
                var collection = _collectionManager.GetProjectionCollectionFromName(_db, type);
                collection.Indexes.DropOne(indexName);
            }

            await InnerCreateIndex(type, indexName, properties);
            return true;
        }

        private async Task InnerCreateIndex(string type, string indexName, IEnumerable<IndexPropertyDefinition> properties)
        {
            var collection = _collectionManager.GetProjectionCollectionFromName(_db, type);

            IndexKeysDefinition<BsonDocument> definition = null;
            var builder = Builders<BsonDocument>.IndexKeys;
            foreach (var property in properties)
            {
                if (definition == null)
                {
                    if (property.Descending)
                        definition = Builders<BsonDocument>.IndexKeys.Descending(property.PropertyName);
                    else
                        definition = Builders<BsonDocument>.IndexKeys.Ascending(property.PropertyName);
                }
                else
                {
                    if (property.Descending)
                        definition = definition.Descending(property.PropertyName);
                    else
                        definition = definition.Ascending(property.PropertyName);
                }

            }
            await collection.Indexes.CreateOneAsync(definition, new CreateIndexOptions() { Name = indexName });
        }
    }
}
