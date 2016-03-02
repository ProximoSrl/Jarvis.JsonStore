using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonObjectService.Core.Support
{
    public static class HashUtils
    {
        public static String GetHashOfSerializedJson(String str)
        {
            
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(NormalizeJsonString(str)));
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }

        public static string NormalizeJsonString(string json)
        {
            // Parse json string into JObject.
            var parsedObject = JObject.Parse(json);

            // Sort properties of JObject.
            var normalizedObject = SortPropertiesAlphabetically(parsedObject);

            // Serialize JObject .
            return JsonConvert.SerializeObject(normalizedObject);
        }

        private static JObject SortPropertiesAlphabetically(JObject original)
        {
            var result = new JObject();

            foreach (var property in original.Properties().ToList().OrderBy(p => p.Name))
            {
                var value = property.Value as JObject;

                if (value != null)
                {
                    value = SortPropertiesAlphabetically(value);
                    result.Add(property.Name, value);
                }
                else
                {
                    result.Add(property.Name, property.Value);
                }
            }

            return result;
        }
    }

 
}
