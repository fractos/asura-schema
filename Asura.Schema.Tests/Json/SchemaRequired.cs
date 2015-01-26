using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Asura.Schema;
using Asura.Schema.Json;

namespace Asura.Schema.Tests.Json
{
    [TestFixture]
    public class SchemaRequired
    {
        protected JsonSchema JsonSchema;

        protected string RequiredAtRootLevelSchemaSource =
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
  },
  ""required"": [ ""username"" ]
}";

        protected string RequiredAtMultiLevelSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""properties"": {
    ""details"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/details"",
      ""type"": ""object"",
      ""title"": ""details schema"",
      ""description"": ""Add description here"",
      ""properties"": {
        ""username"": {
          ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/details/username"",
          ""type"": ""string"",
          ""title"": ""username schema"",
          ""description"": ""Add description here""
        },
        ""password"": {
          ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/details/password"",
          ""type"": ""string"",
          ""title"": ""password schema"",
          ""description"": ""Add description here""
        }
      },
      ""required"": [ ""username"", ""password"" ]
    }
  },
  ""required"": [ ""details"" ]
}";

        [SetUp]
        public void SetUp()
        {
            this.JsonSchema = new JsonSchema();
        }

        [Test]
        public void RequiredAtRootLevelPasses()
        {
            string schemaSource = this.RequiredAtRootLevelSchemaSource;

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
        public void RequiredAtRootLevelFails()
        {
            string schemaSource = this.RequiredAtRootLevelSchemaSource;

            string validateThis =
@"{
  ""thing"": ""demo@user.com"",
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
            Assert.That(e.StartsWith("Property \"username\" was not found"));
        }

        [Test]
        public void RequiredAtMultiLevelPasses()
        {
            string schemaSource = this.RequiredAtMultiLevelSchemaSource;

            string validateThis =
@"{
  ""details"": {
    ""username"": ""demo@user.com"",
    ""password"": ""1234""
  }
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
        public void RequiredAtMultiLevelFails1()
        {
            string schemaSource = this.RequiredAtMultiLevelSchemaSource;

            string validateThis =
@"{
  ""details"": {
    ""thing"": ""demo@user.com"",
    ""password"": ""1234""
  }
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
            Assert.That(e.StartsWith("Property \"username\" was not found"));
        }

        [Test]
        public void RequiredAtMultiLevelFails2()
        {
            string schemaSource = this.RequiredAtMultiLevelSchemaSource;

            string validateThis =
@"{
  ""details"": {
    ""thing"": ""demo@user.com"",
    ""thing2"": ""1234""
  }
}";

            List<string> errors = new List<string>();

            using (JsonSchema schema = new JsonSchema())
            {
                schema.Parse(schemaSource);
                schema.Validate(validateThis, errors);
            }

            Assert.That(errors.Count == 2);

            string e1 = errors.SingleOrDefault(e => e == "Property \"username\" was not found");
            string e2 = errors.SingleOrDefault(e => e == "Property \"password\" was not found");
            Assert.That(!String.IsNullOrEmpty(e1));
            Assert.That(!String.IsNullOrEmpty(e2));
        }

        [Test]
        public void RequiredAtMultiLevelFails3()
        {
            string schemaSource = this.RequiredAtMultiLevelSchemaSource;

            string validateThis =
@"{
  ""stuff"": {
    ""thing"": ""demo@user.com"",
    ""thing2"": ""1234""
  }
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
            Assert.That(e.StartsWith("Property \"details\" was not found"));
        }
    }
}
