using Jarvis.JsonObjectService.Core.Support;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonObjectService.Tests.Core.Support
{
    [TestFixture]
    public class HashUtilsTests
    {
        [Test]
        public void verify_hash_unordered_properties()
        {
            String jsonObject1 = @"{ ""prop1"" : ""test1"", ""prop2"" : ""test2""}";
            String jsonObject2 = @"{ ""prop2"" : ""test2"", ""prop1"" : ""test1""}";

            Assert.AreEqual(
                HashUtils.GetHashOfSerializedJson(jsonObject1),
                HashUtils.GetHashOfSerializedJson(jsonObject2));
        }
    }
}
