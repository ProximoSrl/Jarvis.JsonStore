using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Jarvis.JsonStore.Core.Storage;

namespace Jarvis.JsonStore.Host.Controllers
{
    public class JsonObjectController : ApiController
    {
        private IObjectStore _store;

        public JsonObjectController(
            IObjectStore store)
        {
            _store = store;
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
                result
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
                stored
            );
        }

    }
}
