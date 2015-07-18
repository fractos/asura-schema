using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Asura.Schema;
using Asura.Schema.Json;
using Asura.Schema.Json.Extensions;
using Newtonsoft.Json.Linq;

namespace Asura.Schema.Harness
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestBasic();
            TestGenerate();
            Console.ReadLine();
        }

        static void TestGenerate()
        {
            string jsonSource =
                @"{
  ""id"": ""http://dlcs.io/iiif-img/monster/2a8d91a1-d2e5-4e02-b415-4c49bf73dc9e"",
  ""customer"": ""monster"",
  ""infojs"": {
    ""@context"": ""http://iiif.io/api/image/2/context.json"",
    ""@id"": ""http://dlcs.io/iiif-img/monster/2a8d91a1-d2e5-4e02-b415-4c49bf73dc9e"",
    ""protocol"": ""http://iiif.io/api/image"",
    ""width"": 1840,
    ""height"": 2869,
    ""tiles"": [
      {
        ""width"": 256,
        ""height"": 256,
        ""scaleFactors"": [
          1,
          2,
          4,
          8,
          16,
          32
        ]
      }
    ],
    ""profile"": [
      ""http://iiif.io/api/image/2/level1.json"",
      {
        ""formats"": [
          ""jpg""
        ],
        ""qualities"": [
          ""native"",
          ""color"",
          ""gray""
        ],
        ""supports"": [
          ""regionByPct"",
          ""sizeByForcedWh"",
          ""sizeByWh"",
          ""sizeAboveFull"",
          ""rotationBy90s"",
          ""mirroring"",
          ""gray""
        ]
      }
    ]
  },
  ""usage"": 0,
  ""dateadded"": ""2015-01-06T11:30:00.771+00:00"",
  ""lastused"": ""0001-01-01T00:00:00+00:00"",
  ""naspath"": ""/nas/monster/2a/8d/91/a1/2a8d91a1-d2e5-4e02-b415-4c49bf73dc9e.jp2"",
  ""s3uri"": ""s3://eu-west-1/dlcs-storage/monster/2a8d91a1-d2e5-4e02-b415-4c49bf73dc9e.jp2"",
  ""origin"": ""http://www.fractos.com/iiif/monster/bookofmonsters00smfair_0175.jp2"",
  ""transformed"": false,
  ""auth"": """",
  ""imagestate"": 2
}";
            JObject jObject = JObject.Parse(jsonSource);

            string schemaVersion = "http://json-schema.org/draft-04/schema#";
            string idPrefix = String.Concat("http://dlcs.io/", "schema");

            GenerateSchemaOptions options = new GenerateSchemaOptions
            {
                IncludeId = false,
                IncludeDescription = false,
                IncludeTitle = false
            };

            JObject schema = jObject.GenerateSchema(schemaVersion, idPrefix, String.Empty, "image", options: options);

            string s = schema.ToString();

            Console.WriteLine(s);
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
