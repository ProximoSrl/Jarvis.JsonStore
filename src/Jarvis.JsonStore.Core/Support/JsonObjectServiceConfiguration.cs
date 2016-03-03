using Jarvis.ConfigurationService.Client;
using System;

namespace Jarvis.JsonStore.Core.Support
{
    public class JsonObjectServiceConfiguration
    {
        public String MongoDbConnection { get; protected set; }

        public String Address { get; set; }
    }

    public class StandardJsonObjectServiceConfiguration : JsonObjectServiceConfiguration
    {
        public StandardJsonObjectServiceConfiguration()
        {
            Address = ConfigurationServiceClient.Instance.GetSetting("webapp.port", "http://+:40000");
            
            MongoDbConnection = ConfigurationServiceClient.Instance.GetSetting("connection-string", "mongodb://localhost:27017/jarvis-jsonStore");
        }
    }
}
