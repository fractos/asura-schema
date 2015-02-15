using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

namespace Asura.Schema.Json.Extensions
{
    public static class JTokenEx
    {
        /// <summary>
        /// Generate a JSON Schema (Draft 4) for a particular JToken object.
        /// </summary>
        /// <remarks>
        /// https://gist.github.com/fractos/2967ea77b89b4634f51c
        /// Porting some Python code that generates a JSON Schema (Draft 4 compatible - http://json-schema.org/).
        /// Python original by @perenecabuto at - https://github.com/perenecabuto/json_schema_generator
        /// </remarks>
        /// <param name="self"></param>
        /// <param name="schemaVersion"></param>
        /// <param name="idPrefix"></param>
        /// <param name="parentId"></param>
        /// <param name="objectId"></param>
        /// <param name="firstLevel"></param>
        /// <returns></returns>
        public static JObject GenerateSchema(this JToken self, string schemaVersion, string idPrefix, string parentId, string objectId = "", bool firstLevel = true)
        {
            JObject schema = new JObject();

            if (firstLevel)
            {
                schema.Add("$schema", schemaVersion);
            }

            string id = String.Concat(firstLevel ? idPrefix : parentId, "/", objectId);

            schema.Add("id", id);

            schema.Add("type", Enum.GetName(typeof(JTokenType), self.Type).ToLower());
            schema.Add("title", firstLevel ? "Root schema" : String.Concat(objectId, " schema"));
            schema.Add("description", "Add description here");

            //schema.Add("name", firstLevel ? "/" : objectId);

            if (self.Type == JTokenType.Object)
            {
                JObject properties = new JObject();

                foreach (JProperty property in ((JObject) self).Properties())
                {
                    properties.Add(property.Name, property.Value.GenerateSchema(schemaVersion, idPrefix, id, property.Name, false));
                }

                schema.Add("properties", properties);
            }
            else if (self.Type == JTokenType.Array && self.HasValues)
            {
                JTokenType firstType = self.Values().First().Type;
                bool sameType = self.Values().All(v => v.Type == firstType);

                if (sameType)
                {
                    schema.Add("items", self.Values().First().GenerateSchema(schemaVersion, idPrefix, id, "0", false));
                }
                else
                {
                    schema.Add("items", new JArray());

                    List<JToken> children = self.Values().ToList();

                    for (int x = 0; x < children.Count; x++)
                    {
                        JToken child = children[x];
                        ((JArray) schema["items"]).Add(child.GenerateSchema(schemaVersion, idPrefix, id, x.ToString(), false));
                    }
                }
            }

            return schema;
        }
    }
}