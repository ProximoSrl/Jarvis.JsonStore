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

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _config = new TestStoreConfiguration();
            sut = new JsonStoreClient("localhost", Int32.Parse(ConfigurationManager.AppSettings["testPort"]));
            _bootstrapper = new BootStrapper(_config);
            _bootstrapper.Start();
        }


        [SetUp]
        public void SetUp()
        {
            
        }


        [Test]
        public void Verify_Client_basic_put()
        {
            sut.Put("test-client", "newId", "{}");
        }


        
    }
}
