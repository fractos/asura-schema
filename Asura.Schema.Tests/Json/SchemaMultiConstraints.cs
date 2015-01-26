using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Asura.Schema;
using Asura.Schema.Json;

namespace Asura.Schema.Tests.Json
{
    [TestFixture]
    public class SchemaMultiConstraints
    {
        protected JsonSchema JsonSchema;

        protected string AnyOfSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/thing"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""properties"": {
    ""type"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/type"",
      ""anyOf"": [
        { ""type"": ""string"" },
        { ""type"": ""integer"" }
      ]
    }
  }
}";

        protected string OneOfSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/thing"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""properties"": {
    ""type"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/type"",
      ""oneOf"": [
        { ""type"": ""integer"" },
        { ""enum"": [ ""red"" ] }
      ]
    }
  }
}";

        protected string AllOfSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/thing"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""properties"": {
    ""type"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/type"",
      ""allOf"": [
        { ""type"": ""string"" },
        { ""enum"": [ ""red"" ] }
      ]
    }
  }
}";

        protected string NotSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/thing"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""properties"": {
    ""type"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/type"",
      ""not"": [
        { ""type"": ""string"" },
        { ""enum"": [ ""red"" ] }
      ]
    }
  }
}";

        [SetUp]
        public void SetUp()
        {
            this.JsonSchema = new JsonSchema();
        }

        #region AnyOf
        [Test]
        public void AnyOfPropertyPasses1()
        {
            string schemaSource = this.AnyOfSchemaSource;

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
        public void AnyOfPropertyPasses2()
        {
            string schemaSource = this.AnyOfSchemaSource;

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

            Assert.That(errors.Count == 0);
        }

        [Test]
        public void AnyOfPropertyFails()
        {
            string schemaSource = this.AnyOfSchemaSource;

            string validateThis =
@"{
  ""type"": { ""hello"": ""there"" },
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
            Assert.That(e.StartsWith("'anyOf' constraint failed for property 'type' at 'type'"));
        }
        #endregion

        #region OneOf
        [Test]
        public void OneOfPropertyPasses1()
        {
            string schemaSource = this.OneOfSchemaSource;

            string validateThis =
@"{
  ""type"": ""red"",
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
        public void OneOfPropertyPasses2()
        {
            string schemaSource = this.OneOfSchemaSource;

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

            Assert.That(errors.Count == 0);
        }

        [Test]
        public void OneOfPropertyFails()
        {
            string schemaSource = this.OneOfSchemaSource;

            string validateThis =
@"{
  ""type"": { ""hello"": ""there"" },
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
            Assert.That(e.StartsWith("'oneOf' constraint failed for property 'type' at 'type'"));
        }

        #endregion

        #region AllOf
        [Test]
        public void AllOfPropertyPasses1()
        {
            string schemaSource = this.AllOfSchemaSource;

            string validateThis =
@"{
  ""type"": ""red"",
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
        public void AllOfPropertyFails()
        {
            string schemaSource = this.AllOfSchemaSource;

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

        #endregion

        #region Not
        [Test]
        public void NotPropertyPasses()
        {
            string schemaSource = this.NotSchemaSource;

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

            Assert.That(errors.Count == 0);
        }

        [Test]
        public void NotPropertyFails()
        {
            string schemaSource = this.NotSchemaSource;

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

            Assert.That(errors.Count == 1);
            string e1 = errors.SingleOrDefault(e => e == "'not' constraint failed for property 'type' at 'type'");
            Assert.That(!String.IsNullOrEmpty(e1));
        }

        #endregion
    }
}
