using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace Asura.Schema.Json
{
    public enum JsonSchemaObjectConstraintMembership
    {
        AllOf, AnyOf, OneOf, Not
    }

    public class JsonSchemaObject
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        private Dictionary<string, JsonSchemaObject> _properties = new Dictionary<string, JsonSchemaObject>();
        public Dictionary<string, JsonSchemaObject> Properties
        {
            get { return _properties; }
            private set { _properties = value; }
        }

        private List<JsonSchemaObjectConstraint> _constraints = new List<JsonSchemaObjectConstraint>();
        public List<JsonSchemaObjectConstraint> Constraints
        {
            get { return _constraints; }
            private set { _constraints = value; }
        }

        private JsonSchemaObjectConstraintMembership _constraintMembership = JsonSchemaObjectConstraintMembership.AllOf;
        public JsonSchemaObjectConstraintMembership ConstraintMembership
        {
            get { return _constraintMembership; }
            private set { _constraintMembership = value; }
        }

        public static JsonSchemaObject Generate(JsonSchema schema, string name, JObject j)
        {
            JToken tokenId = j["id"];
            JToken tokenTitle = j["title"];
            JToken tokenDescription = j["description"];

            JToken tokenAllOf = j["allOf"];
            JToken tokenAnyOf = j["anyOf"];
            JToken tokenOneOf = j["oneOf"];
            JToken tokenNot = j["not"];

            JsonSchemaObject schemaObject = new JsonSchemaObject
            {
                ID = tokenId != null ? tokenId.ToString() : GenerateSchemaObjectId(schema, name, j),
                Title = tokenTitle != null ? tokenTitle.ToString() : String.Empty,
                Description = tokenDescription != null ? tokenDescription.ToString() : String.Empty
            };

            if(tokenAllOf == null && tokenAnyOf == null && tokenOneOf == null && tokenNot == null)
            {
                schemaObject.Constraints.Add(JsonSchemaObjectConstraint.Generate(schema, j));
            }
            JToken tokenMultiConstraint = null;

            if(tokenAllOf != null)
            {
                tokenMultiConstraint = tokenAllOf;
                schemaObject.ConstraintMembership = JsonSchemaObjectConstraintMembership.AllOf;
            }
            if(tokenAnyOf != null)
            {
                if(tokenMultiConstraint != null)
                {
                    throw new SchemaException(String.Format("'anyOf' multi-constraint identifier cannot be used in conjunction with other multi-constraint identifiers at '{0}'", j.Path));
                }
                tokenMultiConstraint = tokenAnyOf;
                schemaObject.ConstraintMembership = JsonSchemaObjectConstraintMembership.AnyOf;
            }
            if(tokenOneOf != null)
            {
                if (tokenMultiConstraint != null)
                {
                    throw new SchemaException(String.Format("'oneOf' multi-constraint identifier cannot be used in conjunction with other multi-constraint identifiers at '{0}'", j.Path));
                }
                tokenMultiConstraint = tokenOneOf;
                schemaObject.ConstraintMembership = JsonSchemaObjectConstraintMembership.OneOf;
            }
            if(tokenNot != null)
            {
                if (tokenMultiConstraint != null)
                {
                    throw new SchemaException(String.Format("'not' multi-constraint identifier cannot be used in conjunction with other multi-constraint identifiers at '{0}'", j.Path));
                }
                tokenMultiConstraint = tokenNot;
                schemaObject.ConstraintMembership = JsonSchemaObjectConstraintMembership.Not;
            }

            if(tokenMultiConstraint != null)
            {
                foreach (JToken constraint in tokenMultiConstraint.Children())
                {
                    schemaObject.Constraints.Add(JsonSchemaObjectConstraint.Generate(schema, constraint));
                }
            }

            JObject properties = (JObject) j.SelectToken("properties");

            if (properties != null)
            {
                foreach (JProperty child in properties.Children())
                {
                    schemaObject.Properties.Add(child.Name, JsonSchemaObject.Generate(schema, child.Name, (JObject) properties.SelectToken(child.Name)));
                }
            }

            return schemaObject;
        }

        protected static string GenerateSchemaObjectId(JsonSchema schema, string name, JObject j)
        {
            return String.Concat(schema.Id, "/", j.Path.Replace(".", "/"), "/", name);
        }

        public bool Validate(string name, JToken source, IList<SchemaError> errors)
        {
            List<List<SchemaError>> localErrors = new List<List<SchemaError>>();

            foreach (JsonSchemaObjectConstraint constraint in this.Constraints)
            {
                localErrors.Add(new List<SchemaError>());
                constraint.Validate(name, source, localErrors.Last());
            }

            if(this.ConstraintMembership == JsonSchemaObjectConstraintMembership.AllOf)
            {
                foreach(List<SchemaError> constraintErrors in localErrors)
                {
                    foreach(SchemaError constraintError in constraintErrors)
                    {
                        errors.Add(constraintError);
                    }
                }
            }
            else if(this.ConstraintMembership == JsonSchemaObjectConstraintMembership.AnyOf)
            {
                bool passed = localErrors.Any(e => !e.Any()); // one constraint gave no errors

                if(!passed)
                {
                    errors.Add(new SchemaError(String.Format("'anyOf' constraint failed for property '{0}' at '{1}'", name, source.Path)));
                }
            }
            else if(this.ConstraintMembership == JsonSchemaObjectConstraintMembership.OneOf)
            {
                bool passed = localErrors.Count(e => !e.Any()) == 1; // constraints giving no errors

                if(!passed)
                {
                    errors.Add(new SchemaError(String.Format("'oneOf' constraint failed for property '{0}' at '{1}'", name, source.Path)));
                }
            }
            else if(this.ConstraintMembership == JsonSchemaObjectConstraintMembership.Not)
            {
                bool passed = localErrors.All(e => e.Any());

                if (!passed)
                {
                    errors.Add(new SchemaError(String.Format("'not' constraint failed for property '{0}' at '{1}'", name, source.Path)));
                }
            }

            if(this.Constraints.All(c => c.TypeReference == null))
            {
                foreach (JToken property in source.Children())
                {
                    if (property is JProperty)
                    {
                        JProperty jp = (JProperty) property;

                        if (this.Properties.ContainsKey(jp.Name))
                        {
                            this.Properties[jp.Name].Validate(jp.Name, property.First, errors);
                        }
                    }
                }
            }            

            return !errors.Any();
        }
    }
}
