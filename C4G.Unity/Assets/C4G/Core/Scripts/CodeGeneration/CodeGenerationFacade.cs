using C4G.Core.SheetsParsing;
using C4G.Core.Utils;

namespace C4G.Editor
{
    public class CodeGenerationFacade
    {
        private readonly CodeWriter _codeWriter = new CodeWriter("    ");

        public Result<string, string> GenerateDTOClass(ParsedSheet parsedSheet)
        {
            bool isValid = ValidateParsedSheet(parsedSheet, out string error);
            if (!isValid)
                return Result<string, string>.FromError(error);

            _codeWriter.Clear();

            _codeWriter
                .AddUsing("System.Collections.Generic")
                .WritePublicClass(name: parsedSheet.Name, isPartial: true, baseClass: string.Empty, w =>
                {
                    for (var propertyIndex = 0; propertyIndex < parsedSheet.Properties.Count; propertyIndex++)
                    {
                        ParsedPropertyInfo property = parsedSheet.Properties[propertyIndex];
                        w.WritePublicProperty(property.Name, property.Type);
                    }
                });

            string generatedClass = _codeWriter.Build();

            return Result<string, string>.FromValue(generatedClass);
        }

        public Result<string, string> GenerateWrapperClass(ParsedSheet parsedSheet)
        {
            bool isValid = ValidateParsedSheet(parsedSheet, out string error);
            if (!isValid)
                return Result<string, string>.FromError(error);

            _codeWriter.Clear();

            _codeWriter
                .AddUsing("System.Collections.Generic")
                .WritePublicClass(name: $"{parsedSheet.Name}Wrapper", isPartial: true, baseClass: string.Empty, w =>
                {
                    w.WritePublicProperty("Name", "string");
                    w.WritePublicProperty("Entities", $"List<{parsedSheet.Name}>", $"new List<{parsedSheet.Name}>()");
                });

            string generatedClass = _codeWriter.Build();

            return Result<string, string>.FromValue(generatedClass);
        }

        private static bool ValidateParsedSheet(ParsedSheet parsedSheet, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrEmpty(parsedSheet.Name))
                error = "Code generation error. ParsedSheet name is null or empty";
            else if (parsedSheet.Properties == null)
                error = "Code generation error. ParsedSheet properties are null";
            else if (parsedSheet.Entities == null)
                error = "Code generation error. ParsedSheet entities are null";

            return string.IsNullOrEmpty(error);
        }
    }
}