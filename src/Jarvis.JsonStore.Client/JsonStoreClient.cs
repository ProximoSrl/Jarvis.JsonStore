using Jarvis.JsonStore.Client.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonStore.Client
{
    public class JsonStoreClient
    {
        private string _serviceUri;

        public JsonStoreClient(String serviceAddress, Int32 servicePort)
        {
            _serviceUri = String.Format("http://{0}:{1}", serviceAddress, servicePort);
        }

        private Uri GeneratePutAddress(String type, String id)
        {
            return new Uri(string.Format("{0}/api/store/{1}/{2}", _serviceUri, type, id));
        }

        private Uri GenerateGetAddress(String type, String id)
        {
            return new Uri(string.Format("{0}/api/store/{1}/{2}", _serviceUri, type, id));
        }

        public StoredJsonObject Put(String type, String id, String jsonPayload)
        {
            using (var client = new System.Net.WebClient())
            {
                var resourceUri = GenerateGetAddress(type, id);
                client.Headers.Add("Content-Type", "application/json");
                var result =  client.UploadData(
                    resourceUri, 
                    "PUT",
                    Encoding.UTF8.GetBytes(jsonPayload));
                if (result == null) return null;

                var jsonResponse = Encoding.UTF8.GetString(result);
                return JsonConvert.DeserializeObject< StoredJsonObject>(jsonResponse);
            }
        }

        public StoredJsonObject Get(String type, String id)
        {
            using (var client = new WebClient())
            {
                var resourceUri = GenerateGetAddress(type, id);
                var result = client.DownloadString(resourceUri);
                if (result == null) return null;

                return JsonConvert.DeserializeObject<StoredJsonObject>(result);
            }
        }
    }
}
