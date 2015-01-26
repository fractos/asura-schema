using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Asura.Schema;
using Asura.Schema.Json.Extensions;

namespace Asura.Schema.Json
{
    public class JsonSchema : ISchema
    {
        protected static Regex regexDefinitionPath = new Regex(@"^\#\/definitions\/(.*?)$", RegexOptions.Compiled);

        private Dictionary<string, JsonSchemaObject> _definitions = new Dictionary<string, JsonSchemaObject>();
        public Dictionary<string, JsonSchemaObject> Definitions
        {
            get { return _definitions; }
            private set { _definitions = value; }
        }

        public JsonSchemaObject FindDefinition(string definitionPath)
        {
            Match match = regexDefinitionPath.Match(definitionPath);

            JsonSchemaObject result = null;

            if(!match.Success || match.Groups.Count < 2 || !match.Groups[1].Success || !this.Definitions.TryGetValue(match.Groups[1].Value, out result))
            {
                return null;
            }

            return result;
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        private JsonSchemaObject _schema;
        public JsonSchemaObject Schema
        {
            get { return _schema; }
            private set { _schema = value; }
        }

        public void Parse(string source)
        {
            JObject j = JObject.Parse(source);
            _schema = this.Generate(j);
        }

        protected JsonSchemaObject Generate(JObject j)
        {
            JToken tokenSchema = j["$schema"];
            JObject definitions = (JObject) j.SelectToken("definitions");

            if(tokenSchema == null)
            {
                this.Id = tokenSchema.ToString();
            }

            if (definitions != null)
            {
                foreach (JProperty child in definitions.Children())
                {
                    this.Definitions.Add(child.Name, JsonSchemaObject.Generate(this, child.Name, (JObject) definitions.SelectToken(child.Name)));
                }
            }

            return JsonSchemaObject.Generate(this, "$", j);
        }

        public bool Validate(string source, IList<string> errors)
        {
            JObject j = JObject.Parse(source);
            return j.Validate(this, errors);
        }
     
        public void Dispose()
        {
            // nothing to do
        }
    }
}
