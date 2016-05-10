using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonStore.Host.Models
{
    public class CreateIndexModel
    {
        public String  IndexName { get; set; }

        public List<IndexProperty> Properties { get; set; }
    }

    public class IndexProperty
    {
        public String PropertyName { get; set; }

        public Boolean Descending { get; set; }
    }
}
