namespace Asura.Schema.Json.Extensions
{
    public class GenerateSchemaOptions
    {
        public bool IncludeId { get; set; } = true;

        public bool IncludeDescription { get; set; } = true;

        public bool IncludeTitle { get; set; } = true;

        public static GenerateSchemaOptions Default = new GenerateSchemaOptions();
    }
}