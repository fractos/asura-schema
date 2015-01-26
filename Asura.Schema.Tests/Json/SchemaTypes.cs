using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Asura.Schema;
using Asura.Schema.Json;

namespace Asura.Schema.Tests.Json
{
    [TestFixture]
    public class SchemaTypes
    {
        protected JsonSchema JsonSchema;

        protected string StringsSchemaSource =
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

        [SetUp]
        public void SetUp()
        {
            this.JsonSchema = new JsonSchema();
        }

        [Test]
        public void StringPropertyPasses()
        {
            string schemaSource = this.StringsSchemaSource;

            string validateThis =
@"{
  ""username"": ""demo@user.com"",
}";

            List<string> errors = new List<string>();

            using (JsonSchema schema = new JsonSchema())
            {
                schema.Parse(schemaSource);
                schema.Validate(validateThis, errors);
            }

            Assert.That(errors.Count == 0);
        }

        [Test]
        public void StringPropertyFails()
        {
            string schemaSource = this.StringsSchemaSource;

            string validateThis =
@"{
  ""username"": 1,
}";

            List<string> errors = new List<string>();

            using (JsonSchema schema = new JsonSchema())
            {
                schema.Parse(schemaSource);
                schema.Validate(validateThis, errors);
            }

            Assert.That(errors.Count == 1);
            string e = errors.SingleOrDefault();
            Assert.That(!String.IsNullOrEmpty(e));
            Assert.That(e.StartsWith("Property \"username\" was not of expected type string"));
        }

    }
}
