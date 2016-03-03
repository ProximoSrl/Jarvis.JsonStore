using System;

namespace Jarvis.JsonStore.Core.Support
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
