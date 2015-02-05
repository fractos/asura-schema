using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Asura.Schema;
using Asura.Schema.Json;

namespace Asura.Schema.Tests.Json
{
    [TestFixture]
    public class SchemaConstraints
    {
        protected JsonSchema JsonSchema;

        protected string IntegerExclusiveSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/thing"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""properties"": {
    ""code"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/code"",
      ""type"": ""integer"",
      ""minimum"": 10,
      ""exclusiveMinimum"": ""true"",
      ""maximum"": 100,
      ""exclusiveMaximum"": ""true""
    }
  }
}";

        protected string IntegerNonExclusiveSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/thing"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""properties"": {
    ""code"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/code"",
      ""type"": ""integer"",
      ""minimum"": 10,
      ""maximum"": 100,
    }
  }
}";

        protected string IntegerMultipleOfSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/thing"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""properties"": {
    ""code"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/code"",
      ""type"": ""integer"",
      ""multipleOf"": 10
    }
  }
}";

        protected string FloatExclusiveSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/thing"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""properties"": {
    ""code"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/code"",
      ""type"": ""number"",
      ""minimum"": 10.5,
      ""exclusiveMinimum"": ""true"",
      ""maximum"": 100.5,
      ""exclusiveMaximum"": ""true""
    }
  }
}";

        protected string StringLengthSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/thing"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""properties"": {
    ""code"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/code"",
      ""type"": ""string"",
      ""minLength"": 10,
      ""maxLength"": 20
    }
  }
}";

        protected string StringPatternSchemaSource =
