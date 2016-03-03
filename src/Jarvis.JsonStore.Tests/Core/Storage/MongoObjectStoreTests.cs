using System;
using System.Configuration;
using System.Linq;
using Jarvis.JsonStore.Core.Storage;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace Jarvis.JsonStore.Tests.Core.Storage
{
    [TestFixture]
    public class MongoObjectStoreTests 
    {
        MongoObjectStore sut;
        IMongoDatabase db;
        MongoClient client;
        MongoUrl conn;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            conn = new MongoUrl(ConfigurationManager.AppSettings["testDatabase"]);
            client = new MongoClient(conn);
            
            db = client.GetDatabase(conn.DatabaseName);
        }


        [SetUp]
        public void SetUp()
        {
            client.DropDatabase(conn.DatabaseName);
            CreateSut();
        }

        private void CreateSut()
        {
            sut = new MongoObjectStore(db);
        }

        [Test]
        public async void BasicSave()
        {
            String jsonObject = "{}";
            var saved = await sut.Store("test", "1", jsonObject);
            Assert.That(saved, Is.Not.Null);
            Assert.That(saved.Id, Is.EqualTo(1L));
        }

        [Test]
        public async void basic_load()
        {
            String jsonObject = "{}";
            String jsonObject2 = @"{""test"" : ""value""}";
            await sut.Store("test", "1", jsonObject);
            await sut.Store("test", "1", jsonObject2);

            var loaded = await sut.GetById("test", "1");
            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.Id, Is.EqualTo(2L));
            Assert.That(loaded.JsonPayload, Is.EqualTo(jsonObject2));
        }

        [Test]
        public async void verify_save_increment_counter()
        {
            String jsonObject = "{}";
            var saved1 = await sut.Store("test", "1", jsonObject);
            CreateSut();
            var saved2 = await sut.Store("test", "2", jsonObject);

            Assert.That(saved2, Is.Not.Null);
            Assert.That(saved2.Id, Is.EqualTo(2L));
        }

        [Test]
        public async void verify_save_maintain_history()
        {
            String jsonObject = @"{ ""prop"" : ""test""}";
            var saved1 = await sut.Store("test", "1", "{}");
            var saved2 = await sut.Store("test", "1", jsonObject);
            var coll = db.GetCollection<StoredObject>("test");
            var allObj = coll.Find(new BsonDocument()).ToList();
            Assert.That(allObj.Count, Is.EqualTo(2));
        }

        [Test]
        public async void verify_save_avoid_duplicate()
        {
            String jsonObject = @"{ ""prop"" : ""test""}";
            var saved1 = await sut.Store("test", "1", jsonObject);
            var saved2 = await sut.Store("test", "1", jsonObject);
            Assert.That(saved2, Is.Null);
            var coll = db.GetCollection<StoredObject>("test");
            var allObj = coll.Find(new BsonDocument()).ToList();
            Assert.That(allObj.Count, Is.EqualTo(1));
            Assert.That(allObj.Single().Id, Is.EqualTo(1L));
        }

        [Test]
        public async void verify_save_avoid_duplicate_inverted_property()
        {
            String jsonObject1 = @"{ ""prop1"" : ""test1"", ""prop2"" : ""test2""}";
            String jsonObject2 = @"{ ""prop2"" : ""test2"", ""prop1"" : ""test1""}";
            var saved1 = await sut.Store("test", "1", jsonObject1);
            var saved2 = await sut.Store("test", "1", jsonObject2);
            Assert.That(saved2, Is.Null);
            var coll = db.GetCollection<StoredObject>("test");
            var allObj = coll.Find(new BsonDocument()).ToList();
            Assert.That(allObj.Count, Is.EqualTo(1));
            Assert.That(allObj.Single().Id, Is.EqualTo(1L));
        }

        [Test]
        public async void verify_delete_basic()
        {
            String jsonObject = @"{ ""prop"" : ""test""}";
            var saved1 = await sut.Store("test", "1", jsonObject);
            var saved2 = await sut.DeleteById("test", "1");
            Assert.That(saved2, Is.Not.Null);
            Assert.That(saved2.Deleted, Is.EqualTo(true));
            Assert.That(saved2.JsonPayload, Is.EqualTo(null));
            var coll = db.GetCollection<StoredObject>("test");
            var allObj = coll.Find(new BsonDocument()).ToList();
            Assert.That(allObj.Count, Is.EqualTo(2));
 
        }
    }
}
