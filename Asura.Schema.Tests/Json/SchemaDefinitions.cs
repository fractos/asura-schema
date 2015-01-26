using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Asura.Schema;
using Asura.Schema.Json;

namespace Asura.Schema.Tests.Json
{
    [TestFixture]
    public class SchemaDefinitions
    {
        protected JsonSchema JsonSchema;

        protected string CorrectDefinitionsSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""definitions"": {
    ""credentials"": {
      ""type"": ""object"",
      ""properties"": {
        ""username"": {
          ""type"": ""string""
        },
        ""password"": {
          ""type"": ""string""
        }
      },
      ""required"": [ ""username"", ""password"" ]
    }
  },
  ""properties"": {
    ""user"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/user"",
      ""$ref"": ""#/definitions/credentials"",
    }
  }
}";

        protected string IncorrectDefinitionsSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""definitions"": {
    ""credentials"": {
      ""type"": ""object"",
      ""properties"": {
        ""username"": {
          ""type"": ""string""
        },
        ""password"": {
          ""type"": ""string""
        }
      },
      ""required"": [ ""username"", ""password"" ]
    }
  },
  ""properties"": {
    ""user"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/user"",
      ""$ref"": ""#/definitions/creds"",
    }
  }
}";

        protected string CorrectMultiDefinitionsSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""definitions"": {
    ""credentials"": {
      ""type"": ""object"",
      ""properties"": {
        ""username"": {
          ""type"": ""string""
        },
        ""password"": {
          ""type"": ""string""
        }
      },
      ""required"": [ ""username"", ""password"" ]
    }
  },
  ""properties"": {
    ""user"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/user"",
      ""$ref"": ""#/definitions/credentials"",
    },
    ""otheruser"" : {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/otheruser"",
      ""$ref"": ""#/definitions/credentials"",
    }
  }
}";

        [SetUp]
        public void SetUp()
        {
            this.JsonSchema = new JsonSchema();
        }

        [Test]
        public void DefinitionReferencePasses()
        {
            string schemaSource = this.CorrectDefinitionsSchemaSource;
            
            using (JsonSchema schema = new JsonSchema())
            {
                schema.Parse(schemaSource);
            }

            Assert.That(true);
        }

        [Test]
        [ExpectedException(typeof(SchemaException))]
        public void DefinitionReferenceFails1()
        {
            string schemaSource = this.IncorrectDefinitionsSchemaSource;

            using (JsonSchema schema = new JsonSchema())
            {
                schema.Parse(schemaSource);
            }
        }

        [Test]
        public void DefinitionUsagePasses()
        {
            string schemaSource = this.CorrectDefinitionsSchemaSource;

            string validateThis =
@"{
  ""user"" : {
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
        public void DefinitionMultiUsagePasses()
        {
            string schemaSource = this.CorrectMultiDefinitionsSchemaSource;

            string validateThis =
@"{
  ""user"" : {
    ""username"": ""demo@user.com"",
    ""password"": ""1234""
  },
  ""otheruser"" : {
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
        public void DefinitionMultiUsageFails()
        {
            string schemaSource = this.CorrectMultiDefinitionsSchemaSource;

            string validateThis =
@"{
  ""user"" : {
    ""username"": ""demo@user.com"",
    ""password"": ""1234""
  },
  ""otheruser"" : {
    ""username"": 1,
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
            Assert.That(e.StartsWith("Property \"username\" was not of expected type string"));
        }
    }
}
