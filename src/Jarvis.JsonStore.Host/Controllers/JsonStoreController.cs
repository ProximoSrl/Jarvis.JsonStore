using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Castle.Core.Logging;
using Jarvis.JsonStore.Core.Storage;
using Jarvis.JsonStore.Core.Projections;
using Jarvis.JsonStore.Host.Models;
using System.Linq;
using System.Collections.Generic;
using Jarvis.JsonStore.Core.Model;

namespace Jarvis.JsonStore.Host.Controllers
{
    public class JsonStoreController : ApiController
    {
        private IObjectStore _store;
        private IPayloadFinder _finder;
        private IPayloadManager _manager;

        public ILogger Logger { get; set; }

        public JsonStoreController(
            IObjectStore store,
            IPayloadFinder finder,
            IPayloadManager manager)
        {
            _store = store;
            _finder = finder;
            _manager = manager;
        }

        [HttpGet]
        [Route("api/store/{type}/{id}")]
        public async Task<HttpResponseMessage> Get(String type, String id)
        {
            if (Logger.IsDebugEnabled) Logger.DebugFormat("Get {0} for type {1}", id, type);
            var result = await _store.GetById(type, id);
            if (result == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    string.Format("Id {0} not found for type {1}", id, type));
            }

            return Request.CreateResponse(
                HttpStatusCode.OK,
                result.ToClientStoredJsonObject()
            );
        }

        [HttpPut]
        [Route("api/store/{type}/{id}")]
        public async Task<HttpResponseMessage> Put(String type, String id)
        {
            if (Logger.IsDebugEnabled) Logger.DebugFormat("PUT {0} for type {1}", id, type);
            var payload = await Request.Content.ReadAsStringAsync();
            var stored = await _store.Store(type, id, payload);

            if (stored != null)
            {
                return Request.CreateResponse(
                   HttpStatusCode.OK,
                   stored.ToClientStoredJsonObject()
                );
            }
            return Request.CreateResponse(
                   HttpStatusCode.OK,
                   (String)null
                );
        }

        [HttpPost]
        [Route("api/store/{type}/_search")]
        public async Task<HttpResponseMessage> Find(
            String type,
            Int32 start = 0,
            Int32 num = 10,
            String sort = "")
        {

            var payload = await Request.Content.ReadAsStringAsync();

            if (Logger.IsDebugEnabled) Logger.DebugFormat("find {0} for type {1}", payload, type);
            Boolean sortAscending = true;
            if (!String.IsNullOrEmpty(sort))
            {
                var splitted = sort.Split(' ');
                sort = splitted[0];
                if (splitted.Length > 1 && splitted[1].Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    sortAscending = false;
                }
            }
            var retValue = await _finder.Search(type, payload, sort, sortAscending, start, num);


            return Request.CreateResponse(
                HttpStatusCode.OK,
                retValue
            );
        }

        [HttpPut]
        [Route("api/store/{type}/indexes")]
        public async Task<HttpResponseMessage> EnsureIndex(
            String type,
            CreateIndexModel createIndexModel)
        {
           
            List<String> errors = new List<string>();
            if (createIndexModel == null)
            {
                errors.Add("Parameter wrong!");
            }
            else
            {
                if (String.IsNullOrEmpty(createIndexModel.IndexName))
                {
                    errors.Add("Index name should be specified");
                }
                if (createIndexModel.Properties == null || createIndexModel.Properties.Count == 0)
                {
                    errors.Add("You need at least to choose one property");
                }
            }
            if (errors.Count > 0)
            {
                String errorMessage = String.Join(",", errors);
                Logger.WarnFormat("CreateIndex input parameter error: {0}", errorMessage);
                return Request.CreateResponse(
                       HttpStatusCode.InternalServerError,
                       new { Errors = String.Join(",", errors), Success = false });
            }
            

            if (Logger.IsInfoEnabled) Logger.InfoFormat("Creating index {0} for type {1}", createIndexModel.IndexName, type);

            var result = await _manager.EnsureIndex(
                type,
                createIndexModel.IndexName,
                createIndexModel.Properties.Select(p => new IndexPropertyDefinition(p.PropertyName, p.Descending)));

            if (!result)
                return Request.CreateResponse(
                   HttpStatusCode.InternalServerError,
                   new { Errors = "Generic Error", Success = false });

            return Request.CreateResponse(
                HttpStatusCode.OK,
                new { Success = true }
            );
        }

        [HttpDelete]
        [Route("api/store/{type}/indexes/{name}")]
        public async Task<HttpResponseMessage> DeleteIndex(
            String type,
            String name)
        {
            List<String> errors = new List<string>();
            if (String.IsNullOrEmpty(name))
            {
                errors.Add("Index name should be specified");
            }

            if (errors.Count > 0)
                return Request.CreateResponse(
                   HttpStatusCode.InternalServerError,
                   new { Errors = String.Join(",", errors), Success = false });

            if (Logger.IsInfoEnabled) Logger.InfoFormat("Removing index {0} for type {1}", name, type);

            var result = await _manager.DeleteIndex(type, name);
            if (!result)
                return Request.CreateResponse(
                   HttpStatusCode.InternalServerError,
                   new { Errors = "Generic Error", Success = false });

            return Request.CreateResponse(
                HttpStatusCode.OK,
                new { Success = true }
            );
        }

    }
}
