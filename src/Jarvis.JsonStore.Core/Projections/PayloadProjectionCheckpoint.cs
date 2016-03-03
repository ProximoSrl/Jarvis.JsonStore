using System;

namespace Jarvis.JsonStore.Core.Projections
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
