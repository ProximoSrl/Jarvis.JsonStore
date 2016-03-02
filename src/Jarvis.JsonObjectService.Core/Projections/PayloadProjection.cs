using Castle.Core;
using Jarvis.JsonObjectService.Core.Storage;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jarvis.JsonObjectService.Core.Projections
{
    /// <summary>
    /// 
    /// </summary>
    public class PayloadProjection : IStartable
    {
        IMongoDatabase _database;
        Thread _pollerThread;
        Boolean _stopped = false;
        IMongoCollection<PayloadProjectionCheckpoint> _checkpoints;

        public PayloadProjection(IMongoDatabase database)
        {
            _database = database;
            _checkpoints = _database.GetCollection<PayloadProjectionCheckpoint>("payload.checkpoints");
        }

        private Dictionary<String, Int64> _inMemoryCheckpoint = new Dictionary<string, long>();

        public void Start()
        {
            var checkpoints = _checkpoints.Find(new BsonDocument()).ToList();
            foreach (var checkpoint in checkpoints)
            {
                _inMemoryCheckpoint.Add(checkpoint.Id, checkpoint.LastProcessed);
            }
            _stopped = false;
            _pollerThread = new Thread(Process);
            _pollerThread.Start();
        }

        public void Stop()
        {
            _stopped = true;
        }

        private Boolean _isPolling = false;

        public void Process(Object State)
        {
            if (_isPolling) return; //TODO:Replace with interlocked exchange technique
            _isPolling = true;
            try
            {
                while (_stopped == false)
                {
                    UpdateCollectionCount();
                    foreach (var collection in _collections)
                    {
                        var checkPoint = GetCheckpoint(collection.Key);
                        var events = collection.Value.Events
                             .Find(Builders<StoredObject>.Filter.Gt(o => o.Id, checkPoint))
                             .Sort(Builders<StoredObject>.Sort.Descending(o => o.Id))
                             .Limit(1000)
                             .ToEnumerable();
                        Int64 lastCheckpoint = checkPoint;
                        foreach (var @event in events)
                        {
                            if (@event.OpType == OperationType.Put)
                            {
                                BsonDocument doc = BsonDocument.Parse(@event.JsonPayload);
                                doc["_id"] = @event.ApplicationId;
                                collection.Value.Projection.ReplaceOneAsync(
                                     Builders<BsonDocument>.Filter.Eq("_id", @event.ApplicationId),
                                     doc,
                                     new UpdateOptions { IsUpsert = true });

                            }
                            else if (@event.OpType == OperationType.Put)
                            {
                                collection.Value.Projection.DeleteOne(
                                     Builders<BsonDocument>.Filter.Eq("_id", @event.ApplicationId));
                            }
                            lastCheckpoint = @event.Id;
                        }

                        SetCheckpoint(collection.Key, lastCheckpoint);

                    }
                }
            }
            finally
            {
                _isPolling = false;
            }
            Thread.Sleep(2000);
        }

        private void SetCheckpoint(string key, long lastCheckpoint)
        {
            var actual = _inMemoryCheckpoint[key];
            if (actual < lastCheckpoint)
            {
                _inMemoryCheckpoint[key] = lastCheckpoint;

                var checkpoint = new PayloadProjectionCheckpoint()
                {
                    Id = key,
                    LastProcessed = lastCheckpoint
                };

                _checkpoints.ReplaceOneAsync(
                    Builders<PayloadProjectionCheckpoint>.Filter.Eq(o => o.Id, key),
                    checkpoint,
                    new UpdateOptions { IsUpsert = true });
            }
        }


        private Int64 GetCheckpoint(string key)
        {
            if (!_inMemoryCheckpoint.ContainsKey(key))
            {
                _inMemoryCheckpoint.Add(key, 0);
            }
            return _inMemoryCheckpoint[key];
        }

        private Dictionary<String, CollectionInfo> _collections =
            new Dictionary<String, CollectionInfo>();

        private void UpdateCollectionCount()
        {
            var collections = _database.ListCollections().ToList();
            foreach (var collection in collections)
            {
                var collName = collection["name"].AsString;
                if (collName.StartsWith("events."))
                {
                    if (!_collections.ContainsKey(collName))
                    {
                        var eventCollection = _database.GetCollection<StoredObject>(collName);
                        var projectionCollection = _database.GetCollection<BsonDocument>(collName.Substring("events.".Length));
                        _collections.Add(collName, new CollectionInfo()
                        {
                            Events = eventCollection,
                            Projection = projectionCollection,
                        });
                    }
                }
            }
        }

        private class CollectionInfo
        {
            public IMongoCollection<StoredObject> Events { get; set; }

            public IMongoCollection<BsonDocument> Projection { get; set; }
        }
    }
}
