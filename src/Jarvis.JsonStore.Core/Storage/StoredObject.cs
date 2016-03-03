using System;

namespace Jarvis.JsonStore.Core.Storage
{
    public class StoredObject
    {
        public Int64 Id { get; set; }

        public String ApplicationId { get; set; }

        public String Hash { get; set; }

        public DateTime TimeStamp { get; set; }

        public String JsonPayload { get; set; }

        /// <summary>
        /// Object is marked as deleted
        /// </summary>
        public Boolean Deleted { get; set; }

        public OperationType OpType { get; set; }
    }

    public enum OperationType
    {
        Put,
        Delete
    }

}
