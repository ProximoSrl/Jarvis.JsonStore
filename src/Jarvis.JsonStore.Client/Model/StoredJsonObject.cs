using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonStore.Client.Model
{
    public class StoredJsonObject
    {
        public String ApplicationId { get; set; }

        public String Hash { get; set; }

        public String JsonPayload { get; set; }

        public Int32 Version { get; set; }
    }
}
