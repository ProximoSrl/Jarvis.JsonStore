using Jarvis.JsonStore.Client.Model;
using Jarvis.JsonStore.Core.Storage;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonStore.Core.Support
{
    public static class BsonDocumentUtils
    {
        public static StoredJsonObject ConvertToStoredJsonObject(this BsonDocument doc)
        {
            var retValue = new StoredJsonObject()
            {
                ApplicationId = doc["_id"].AsString,
                Hash = doc["_jsonStore"]["hash"].AsString,
                Version = doc["_jsonStore"]["version"].AsInt32,
            };

            BsonDocument plain = new BsonDocument();
            foreach (var property in doc)
            {
                if (!IsSystemProp(property.Name))
                {
                    plain.Add(property);
                }
            }
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            retValue.JsonPayload = plain.ToJson<MongoDB.Bson.BsonDocument>(jsonWriterSettings);

            return retValue;
        }

        private static bool IsSystemProp(string name)
        {
            return name.StartsWith("_jsonStore") || name == "_id";
         }

        public static  BsonDocument ToBsonDocument(this StoredObject obj)
        {
            var doc = BsonDocument.Parse(obj.JsonPayload);
            doc["_id"] = obj.ApplicationId.AsString;
            var jsonStoreData = new BsonDocument();
            jsonStoreData["version"] = obj.Version;
            jsonStoreData["hash"] = obj.Hash;
            jsonStoreData["id"] = obj.Id;

            doc["_jsonStore"] = jsonStoreData;

            return doc;
        }
    }
}
