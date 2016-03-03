using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Castle.Core.Logging;
using Jarvis.JsonStore.Core.Storage;
using Jarvis.JsonStore.Core.Projections;

namespace Jarvis.JsonStore.Host.Controllers
{
    public class JsonStoreController : ApiController
    {
        private IObjectStore _store;
        private IPayloadFinder _finder;

        public ILogger Logger { get; set; }

        public JsonStoreController(
            IObjectStore store,
            IPayloadFinder finder)
        {
            _store = store;
            _finder = finder;
        }

        [HttpGet]
        [Route("api/store/{type}/{id}")]
        public async Task<HttpResponseMessage> Get(String type, String id)
        {
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
            var payload = await Request.Content.ReadAsStringAsync();
            var stored = await _store.Store(type, id, payload);

            return Request.CreateResponse(
                HttpStatusCode.OK,
                stored.ToClientStoredJsonObject()
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
    }
}
