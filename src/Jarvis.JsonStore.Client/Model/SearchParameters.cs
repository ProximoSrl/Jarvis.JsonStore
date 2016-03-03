using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonStore.Client.Model
{
    public class SearchParameters
    {
        /// <summary>
        /// Json query with mongo syntax.
        /// </summary>
        public String JsonQuery { get; set; }

        /// <summary>
        /// Record to start, is used for pagination.
        /// </summary>
        public Int32 Start { get; set; }

        /// <summary>
        /// Number of records to run
        /// </summary>
        public Int32 NumberOfRecords { get; set; }

        /// <summary>
        /// Soring in form of
        /// 
        /// Property asc
        /// or
        /// Property desc
        /// or 
        /// Property 
        /// 
        /// Default order is Ascending.
        /// </summary>
        public String Sort { get; set; }

    }
}
