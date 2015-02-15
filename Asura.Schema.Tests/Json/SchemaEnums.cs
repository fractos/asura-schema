using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Asura.Schema.Json;

namespace Asura.Schema.Tests.Json
{
    [TestFixture]
    public class SchemaEnums
    {
        protected JsonSchema JsonSchema;

        protected string EnumsSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/thing"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""properties"": {
    ""type"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/type"",
      ""type"": ""string"",
      ""enum"": [ ""red"", ""green"", ""blue"" ]
    }
  }
}";

        [SetUp]
        public void SetUp()
        {
            this.JsonSchema = new JsonSchema();
        }

        [Test]
        public void EnumPropertyPasses()
        {
            string schemaSource = this.EnumsSchemaSource;

            string validateThis =
@"{
  ""type"": ""green"",
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
        public void EnumPropertyFails1()
        {
            string schemaSource = this.EnumsSchemaSource;

            string validateThis =
@"{
  ""type"": ""black"",
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
            Assert.That(e.StartsWith("Property \"type\" was not one of the expected enum values"));
        }

        [Test]
        public void EnumPropertyFails2()
        {
            string schemaSource = this.EnumsSchemaSource;

            string validateThis =
@"{
  ""type"": 1,
}";

            List<string> errors = new List<string>();

            using (JsonSchema schema = new JsonSchema())
            {
                schema.Parse(schemaSource);
                schema.Validate(validateThis, errors);
            }

            Assert.That(errors.Count == 2);
            string e1 = errors.SingleOrDefault(e => e == "Property \"type\" was not of expected type string");
            string e2 = errors.SingleOrDefault(e => e == "Property \"type\" was not one of the expected enum values");
            Assert.That(!String.IsNullOrEmpty(e1));
            Assert.That(!String.IsNullOrEmpty(e2));
        }

    }
}
