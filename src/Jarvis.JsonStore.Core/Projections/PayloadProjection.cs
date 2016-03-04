using System;
using System.Collections.Generic;
using System.Threading;
using Castle.Core;
using Jarvis.JsonStore.Core.Storage;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Concurrent;
using Jarvis.JsonStore.Core.Support;

namespace Jarvis.JsonStore.Core.Projections
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
        PayloadProjectionCollectionManager _collectionManager;
        public PayloadProjection(IMongoDatabase database, PayloadProjectionCollectionManager collectionManager)
        {
            _database = database;
            _collectionManager = collectionManager;
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
            while (_stopped == false)
            {
                UpdateCollectionCount();
                foreach (var collectionInfo in _collections)
                {
                    var checkPoint = GetCheckpoint(collectionInfo.Key);
                    var events = collectionInfo.Value.Events
                         .Find(Builders<StoredObject>.Filter.Gt(o => o.Id, checkPoint))
                         .Sort(Builders<StoredObject>.Sort.Ascending(o => o.Id))
                         .Limit(10000)
                         .ToEnumerable();
                    Int64 lastCheckpoint = checkPoint;
                    foreach (var @event in events)
                    {
                        var projectionCollection = collectionInfo.Value.Projection;
                        ProcessEvent(@event, projectionCollection);
                        lastCheckpoint = @event.Id;
                    }

                    SetCheckpoint(collectionInfo.Key, lastCheckpoint);
                }
                Thread.Sleep(1000);
            }
        }

        private void ProcessEvent(
            StoredObject @event,
            IMongoCollection<BsonDocument> projectionCollection)
        {
            if (@event.OpType == OperationType.Put)
            {
                BsonDocument doc = @event.ToBsonDocument();

                projectionCollection.ReplaceOne(
                     Builders<BsonDocument>.Filter.Eq("_id", @event.ApplicationId.AsString),
                     doc,
                     new UpdateOptions { IsUpsert = true });

            }
            else if (@event.OpType == OperationType.Put)
            {
                projectionCollection.DeleteOne(
                     Builders<BsonDocument>.Filter.Eq("_id", @event.ApplicationId.AsString));
            }
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
                if (_collectionManager.IsEventCollection(collName))
                {
                    var type = _collectionManager.GetTypeNameFromEventCollectionName(collName);
                    if (!_collections.ContainsKey(collName))
                    {
                        var eventCollection = _collectionManager.GetEventsCollectionFromName(_database, type);
                        var projectionCollection = _collectionManager.GetProjectionCollectionFromName(_database, type);
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

    public class PayloadProjectionCollectionManager
    {
        private ConcurrentDictionary<String, IMongoCollection<StoredObject>> _eventsCollections =
            new ConcurrentDictionary<string, IMongoCollection<StoredObject>>();

        private ConcurrentDictionary<String, IMongoCollection<BsonDocument>> _projectionCollections =
            new ConcurrentDictionary<string, IMongoCollection<BsonDocument>>();

        public Boolean IsEventCollection(String collectionName)
        {
            return collectionName.StartsWith("events.");
        }

        public IMongoCollection<StoredObject> GetEventsCollectionFromName(IMongoDatabase db, String name)
        {
            if (!_eventsCollections.ContainsKey(name))
            {
                var collection = db.GetCollection<StoredObject>("events." + name);
                _eventsCollections.AddOrUpdate(name, collection, (k, c) => collection);
            }
            return _eventsCollections[name];
        }

        public IMongoCollection<BsonDocument> GetProjectionCollectionFromName(IMongoDatabase db, String name)
        {
            if (!_projectionCollections.ContainsKey(name))
            {
                var collection = db.GetCollection<BsonDocument>(name);
                _projectionCollections.AddOrUpdate(name, collection, (k, c) => collection);
            }
            return _projectionCollections[name];
        }

        internal String GetTypeNameFromEventCollectionName(string collName)
        {
            return collName.Substring("events.".Length);
        }
    }
}
