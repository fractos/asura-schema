using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Asura.Schema;
using Asura.Schema.Json;

namespace Asura.Schema.Harness
{
    class Program
    {
        static void Main(string[] args)
        {
            TestBasic();

            Console.ReadLine();
        }

        static void TestBasic()
        {
            string schemaSource =
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

            string validateThis =
@"{
  ""details"": {
    ""username"": ""demo@user.com"",
    ""password"": ""1234""
  }
}";

            List<string> errors = new List<string>();

            using (ISchema schema = new JsonSchema())
            {
                schema.Parse(schemaSource);
                schema.Validate(validateThis, errors);
            }
        }
    }
}
