using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Json.ObjectService.Host.Controllers
{
    public class JsonObjectController : ApiController
    {
        [HttpGet]
        [Route("api/{collectionid}/{id}")]
        public IHttpActionResult Get(String collectionid, String id)
        {
            return null;
        }
    
    }
}
