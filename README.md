# Asura.Schema
A Json-Schema draft 4 validator.

Mostly. Probably covered about 75% of the specification so far.

Draft 4 specification at: http://json-schema.org/

Be warned, this is a work in progress. I'm not going at it with the goals of 100% coverage if it turns out to be a pain,
or with ideas of backwards compatibility between draft versions, or with any eye to future versions. This is just doing
what it needs to do for my purposes.

##Basic usage


schemaSource:
```json
{
  "$schema": "http://json-schema.org/draft-04/schema#",
  "id": "http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser",
  "type": "object",
  "title": "Root schema",
  "description": "Add description here",
  "properties": {
    "username": {
      "id": "http://xizi.io/4bf232bf-b24b-4d36-b36c-b1214df98071/demouser/username",
      "type": "string",
      "title": "username schema",
      "description": "Add description here"
    }
  },
  "required": [ "username" ]
}
```

validateThis:
```json
{
  "details": {
    "username": "demo@user.com",
    "password": "1234"
  }
}
```
Assuming `schemaSource` is a schema compatible with Json-Schema draft 4, and `validateThis` is a piece of Json that should be validated against that schema.

```C#
List<string> errors = new List<string>();

using (JsonSchema schema = new JsonSchema())
{
	schema.Parse(schemaSource);
	if(!schema.Validate(validateThis, errors))
	{
	  Console.WriteLine(String.Join("\r\n", errors));
	}
}
```
