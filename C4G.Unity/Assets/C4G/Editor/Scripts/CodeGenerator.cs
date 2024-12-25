using System;
using System.Text;

namespace C4G.Editor
{
    public static class CodeGenerator
    {
        public static string GenerateDTOClass(ParsedSheet parsedSheet)
        {
            ThrowIfParsedSheetIsInvalid(parsedSheet);

            var sb = new StringBuilder();

            sb.AppendLine($"public partial class {parsedSheet.Name}");
            sb.AppendLine("{");

            foreach (var property in parsedSheet.Properties)
            {
                sb.AppendLine($"    public {property.Type} {property.Name} {{ get; set; }}");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        public static string GenerateWrapperClass(ParsedSheet parsedSheet)
        {
            ThrowIfParsedSheetIsInvalid(parsedSheet);

            var sb = new StringBuilder();

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"public partial class {parsedSheet.Name}Wrapper");
            sb.AppendLine("{");
            
            sb.AppendLine("    public string Name { get; set; }");
            sb.AppendLine($"    public List<{parsedSheet.Name}> Entities {{ get; set; }} = new List<{parsedSheet.Name}>();");

            sb.AppendLine("}");

            return sb.ToString();
        }

        private static void ThrowIfParsedSheetIsInvalid(ParsedSheet parsedSheet)
        {
            if (parsedSheet == null)
                throw new NullReferenceException($"Parameter '{nameof(parsedSheet)}' is null");

            if (string.IsNullOrEmpty(parsedSheet.Name))
                throw new NullReferenceException($"Property '{nameof(parsedSheet)}.{nameof(parsedSheet.Name)}' is null or empty");

            if (parsedSheet.Properties == null)
                throw new NullReferenceException($"Property '{nameof(parsedSheet)}.{nameof(parsedSheet.Properties)}' is null");

            if (parsedSheet.Entities == null)
                throw new NullReferenceException($"Property '{nameof(parsedSheet)}.{nameof(parsedSheet.Entities)}' is null");
        }
    }
}