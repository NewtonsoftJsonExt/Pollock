using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Web.Script.Serialization;

namespace Tests
{
    [TestFixture]
    public class EnumTemplateTests
    {
        [Serializable]
        class ValueContainer
        {
            [JsonProperty("value")]
            public EnumTemplate Value { get; set; }
        }

        [Test, Category("not_mono")]
        public void JsSerializer()
        {
            const string data = @"{""value"":""Ctr""}";
            var v = new JavaScriptSerializer().Deserialize<ValueContainer>(data);
            Assert.AreEqual(EnumTemplate.Contractor, v.Value);
        }

        [Test]
        public void Newtonsoft()
        {
            const string data = @"{""value"":""Ctr""}";
            var result = JsonConvert.DeserializeObject<ValueContainer>(data);
            Assert.AreEqual(EnumTemplate.Contractor, result.Value);
        }

        [Test]
        public void Newtonsoft_serialize()
        {
            const string expected = @"{""value"":""Mgr""}";
            var result = JsonConvert.SerializeObject(new ValueContainer { Value = EnumTemplate.Manager });
            Assert.AreEqual(expected, result);
        }
    }
}