@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/thing"",
  ""type"": ""object"",
  ""title"": ""Root schema"",
  ""description"": ""Add description here"",
  ""properties"": {
    ""code"": {
      ""id"": ""http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/code"",
      ""type"": ""string"",
      ""pattern"": ""/abc/""
    }
  }
}";

        [SetUp]
        public void SetUp()
        {
            this.JsonSchema = new JsonSchema();
        }

        #region Integer Exclusive
        [Test]
        public void IntegerExclusivePasses1()
        {
            string schemaSource = this.IntegerExclusiveSchemaSource;

            string validateThis =
@"{
  ""code"": 10,
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
        public void IntegerExclusivePasses2()
        {
            string schemaSource = this.IntegerExclusiveSchemaSource;

            string validateThis =
@"{
  ""code"": 100,
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
        public void IntegerExclusiveFails1()
        {
            string schemaSource = this.IntegerExclusiveSchemaSource;

            string validateThis =
@"{
  ""code"": 9,
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
            Assert.That(e.StartsWith("Property \"code\" with value 9 is less than exclusive minimum 10"));
        }

        [Test]
        public void IntegerExclusiveFails2()
        {
            string schemaSource = this.IntegerExclusiveSchemaSource;

            string validateThis =
@"{
  ""code"": 101,
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
            Assert.That(e.StartsWith("Property \"code\" with value 101 is greater than exclusive maximum 100"));
        }

        [Test]
        public void IntegerExclusiveFails3()
        {
            string schemaSource = this.IntegerExclusiveSchemaSource;

            string validateThis =
@"{
  ""code"": 0,
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
            Assert.That(e.StartsWith("Property \"code\" with value 0 is less than exclusive minimum 10"));
        }

        [Test]
        public void IntegerExclusiveFails4()
        {
            string schemaSource = this.IntegerExclusiveSchemaSource;

            string validateThis =
@"{
  ""code"": 1000,
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
            Assert.That(e.StartsWith("Property \"code\" with value 1000 is greater than exclusive maximum 100"));
        }
        #endregion

        #region Integer - Non-Exclusive
        [Test]
        public void IntegerNonExclusivePasses1()
        {
            string schemaSource = this.IntegerNonExclusiveSchemaSource;

            string validateThis =
@"{
  ""code"": 11,
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
        public void IntegerNonExclusivePasses2()
        {
            string schemaSource = this.IntegerNonExclusiveSchemaSource;

            string validateThis =
@"{
  ""code"": 99,
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
        public void IntegerNonExclusiveFails1()
        {
            string schemaSource = this.IntegerNonExclusiveSchemaSource;

            string validateThis =
@"{
  ""code"": 10,
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
            Assert.That(e.StartsWith("Property \"code\" with value 10 is less than non-exclusive minimum 10"));
        }

        [Test]
        public void IntegerNonExclusiveFails2()
        {
            string schemaSource = this.IntegerNonExclusiveSchemaSource;

            string validateThis =
@"{
  ""code"": 100,
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
            Assert.That(e.StartsWith("Property \"code\" with value 100 is greater than non-exclusive maximum 100"));
        }

        [Test]
        public void IntegerNonExclusiveFails3()
        {
            string schemaSource = this.IntegerNonExclusiveSchemaSource;

            string validateThis =
@"{
  ""code"": 0,
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
            Assert.That(e.StartsWith("Property \"code\" with value 0 is less than non-exclusive minimum 10"));
        }

        [Test]
        public void IntegerNonExclusiveFails4()
        {
            string schemaSource = this.IntegerNonExclusiveSchemaSource;

            string validateThis =
@"{
  ""code"": 1000,
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
            Assert.That(e.StartsWith("Property \"code\" with value 1000 is greater than non-exclusive maximum 100"));
        }
        #endregion

        #region Integer - MultipleOf
        [Test]
        public void IntegerMultipleOfPasses1()
        {
            string schemaSource = this.IntegerMultipleOfSchemaSource;

            string validateThis =
@"{
  ""code"": 10,
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
        public void IntegerMultipleOfPasses2()
        {
            string schemaSource = this.IntegerMultipleOfSchemaSource;

            string validateThis =
@"{
  ""code"": 100,
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
        public void IntegerMultipleOfFails1()
        {
            string schemaSource = this.IntegerMultipleOfSchemaSource;

            string validateThis =
@"{
  ""code"": 1,
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
            Assert.That(e.StartsWith("Property \"code\" with value 1 is not a multiple of 10"));
        }

        [Test]
        public void IntegerMultipleOfFails2()
        {
            string schemaSource = this.IntegerMultipleOfSchemaSource;

            string validateThis =
@"{
  ""code"": 11,
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
            Assert.That(e.StartsWith("Property \"code\" with value 11 is not a multiple of 10"));
        }
        #endregion

        #region String - Length
        [Test]
        public void StringLengthPasses1()
        {
            string schemaSource = this.StringLengthSchemaSource;

            string validateThis =
@"{
  ""code"": ""1234567890"",
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
        public void StringLengthPasses2()
        {
            string schemaSource = this.StringLengthSchemaSource;

            string validateThis =
@"{
  ""code"": ""1234567890123456789"",
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
        public void StringLengthFails1()
        {
            string schemaSource = this.StringLengthSchemaSource;

            string validateThis =
@"{
  ""code"": ""123456789"",
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
            Assert.That(e.StartsWith("Property \"code\" with length 9 has length less than minimum 10"));
        }

        [Test]
        public void StringLengthFails2()
        {
            string schemaSource = this.StringLengthSchemaSource;

            string validateThis =
@"{
  ""code"": ""123456789012345678901"",
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
            Assert.That(e.StartsWith("Property \"code\" with length 21 has length greater than maximum 20"));
        }

        [Test]
        public void StringLengthFails3()
        {
            string schemaSource = this.StringLengthSchemaSource;

            string validateThis =
@"{
  ""code"": ""123"",
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
            Assert.That(e.StartsWith("Property \"code\" with length 3 has length less than minimum 10"));
        }

        [Test]
        public void StringLengthFails4()
        {
            string schemaSource = this.StringLengthSchemaSource;

            string validateThis =
@"{
  ""code"": ""1234567890123456789012345678901234567890"",
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
            Assert.That(e.StartsWith("Property \"code\" with length 40 has length greater than maximum 20"));
        }
        #endregion

        #region String - Pattern
        [Test]
        public void StringPatternPasses1()
        {
            string schemaSource = this.StringPatternSchemaSource;

            string validateThis =
@"{
  ""code"": ""123abc456"",
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
        public void StringPatternFails1()
        {
            string schemaSource = this.StringPatternSchemaSource;

            string validateThis =
@"{
  ""code"": ""1234567890123456789012345678901234567890"",
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
            Assert.That(e.StartsWith("Property \"code\" with value 1234567890123456789012345678901234567890 does not match pattern /abc/"));
        }

        #endregion
    }
}
