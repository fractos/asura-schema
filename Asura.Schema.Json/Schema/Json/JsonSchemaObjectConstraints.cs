using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Asura.Schema;

namespace Asura.Schema.Json
{
    public class JsonSchemaObjectConstraint
    {
        public JTokenType Type { get; set; }
        public JsonSchemaObject TypeReference { get; set; }

        private List<string> _required = new List<string>();
        public List<string> Required
        {
            get { return _required; }
            private set { _required = value; }
        }

        private List<string> _enums = new List<string>();
        public List<string> Enums
        {
            get { return _enums; }
            private set { _enums = value; }
        }

        #region Number/int
        public int Minimum { get; set; }
        public bool ExclusiveMinimum { get; set; }
        public int Maximum { get; set; }
        public bool ExclusiveMaximum { get; set; }
        public int MultipleOf { get; set; }
        #endregion

        #region String
        public int MaxLength { get; set; }
        public int MinLength { get; set; }
        private Regex _pattern;
        public string Pattern
        {
            get { return _pattern.ToString(); }
            set { _pattern = new Regex(value, RegexOptions.Compiled); }
        }
        #endregion

        #region Arrays
        public bool AdditionalItems { get; set; }
        private List<JsonSchemaObject> _items = new List<JsonSchemaObject>();
        public List<JsonSchemaObject> Items
        {
            get { return _items; }
            set { _items = value; }
        }
        public int MinItems { get; set; }
        public int MaxItems { get; set; }
        #endregion

