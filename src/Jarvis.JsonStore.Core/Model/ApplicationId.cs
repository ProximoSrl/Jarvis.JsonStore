using System;
using System.ComponentModel;
using Jarvis.JsonStore.Core.Support;
using MongoDB.Bson.Serialization.Attributes;

namespace Jarvis.JsonStore.Core.Model
{
    [BsonSerializer(typeof(ClientAbstractStringValueBsonSerializer<ApplicationId>))]
    [TypeConverter(typeof(StringValueTypeConverter<ApplicationId>))]
    public class ApplicationId : LowercaseClientAbstractStringValue
    {
        public ApplicationId(string value)
            : base(value)
        {
        }

        public static implicit operator ApplicationId(String value)
        {
            return new ApplicationId(value);
        }

    }
}
