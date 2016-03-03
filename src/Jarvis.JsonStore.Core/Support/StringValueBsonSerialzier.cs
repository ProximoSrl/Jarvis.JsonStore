using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jarvis.JsonStore.Core.Model;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace Jarvis.JsonStore.Core.Support
{
    public class ClientAbstractStringValueBsonSerializer<T> : IBsonSerializer
    {
 
        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.Null)
            {
                context.Reader.ReadNull();
                return null;
            }

            var id = context.Reader.ReadString();
            return Activator.CreateInstance(typeof(T), new object[] { id });
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
            }
            else
            {
                context.Writer.WriteString((ClientAbstractStringValue)value);
            }
        }

        public Type ValueType { get; private set; }
    }
}