        public static JsonSchemaObjectConstraint Generate(JsonSchema schema, JToken source)
        {
            JsonSchemaObjectConstraint constraint = new JsonSchemaObjectConstraint();

            JToken tokenType = source["type"];
            JToken tokenRef = source["$ref"];
            JToken tokenEnum = source["enum"];
            JToken tokenRequired = source["required"];

            JToken tokenMinimum = source["minimum"];
            JToken tokenExclusiveMinimum = source["exclusiveMinimum"];
            JToken tokenMaximum = source["maximum"];
            JToken tokenExclusiveMaximum = source["exclusiveMaximum"];
            JToken tokenMultipleOf = source["multipleOf"];
            JToken tokenMinLength = source["minLength"];
            JToken tokenMaxLength = source["maxLength"];
            JToken tokenPattern = source["pattern"];
            JToken tokenAdditionalItems = source["additionalItems"];
            JToken tokenItems = source["items"];
            JToken tokenMinItems = source["minItems"];
            JToken tokenMaxItems = source["maxItems"];

            JsonSchemaObject schemaTypeReference = null;

            #region assertions
            if (tokenType == null && tokenRef == null && tokenEnum == null)
            {
                throw new SchemaException(String.Format("Could not find 'type', '$ref' or 'enum' property on schema object '{0}'", source.Path));
            }
            if (tokenType != null && tokenRef != null)
            {
                throw new SchemaException(String.Format("'$ref' and 'type' properties are not permitted at same level of schema object '{0}'", source.Path));
            }
            if (tokenRef != null)
            {
                schemaTypeReference = schema.FindDefinition(tokenRef.ToString());
                if (schemaTypeReference == null)
                {
                    throw new SchemaException(String.Format("Could not find definition of type with reference '{0}' on schema object '{1}'", tokenRef.ToString(), source.Path));
                }
            }

            if(source.Type != JTokenType.Integer && source.Type != JTokenType.Float)
            {
                if(tokenMinimum != null)
                {
                    throw new SchemaException(String.Format("'minimum' not valid for anything other than a number type at {0}", source.Path));
                }
                if(tokenExclusiveMinimum != null)
                {
                    throw new SchemaException(String.Format("'exclusiveMinimum' not valid for anything other than a number type at {0}", source.Path));
                }
                if(tokenMaximum != null)
                {
                    throw new SchemaException(String.Format("'maximum' not valid for anything other than a number type at {0}", source.Path));
                }
                if (tokenExclusiveMaximum != null)
                {
                    throw new SchemaException(String.Format("'exclusiveMaximum' not valid for anything other than a number type at {0}", source.Path));
                }
                if (tokenMultipleOf != null)
                {
                    throw new SchemaException(String.Format("'multipleOf' not valid for anything other than a number type at {0}", source.Path));
                }
            }
            else if(source.Type != JTokenType.String)
            {
                if(tokenMinLength != null)
                {
                    throw new SchemaException(String.Format("'minLength' not valid for anything other than a string type at {0}", source.Path));
                }
                if (tokenMaxLength != null)
                {
                    throw new SchemaException(String.Format("'maxLength' not valid for anything other than a string type at {0}", source.Path));
                }
                if (tokenPattern != null)
                {
                    throw new SchemaException(String.Format("'pattern' not valid for anything other than a string type at {0}", source.Path));
                }
            }
            else if(source.Type != JTokenType.Array)
            {
                if(tokenAdditionalItems != null)
                {
                    throw new SchemaException(String.Format("'additionalItems' not valid for anything other than an array type at {0}", source.Path));
                }
                if(tokenMinItems != null)
                {
                    throw new SchemaException(String.Format("'minItems' not valid for anything other than an array type at {0}", source.Path));
                }
                if (tokenMaxItems != null)
                {
                    throw new SchemaException(String.Format("'maxItems' not valid for anything other than an array type at {0}", source.Path));
                }
            }
            #endregion

            if (tokenMinimum != null) { constraint.Minimum = tokenMinimum.Value<int>(); }
            if (tokenExclusiveMinimum != null) { constraint.ExclusiveMinimum = tokenExclusiveMinimum.Value<bool>(); }
            if (tokenMaximum != null) { constraint.Maximum = tokenMaximum.Value<int>(); }
            if (tokenExclusiveMaximum != null) { constraint.ExclusiveMaximum = tokenExclusiveMaximum.Value<bool>(); }
            if (tokenMultipleOf != null) { constraint.MultipleOf = tokenMultipleOf.Value<int>(); }
            if (tokenMinLength != null) { constraint.MinLength = tokenMinLength.Value<int>(); }
            if (tokenMaxLength != null) { constraint.MaxLength = tokenMaxLength.Value<int>(); }
            if (tokenPattern != null) { constraint.Pattern = tokenPattern.Value<string>(); }
            if (tokenAdditionalItems != null) { constraint.AdditionalItems = tokenAdditionalItems.Value<bool>(); }
            if (tokenMinItems != null) { constraint.MinItems = tokenMinItems.Value<int>(); }
            if (tokenMaxItems != null) { constraint.MaxItems = tokenMaxItems.Value<int>(); }

            if (tokenItems != null)
            {

            }

            // $ref and type
            if (tokenRef == null && tokenType != null)
            {
                constraint.Type = (JTokenType) Enum.Parse(typeof(JTokenType), tokenType.ToString(), true);
            }
            else if (tokenRef != null && tokenType == null)
            {
                constraint.TypeReference = schemaTypeReference;
            }

            // required
            if (tokenRequired != null && tokenRequired.Type == JTokenType.Array)
            {
                constraint.Required.AddRange(tokenRequired.Children().Where(c => c.Type == JTokenType.String).Select(c => c.Value<string>()));
            }

            // enums
            if (tokenEnum != null)
            {
                constraint.Enums.AddRange(tokenEnum.Children().Where(c => c.Type == JTokenType.String).Select(c => c.Value<string>()));
            }

            return constraint;
        }

        public bool Validate(string name, JToken source, IList<SchemaError> errors)
        {
            int originalErrorCount = errors.Count;

            foreach (string requiredProperty in this.Required)
            {
                if (source.SelectToken(requiredProperty) == null)
                {
                    errors.Add(new SchemaError(String.Format("Property \"{0}\" was not found", requiredProperty)));
                }
            }

            if (this.TypeReference != null)
            {
                this.TypeReference.Validate(name, source, errors);
            }
            else
            {
                if (this.Type != JTokenType.None && source.Type != this.Type)
                {
                    errors.Add(new SchemaError(String.Format("Property \"{0}\" was not of expected type {1}", name, this.Type.ToString().ToLower())));
                }

                if (this.Enums.Any() && !this.Enums.Any(e => e == source.ToString()))
                {
                    errors.Add(new SchemaError(String.Format("Property \"{0}\" was not one of the expected enum values", name)));
                }
            }

            return errors.Count == originalErrorCount;
        }

    }

}
