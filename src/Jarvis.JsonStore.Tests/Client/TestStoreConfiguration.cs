using Jarvis.JsonStore.Core.Support;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonStore.Tests.Client
{
    class TestStoreConfiguration : JsonObjectServiceConfiguration
    {
        public TestStoreConfiguration()
        {
            this.Address = ConfigurationManager.AppSettings["testaddress"];
            this.MongoDbConnection = ConfigurationManager.AppSettings["testDatabase"];
        }
    }
}
