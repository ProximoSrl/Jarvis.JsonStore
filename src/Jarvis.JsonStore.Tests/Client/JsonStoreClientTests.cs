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
using Jarvis.JsonStore.Client;
using Jarvis.JsonStore.Host.Support;
using Jarvis.JsonStore.Tests.Client;

namespace Jarvis.JsonStore.Tests.Core.Projection
{
    [TestFixture]
    public class JsonStoreClientTests
    {
        JsonStoreClient sut;
        BootStrapper _bootstrapper;
        TestStoreConfiguration _config;

        IMongoDatabase db;
        MongoClient client;
        MongoUrl conn;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            conn = new MongoUrl(ConfigurationManager.AppSettings["testDatabase"]);
            client = new MongoClient(conn);
            db = client.GetDatabase(conn.DatabaseName);
            _config = new TestStoreConfiguration();
            sut = new JsonStoreClient("localhost", Int32.Parse(ConfigurationManager.AppSettings["testPort"]));
            _bootstrapper = new BootStrapper(_config);
            _bootstrapper.Start();
        }


        [SetUp]
        public void SetUp()
        {
            client.DropDatabase(conn.DatabaseName);
        }


        [Test]
        public void Verify_Client_basic_put()
        {
            sut.Put("test-client", "newId", "{}");
        }

        [Test]
        public void verify_client_search()
        {
            const string objectType = "test-client-search";
            sut.Put(objectType, "1", @"{""name"" : ""Alfred"" }");
            sut.Put(objectType, "2", @"{""name"" : ""Brandon"" }");
            var result = sut.Search(objectType, new JsonStore.Client.Model.SearchParameters()
            {
                JsonQuery = "{}",
                NumberOfRecords = 100,
                Sort = "",
                Start = 0,
            });
            Assert.That(result.Result, Has.Count.EqualTo(2));
        }

        [Test]
        public void verify_client_search_typed()
        {
            const string objectType = "test-client-search";
            sut.Put(objectType, "1", @"{""Name"" : ""Alfred"" }");
            sut.Put(objectType, "2", @"{""Name"" : ""Brandon"" }");
            var result = sut.Search<TestJson>(objectType, new JsonStore.Client.Model.SearchParameters()
            {
                JsonQuery = "{}",
                NumberOfRecords = 100,
                Sort = "Name",
                Start = 0,
            });
            Assert.That(result.Result, Has.Count.EqualTo(2));
            Assert.That(result.RecordCount, Is.EqualTo(2));
            foreach (var record in result.Result)
            {
                Console.WriteLine(record.Payload.Name);
            }
            Assert.That(result.Result[0].Payload.Name, Is.EqualTo("Alfred"));
        }

        private class TestJson
        {
            public String Name { get; set; }
        }
    }
}
