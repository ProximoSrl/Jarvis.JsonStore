using Newtonsoft.Json;
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

    public class StoredJsonObject<T>
    {
        public StoredJsonObject(StoredJsonObject original)
        {
            this.ApplicationId = original.ApplicationId;
            this.Hash = original.Hash;
            this.Version = original.Version;
            if (!String.IsNullOrEmpty(original.JsonPayload))
            {
                this.Payload = JsonConvert.DeserializeObject<T>(original.JsonPayload);
            }
        }

        public String ApplicationId { get; set; }

        public String Hash { get; set; }

        public Int32 Version { get; set; }

        public T Payload { get; set; }


    }
}
