using NUnit.Framework;

using Asura.Schema.Json;

namespace Asura.Schema.Tests.Json
{
    [TestFixture]
    public class SchemaDecoration
    {
        protected JsonSchema JsonSchema;

        protected string FullDecorationSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""properties"": {
    ""username"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/username"",
      ""type"": ""string"",
      ""title"": ""username schema"",
      ""description"": ""Add description here""
    }
  }
}";

        protected string PartialDecorationSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser"",
  ""type"": ""object"",
  ""description"": ""Add description here"",
  ""properties"": {
    ""username"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/username"",
      ""type"": ""string"",
      ""description"": ""Add description here""
    }
  }
}";

        [SetUp]
        public void SetUp()
        {
            this.JsonSchema = new JsonSchema();
        }

        [Test]
        public void FullDecorationParses()
        {
            string schemaSource = this.FullDecorationSchemaSource;

            using (JsonSchema schema = new JsonSchema())
            {
                schema.Parse(schemaSource);
            }

            Assert.True(true);
        }

        [Test]
        public void PartialDecorationParses()
        {
            string schemaSource = this.PartialDecorationSchemaSource;

            using (JsonSchema schema = new JsonSchema())
            {
                schema.Parse(schemaSource);
            }

            Assert.True(true);
        }
    }
}
