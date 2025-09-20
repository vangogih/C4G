using System.Text;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;

namespace C4G.Editor
{
    public class CodeGenerationFacade
    {
        public Result<string, string> GenerateDTOClass(ParsedSheet parsedSheet)
        {
            bool isValid = ValidateParsedSheet(parsedSheet, out string error);
            if (!isValid)
                return Result<string, string>.FromError(error);

            var sb = new StringBuilder();

            sb.AppendLine($"public partial class {parsedSheet.Name}");
            sb.AppendLine("{");

            foreach (var property in parsedSheet.Properties)
            {
                sb.AppendLine($"    public {property.Type} {property.Name} {{ get; set; }}");
            }

            sb.AppendLine("}");

            string generatedClass = sb.ToString();

            return Result<string, string>.FromValue(generatedClass);
        }

        public Result<string, string> GenerateWrapperClass(ParsedSheet parsedSheet)
        {
            bool isValid = ValidateParsedSheet(parsedSheet, out string error);
            if (!isValid)
                return Result<string, string>.FromError(error);

            var sb = new StringBuilder();

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"public partial class {parsedSheet.Name}Wrapper");
            sb.AppendLine("{");
            
            sb.AppendLine("    public string Name { get; set; }");
            sb.AppendLine($"    public List<{parsedSheet.Name}> Entities {{ get; set; }} = new List<{parsedSheet.Name}>();");

            sb.AppendLine("}");

            string generatedClass = sb.ToString();

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