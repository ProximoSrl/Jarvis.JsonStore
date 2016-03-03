using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Jarvis.JsonStore.Core.Support;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Jarvis.JsonStore.Core.Storage
{
    public class MongoObjectStore : IObjectStore
    {
        IMongoDatabase _database;

        public ILogger Logger { get; set; }

        private class ConnectionInfo
        {
            public IMongoCollection<StoredObject> Collection { get; set; }

            public Int64 CurrentId;

            public Int64 GetNextId()
            {
                return Interlocked.Increment(ref CurrentId);
            }
        }

        private ConcurrentDictionary<String, ConnectionInfo>
            collections = new ConcurrentDictionary<string, ConnectionInfo>();

        private ConnectionInfo GetCollectionForType(String type)
        {
            if (!collections.ContainsKey(type))
            {
                var collection = _database.GetCollection<StoredObject>("events." + type);
                var last = collection
                    .Find(new BsonDocument())
                    .Sort(Builders<StoredObject>.Sort.Descending(o => o.Id))
                    .FirstOrDefault();
                Int64 currentId = 0;
                if (last != null)
                    currentId = last.Id;

                ConnectionInfo info = new ConnectionInfo()
                {
                    Collection = collection,
                    CurrentId = currentId
                };
                collections.AddOrUpdate(type, info, (c1, c2) => info);
            }
            return collections[type];
        }

        public MongoObjectStore(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<StoredObject> GetById(String type, String id)
        {
            var collectionInfo = GetCollectionForType(type);
            var obj = await collectionInfo.Collection
                .Find(Builders<StoredObject>.Filter.Eq("ApplicationId", id))
                .Sort(Builders<StoredObject>.Sort.Descending(o => o.Id))
                .FirstOrDefaultAsync();
            return obj;
        }

        public async Task<StoredObject> Store(String type, String id, String jsonObject)
        {
            var collectionInfo = GetCollectionForType(type);
            var obj = await GetById(type, id);
            var hash = HashUtils.GetHashOfSerializedJson(jsonObject);
            var version = 1;
            if (obj != null)
            {
                //check hash
                if (obj.Hash == hash)
                    return null;

                version = obj.Version + 1;
            }

            StoredObject so = new StoredObject()
            {
                Id = collectionInfo.GetNextId(),
                ApplicationId = id,
                JsonPayload = jsonObject,
                TimeStamp = DateTime.UtcNow,
                Hash = hash,
                Version = version,
                OpType = OperationType.Put
            };
            collectionInfo.Collection.InsertOne(so);
            return so;
        }

        public async Task<StoredObject> DeleteById(String type, String id)
        {
            var collectionInfo = GetCollectionForType(type);
            var obj = await GetById(type, id);
            if (obj == null)
            {
                return null;
            }

            StoredObject so = new StoredObject()
            {
                Id = collectionInfo.GetNextId(),
                ApplicationId = id,
                JsonPayload = null,
                TimeStamp = DateTime.UtcNow,
                Hash = obj.Hash,
                Deleted = true,
                OpType = OperationType.Delete
            };
            collectionInfo.Collection.InsertOne(so);
            return so;


        }
    }
}
