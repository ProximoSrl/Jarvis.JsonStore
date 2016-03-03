using Jarvis.JsonStore.Client.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jarvis.JsonStore.Core.Support;

namespace Jarvis.JsonStore.Core.Projections
{
    public interface IPayloadFinder
    {
        /// <summary>
        /// Issue a search to the underling system and return data to the client.
        /// </summary>
        /// <param name="type">Type to query</param>
        /// <param name="jsonQuery">Query with mongo syntax</param>
        /// <param name="sortProperty">If different from null sorts for the specifed property</param>
        /// <param name="sortAscending">Specify if sort is ascending (true) or descending (false)</param>
        /// <param name="startFrom">Record to start from</param>
        /// <param name="maxRecord">Max number of record to return.</param>
        /// <returns></returns>
        Task<IList<StoredJsonObject>> Search(
            String type, 
            String jsonQuery,
            String sortProperty,
            Boolean sortAscending, 
            Int32 startFrom, 
            Int32 maxRecord);

    }

    public class MongoPayloadFinder : IPayloadFinder
    {
        PayloadProjectionCollectionManager _collectionManager;
        IMongoDatabase _db;

        public MongoPayloadFinder(
            IMongoDatabase db,
            PayloadProjectionCollectionManager collectionManager)
        {
            _db = db;
            _collectionManager = collectionManager;
        }

        public async Task<IList<StoredJsonObject>> Search(
            string type,
            string jsonQuery,
            string sortProperty,
            Boolean sortAscending,
            int startFrom, 
            int maxRecord)
        {
            if (String.IsNullOrEmpty(jsonQuery))
                jsonQuery = "{}";

            var query = _collectionManager.GetProjectionCollectionFromName(_db, type)
                 .Find(BsonDocument.Parse(jsonQuery));
            if (!String.IsNullOrEmpty(sortProperty))
            {
                if (sortAscending)
                {
                    query = query.Sort(Builders<BsonDocument>.Sort.Ascending(sortProperty));
                }
                else
                {
                    query = query.Sort(Builders<BsonDocument>.Sort.Descending(sortProperty));
                }
            }
            query = query
                .Skip(startFrom)
                .Limit(maxRecord);

            var result = await query.ToListAsync();
            var parsedResult = result.Select(d => d.ConvertToStoredJsonObject()).ToList();
            return parsedResult;
        }
    }
}
