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

        public JsonStoreClient(String serverAddress, Int32 servicePort)
        {
            _serviceUri = String.Format("http://{0}:{1}", serverAddress, servicePort);
        }

        public JsonStoreClient(String fullServiceAddress)
        {
            _serviceUri = fullServiceAddress;
        }

        private Uri GeneratePutAddress(String type, String id)
        {
            return new Uri(string.Format("{0}/api/store/{1}/{2}", _serviceUri, type, id));
        }

        private Uri GenerateGetAddress(String type, String id)
        {
            return new Uri(string.Format("{0}/api/store/{1}/{2}", _serviceUri, type, id));
        }

        private Uri GenerateSearchAddress(String type, SearchParameters parameters)
        {
            return new Uri(string.Format("{0}/api/store/{1}/_search?start={2}&num={3}&sort={4}",
                _serviceUri,
                type,
                parameters.Start,
                parameters.NumberOfRecords,
                parameters.Sort));
        }

        public StoredJsonObject Put(String type, String id, String jsonPayload)
        {
            using (var client = new System.Net.WebClient())
            {
                var resourceUri = GenerateGetAddress(type, id);
                client.Headers.Add("Content-Type", "application/json");
                var result = client.UploadData(
                    resourceUri,
                    "PUT",
                    Encoding.UTF8.GetBytes(jsonPayload));
                if (result == null) return null;

                var jsonResponse = Encoding.UTF8.GetString(result);
                return JsonConvert.DeserializeObject<StoredJsonObject>(jsonResponse);
            }
        }

        public async Task<StoredJsonObject> PutAsync(String type, String id, String jsonPayload)
        {
            using (var client = new System.Net.WebClient())
            {
                var resourceUri = GenerateGetAddress(type, id);
                client.Headers.Add("Content-Type", "application/json");
                var result = await client.UploadDataTaskAsync(
                    resourceUri,
                    "PUT",
                    Encoding.UTF8.GetBytes(jsonPayload));
                if (result == null) return null;

                var jsonResponse = Encoding.UTF8.GetString(result);
                return JsonConvert.DeserializeObject<StoredJsonObject>(jsonResponse);
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

        public async Task<StoredJsonObject> GetAsync(String type, String id)
        {
            using (var client = new WebClient())
            {
                var resourceUri = GenerateGetAddress(type, id);
                var result = await client.DownloadStringTaskAsync(resourceUri);
                if (result == null) return null;

                return JsonConvert.DeserializeObject<StoredJsonObject>(result);
            }
        }

        public StoredJsonObject<T> Get<T>(String type, String id) where T : class
        {
            var result = Get(type, id);
            return new StoredJsonObject<T>(result);
        }

        public async Task<StoredJsonObject<T>> GetAsync<T>(String type, String id) where T : class
        {
            var result = await GetAsync(type, id);
          
            return new StoredJsonObject<T>(result);
        }

        public List<StoredJsonObject> Search(String type, SearchParameters searchParameters)
        {
            using (var client = new WebClient())
            {
                var resourceUri = GenerateSearchAddress(type, searchParameters);
                client.Headers.Add("Content-Type", "application/json");
                var payload = Encoding.UTF8.GetBytes(searchParameters.JsonQuery);
                var result = client.UploadData(resourceUri, payload);
                if (result == null) return new List<StoredJsonObject>();

                var stringResult = Encoding.UTF8.GetString(result);

                return JsonConvert.DeserializeObject<List<StoredJsonObject>>(stringResult);
            }
        }

        public List<StoredJsonObject<T>> Search<T>(String type, SearchParameters searchParameters)
        {
            var result = Search(type, searchParameters);
            return result.Select(o => new StoredJsonObject<T>(o)).ToList();
        }

        public async Task<List<StoredJsonObject>> SearchAsync(String type, SearchParameters searchParameters)
        {
            using (var client = new WebClient())
            {
                var resourceUri = GenerateSearchAddress(type, searchParameters);
                client.Headers.Add("Content-Type", "application/json");
                var payload = Encoding.UTF8.GetBytes(searchParameters.JsonQuery);
                var result = await client.UploadDataTaskAsync(resourceUri, payload);
                if (result == null) return new List<StoredJsonObject>();

                var stringResult = Encoding.UTF8.GetString(result);

                return JsonConvert.DeserializeObject<List<StoredJsonObject>>(stringResult);
            }
        }

        public async Task<List<StoredJsonObject<T>>> SearchAsync<T>(String type, SearchParameters searchParameters)
        {
            var result = await SearchAsync(type, searchParameters);
            return result.Select(o => new StoredJsonObject<T>(o)).ToList();
        }

    }
}

