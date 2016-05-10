using Jarvis.JsonStore.Core.Projections;
using Jarvis.JsonStore.Core.Storage;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;
using Jarvis.JsonStore.Core.Model;
using Castle.Core.Logging;

namespace Jarvis.JsonStore.Tests.Core.Projection
{
    [TestFixture]
    public class PayloadManagerTests
    {
        MongoObjectStore objectStore;
        MongoPayloadManager sut;
        IMongoDatabase db;
        MongoClient client;
        MongoUrl conn;
        PayloadProjectionCollectionManager connManager;
        PayloadProjection projection;
        private string _eventsCollectionName = "events.test";
        private string _projectionCollectionName = "test";

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            conn = new MongoUrl(ConfigurationManager.AppSettings["testDatabase"]);
            client = new MongoClient(conn);
            db = client.GetDatabase(conn.DatabaseName);
            objectStore = new MongoObjectStore(db);
            connManager = new PayloadProjectionCollectionManager();
            projection = new PayloadProjection(db, connManager);
        }


        [SetUp]
        public void SetUp()
        {
            client.DropDatabase(conn.DatabaseName);
            CreateSut();
        }

        private void CreateSut()
        {
            sut = new MongoPayloadManager(db, connManager, NullLogger.Instance);
        }


        [Test]
        public async void basic_indexing()
        {
            String jsonObject1 = @"{""test"" : ""value1""}";
            String jsonObject2 = @"{""test"" : ""value2""}";
            await objectStore.Store("test", "1", jsonObject1);
            await objectStore.Store("test", "2", jsonObject2);

            ProcessEvents();

            var result = await sut.EnsureIndex("test", "index1", new[] { new IndexPropertyDefinition("test", false) });
            Assert.That(result);

            var indexes = connManager.GetProjectionCollectionFromName(db, "test").Indexes.List().ToList();
            var indexNames = indexes.Select(d => d["name"].AsString).ToList();
            Assert.That(indexNames.Contains("index1"));
        }

        [Test]
        public async void change_index_property()
        {
            String jsonObject1 = @"{""test1"" : ""value1""}";
            String jsonObject2 = @"{""test2"" : ""value2""}";
            await objectStore.Store("test", "1", jsonObject1);
            await objectStore.Store("test", "2", jsonObject2);

            ProcessEvents();

            var result = await sut.EnsureIndex("test", "index1", new[] { new IndexPropertyDefinition("test1", false) });
            Assert.That(result);
            result = await sut.EnsureIndex("test", "index1", new[] { new IndexPropertyDefinition("test2", false) });
            Assert.That(result);

            var indexes = connManager.GetProjectionCollectionFromName(db, "test").Indexes.List().ToList();
            var indexNames = indexes.Select(d => d["name"].AsString).ToList();
            Assert.That(indexNames.Contains("index1"));

        }

        private void ProcessEvents()
        {
            var coll = db.GetCollection<BsonDocument>(_projectionCollectionName);
            var eventsColl = db.GetCollection<StoredObject>(_eventsCollectionName);
            foreach (var evt in eventsColl.Find(new BsonDocument()).ToEnumerable())
            {
                projection.CallMethod("ProcessEvent", 
                    new Type[] { typeof(StoredObject), typeof(IMongoCollection<BsonDocument>) },
                    (StoredObject) evt, (IMongoCollection <BsonDocument>) coll);
            }
        }
    }
}
