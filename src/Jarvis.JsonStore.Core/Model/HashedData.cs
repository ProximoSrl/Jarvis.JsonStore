using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonStore.Core.Model
{
    public class HashedData
    {
        public String Hash { get; set; }

        public Int32 Version { get; set; }

        public ApplicationId ApplicationId { get; set; }
    }
}
