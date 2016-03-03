using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonStore.Client.Model
{
    /// <summary>
    /// represent a json payload stored in the system.
    /// </summary>
    public class StoredJsonObject
    {

        public String ApplicationId { get; set; }

        public String Hash { get; set; }

        public Int32 Version { get; set; }

        public String JsonPayload { get; set; }


    }
}
