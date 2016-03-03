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

    public class FindResult
    {
        public Int64 RecordCount { get; set; }

        public IList<StoredJsonObject> Result { get; set; }

        internal FindResult<T> ConvertToTyped<T>()
        {
            FindResult<T> retValue = new FindResult<T>() { RecordCount = this.RecordCount };
            retValue.Result = Result.Select(o => new StoredJsonObject<T>(o)).ToList();
            return retValue;
        }
    }

    public class FindResult<T>
    {
        public Int64 RecordCount { get; set; }

        public IList<StoredJsonObject<T>> Result { get; set; }
    }
}
