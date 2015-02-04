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
        public bool HasMinimum { get; set; }
        public int Minimum { get; set; }
        public bool ExclusiveMinimum { get; set; }
        public bool HasMaximum { get; set; }
        public int Maximum { get; set; }
        public bool ExclusiveMaximum { get; set; }
        public bool HasMultipleOf { get; set; }
        public int MultipleOf { get; set; }
        #endregion

        #region String
        public bool HasMaxLength { get; set; }
        public int MaxLength { get; set; }
        public bool HasMinLength { get; set; }
        public int MinLength { get; set; }
        public bool HasPattern { get; set; }
        private Regex _pattern;
        public string Pattern
        {
            get { return _pattern.ToString(); }
            set
            {
                if(value.StartsWith("/"))
                {
                    value = value.Substring(1);
                }
                if(value.EndsWith("/"))
                {
                    value = value.Substring(0, value.Length - 1);
                }
                _pattern = new Regex(value, RegexOptions.Compiled);
            }
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
        public bool HasMinItems { get; set; }
        public int MinItems { get; set; }
        public bool HasMaxItems { get; set; }
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

            if (tokenType != null)
            {
                JTokenType type = (JTokenType) Enum.Parse(typeof(JTokenType), tokenType.Value<string>(), true);

                if (type != JTokenType.Integer && type != JTokenType.Float)
                {
                    if (tokenMinimum != null)
                    {
                        throw new SchemaException(String.Format("'minimum' not valid for anything other than a number type at {0}", source.Path));
                    }
                    if (tokenExclusiveMinimum != null)
                    {
                        throw new SchemaException(String.Format("'exclusiveMinimum' not valid for anything other than a number type at {0}", source.Path));
                    }
                    if (tokenMaximum != null)
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
                else if (type != JTokenType.String)
                {
                    if (tokenMinLength != null)
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
                else if (type != JTokenType.Array)
                {
                    if (tokenAdditionalItems != null)
                    {
                        throw new SchemaException(String.Format("'additionalItems' not valid for anything other than an array type at {0}", source.Path));
                    }
                    if (tokenMinItems != null)
                    {
                        throw new SchemaException(String.Format("'minItems' not valid for anything other than an array type at {0}", source.Path));
                    }
                    if (tokenMaxItems != null)
                    {
                        throw new SchemaException(String.Format("'maxItems' not valid for anything other than an array type at {0}", source.Path));
                    }
                }
            }
            #endregion

            if (tokenMinimum != null) { constraint.HasMinimum = true; constraint.Minimum = tokenMinimum.Value<int>(); }
            if (tokenExclusiveMinimum != null) { constraint.ExclusiveMinimum = tokenExclusiveMinimum.Value<bool>(); }
            if (tokenMaximum != null) { constraint.HasMaximum = true; constraint.Maximum = tokenMaximum.Value<int>(); }
            if (tokenExclusiveMaximum != null) { constraint.ExclusiveMaximum = tokenExclusiveMaximum.Value<bool>(); }
            if (tokenMultipleOf != null) { constraint.HasMultipleOf = true; constraint.MultipleOf = tokenMultipleOf.Value<int>(); }
            if (tokenMinLength != null) { constraint.HasMinLength = true; constraint.MinLength = tokenMinLength.Value<int>(); }
            if (tokenMaxLength != null) { constraint.HasMaxLength = true; constraint.MaxLength = tokenMaxLength.Value<int>(); }
            if (tokenPattern != null) { constraint.HasPattern = true; constraint.Pattern = tokenPattern.Value<string>(); }
            if (tokenAdditionalItems != null) { constraint.AdditionalItems = tokenAdditionalItems.Value<bool>(); }
            if (tokenMinItems != null) { constraint.HasMinItems = true; constraint.MinItems = tokenMinItems.Value<int>(); }
            if (tokenMaxItems != null) { constraint.HasMaxItems = true; constraint.MaxItems = tokenMaxItems.Value<int>(); }

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

                if (source.Type == JTokenType.Integer)
                {
                    int value = source.Value<int>();

                    if(this.HasMinimum && this.ExclusiveMinimum && value < this.Minimum)
                    {
                        errors.Add(new SchemaError(String.Format("Property \"{0}\" with value {1} is less than exclusive minimum {2}", name, value, this.Minimum)));
                    }
                    else if(this.HasMinimum && !this.ExclusiveMinimum && value <= this.Minimum)
                    {
                        errors.Add(new SchemaError(String.Format("Property \"{0}\" with value {1} is less than non-exclusive minimum {2}", name, value, this.Minimum)));
                    }
                    if(this.HasMaximum && this.ExclusiveMaximum && value > this.Maximum)
                    {
                        errors.Add(new SchemaError(String.Format("Property \"{0}\" with value {1} is greater than exclusive maximum {2}", name, value, this.Maximum)));
                    }
                    else if(this.HasMaximum && !this.ExclusiveMaximum && value >= this.Maximum)
                    {
                        errors.Add(new SchemaError(String.Format("Property \"{0}\" with value {1} is greater than non-exclusive maximum {2}", name, value, this.Maximum)));
                    }
                    if(this.HasMultipleOf && value % this.MultipleOf != 0)
                    {
                        errors.Add(new SchemaError(String.Format("Property \"{0}\" with value {1} is not a multiple of {2}", name, value, this.MultipleOf)));
                    }
                }
                else if (source.Type == JTokenType.String)
                {
                    string value = source.Value<string>();

                    if(this.HasMinLength && value.Length < this.MinLength)
                    {
                        errors.Add(new SchemaError(String.Format("Property \"{0}\" with length {1} has length less than minimum {2}", name, value.Length, this.MinLength)));
                    }
                    if(this.HasMaxLength && value.Length > this.MaxLength)
                    {
                        errors.Add(new SchemaError(String.Format("Property \"{0}\" with length {1} has length greater than maximum {2}", name, value.Length, this.MaxLength)));
                    }
                    if(this.HasPattern && !_pattern.IsMatch(value))
                    {
                        errors.Add(new SchemaError(String.Format("Property \"{0}\" with value {1} does not match pattern /{2}/", name, value, this.Pattern)));
                    }
                }
                else if (source.Type == JTokenType.Array)
                {
                    JToken[] array = source.ToArray();

                    if(this.HasMinItems && array.GetLength(0) < this.MinItems)
                    {
                        errors.Add(new SchemaError(String.Format("Property \"{0}\" with {1} items has fewer items than minimum {2}", name, array.GetLength(0), this.MinItems)));
                    }
                    if(this.HasMaxItems && array.GetLength(0) > this.MaxItems)
                    {
                        errors.Add(new SchemaError(String.Format("Property \"{0}\" with {1} items has more items than maximum {2}", name, array.GetLength(0), this.MaxItems)));
                    }
                }
            }

            return errors.Count == originalErrorCount;
        }

    }

}
