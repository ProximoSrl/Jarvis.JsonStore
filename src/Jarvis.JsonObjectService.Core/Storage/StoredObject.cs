using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonObjectService.Core.Storage
{
    public class StoredObject
    {
        public Int64 Id { get; set; }

        public String ApplicationId { get; set; }

        public String Hash { get; set; }

        public DateTime TimeStamp { get; set; }

        public String JsonPayload { get; set; }
    }
}
