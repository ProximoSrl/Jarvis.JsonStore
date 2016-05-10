using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonStore.Client.Model
{
    public class CreateIndexRequest
    {
        public String IndexName { get; set; }

        public List<IndexPropertyDefinition> Properties { get; set; }
    }

    public class IndexPropertyDefinition
    {
        public String PropertyName { get; set; }

        public Boolean Descending { get; set; }

        public IndexPropertyDefinition(String propertyName, Boolean descending = false)
        {
            PropertyName = propertyName;
            Descending = descending;
        }
    }
}
