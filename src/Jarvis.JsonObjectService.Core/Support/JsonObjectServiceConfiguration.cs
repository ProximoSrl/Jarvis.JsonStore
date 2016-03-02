using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonObjectService.Core.Support
{
    public class JsonObjectServiceConfiguration
    {
        public String MongoDbConnection { get; protected set; }

    }

    public class LocalJsonObjectServiceConfiguration : JsonObjectServiceConfiguration
    {
        public LocalJsonObjectServiceConfiguration()
        {
            MongoDbConnection = "mongodb://localhost:27017/jarvis-jsonStore";
        }
    }
}
