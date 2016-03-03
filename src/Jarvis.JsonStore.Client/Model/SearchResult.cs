using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonStore.Client.Model
{


    public class SearchResult<T>
    {
        public Int64 RecordCount { get; set; }

        public List<StoredJsonObject<T>> Result { get; set; }

    }
}
