using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace Asura.Schema.Json.Extensions
{
    public static class JObjectEx
    {
        public static bool Validate(this JObject self, ISchema schema, IList<string> errorStrings)
        {
            List<SchemaError> errors = new List<SchemaError>();
            if(schema is JsonSchema)
            {
                if(!((JsonSchema) schema).Schema.Validate("$", self, errors))
                {
                    foreach(SchemaError schemaError in errors)
                    {
                        errorStrings.Add(schemaError.ToString());
                    }
                }
            }

            return !errors.Any();
        }
    }
}
