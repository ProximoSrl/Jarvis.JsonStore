using System;
using Jarvis.JsonStore.Client.Model;
using ApplicationId = Jarvis.JsonStore.Core.Model.ApplicationId;

namespace Jarvis.JsonStore.Core.Storage
{
    public class StoredObject
    {
        public Int64 Id { get; set; }

        public ApplicationId ApplicationId { get; set; }

        public Int32 Version { get; set; }

        public String Hash { get; set; }

        public DateTime TimeStamp { get; set; }

        public String JsonPayload { get; set; }

        /// <summary>
        /// Object is marked as deleted
        /// </summary>
        public Boolean Deleted { get; set; }

        public OperationType OpType { get; set; }

        public StoredJsonObject ToClientStoredJsonObject()
        {
            return new StoredJsonObject()
            {
                Hash = Hash,
                JsonPayload = JsonPayload,
                ApplicationId = ApplicationId,
                Version = Version,
            };
        }
    }

    public enum OperationType
    {
        Put,
        Delete
    }

}
