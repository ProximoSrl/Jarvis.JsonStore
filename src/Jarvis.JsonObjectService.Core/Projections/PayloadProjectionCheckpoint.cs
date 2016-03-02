using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonObjectService.Core.Projections
{
    class PayloadProjectionCheckpoint
    {
        /// <summary>
        /// the id is the type of the json object processed.
        /// </summary>
        public String Id { get; set; }

        public Int64 LastProcessed { get; set; }
    }
}
