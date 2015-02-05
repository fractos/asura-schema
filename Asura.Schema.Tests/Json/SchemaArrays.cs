using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Asura.Schema;
using Asura.Schema.Json;

namespace Asura.Schema.Tests.Json
{
    [TestFixture]
    public class SchemaArrays
    {
        protected JsonSchema JsonSchema;

        protected string ArrayItemsSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/session"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""properties"": {
	""locations"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/session/locations"",
      ""type"": ""array"",
      ""title"": ""type schema"",
      ""description"": ""Add description here"",
	  ""items"": {
		""type"": ""object"",
		""title"": ""item schema"",
		""properties"": {
			""datetime"": {
				""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/session/locations/datetime"",
				""type"": ""string"",
				""title"": ""datetime schema"",
				""description"": ""Add description here""
			},
			""location"": {
				""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/session/locations/location"",
				""type"": ""string"",
				""title"": ""location schema"",
				""description"": ""Add description here""
			}
		}
	  }
    }
  }
}";

        [SetUp]
        public void SetUp()
        {
            this.JsonSchema = new JsonSchema();
        }

        [Test]
        public void ArrayItemPasses()
        {
            string schemaSource = this.ArrayItemsSchemaSource;

            string validateThis =
@"{
  ""locations"": [
    {
      ""datetime"": ""2015-02-04 20:23:00.22"",
      ""location"": ""51.02323, -0.00324""
    }
  ]
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
        public void ArrayItemFails1()
        {
            string schemaSource = this.ArrayItemsSchemaSource;

            string validateThis =
@"{
  ""locations"": [
    {
      ""datetime"": 22,
      ""location"": ""51.02323, -0.00324""
    }
  ]
}";

            List<string> errors = new List<string>();

            using (JsonSchema schema = new JsonSchema())
            {
                schema.Parse(schemaSource);
                schema.Validate(validateThis, errors);
            }

            Assert.That(errors.Count == 1);
        }

        [Test]
        public void ArrayItemFails2()
        {
            string schemaSource = this.ArrayItemsSchemaSource;

            string validateThis =
@"{
  ""locations"": [
    {
      ""datetime"": ""2015-02-04 20:23:00.22"",
      ""location"": ""51.02323, -0.00324""
    },
    22
  ]
}";

            List<string> errors = new List<string>();

            using (JsonSchema schema = new JsonSchema())
            {
                schema.Parse(schemaSource);
                schema.Validate(validateThis, errors);
            }

            Assert.That(errors.Count == 1);
        }

        [Test]
        public void ArrayItemFails3()
        {
            string schemaSource = this.ArrayItemsSchemaSource;

            string validateThis =
@"{
  ""locations"": [
    {
      ""datetime"": ""2015-02-04 20:23:00.22"",
      ""location"": ""51.02323, -0.00324""
    },
    {
      ""datetime"": 22,
      ""location"": ""51.02323, -0.00324""
    }
  ]
}";

            List<string> errors = new List<string>();

            using (JsonSchema schema = new JsonSchema())
            {
                schema.Parse(schemaSource);
                schema.Validate(validateThis, errors);
            }

            Assert.That(errors.Count == 1);
        }
    }
}
