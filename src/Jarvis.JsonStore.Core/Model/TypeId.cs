using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonStore.Core.Model
{
    public class TypeId : LowercaseClientAbstractStringValue
    {
        public TypeId(string value)
            : base(value)
        {
        }

        public static implicit operator TypeId(String value)
        {
            return new TypeId(value);
        }

    }
}
