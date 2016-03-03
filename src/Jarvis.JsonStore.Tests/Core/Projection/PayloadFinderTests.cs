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

namespace Jarvis.JsonStore.Tests.Core.Projection
{
    [TestFixture]
    public class PayloadFinderTests
    {
        MongoObjectStore objectStore;
        MongoPayloadFinder sut;
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
            sut = new MongoPayloadFinder(db, connManager);
        }


        [Test]
        public async void verify_query_equality()
        {
            String jsonObject1 = @"{ ""prop"" : ""1""}";
            String jsonObject2 = @"{ ""prop"" : ""2""}";
            await objectStore.Store("test", "1", jsonObject1);
            await objectStore.Store("test", "2", jsonObject2);

            ProcessEvents();
          
            var query = @"{""prop"" : ""2""}";
            var result = await sut.Search("test", query, "", false, 0, 10);

             Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].JsonPayload, Is.Not.Null);

        }

        [Test]
        public async void verify_null_query_()
        {
            String jsonObject1 = @"{ ""prop"" : ""1""}";
            String jsonObject2 = @"{ ""prop"" : ""2""}";
            await objectStore.Store("test", "1", jsonObject1);
            await objectStore.Store("test", "2", jsonObject2);

            ProcessEvents();

            var query = @"";
            var result = await sut.Search("test", query, "", false, 0, 10);

            Assert.That(result, Has.Count.EqualTo(2));
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
