using System.Text;
using C4G.Core;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;

namespace C4G.Editor
{
    public class CodeGenerationFacade
    {
        public Result<string, EC4GError> GenerateDTOClass(ParsedSheet parsedSheet)
        {
            EC4GError error = ValidateParsedSheet(parsedSheet);
            if (error != EC4GError.None)
                return Result<string, EC4GError>.FromError(error);

            var sb = new StringBuilder();

            sb.AppendLine($"public partial class {parsedSheet.Name}");
            sb.AppendLine("{");

            foreach (var property in parsedSheet.Properties)
            {
                sb.AppendLine($"    public {property.Type} {property.Name} {{ get; set; }}");
            }

            sb.AppendLine("}");

            string generatedClass = sb.ToString();

            return Result<string, EC4GError>.FromValue(generatedClass);
        }

        public Result<string, EC4GError> GenerateWrapperClass(ParsedSheet parsedSheet)
        {
            EC4GError error = ValidateParsedSheet(parsedSheet);
            if (error != EC4GError.None)
                return Result<string, EC4GError>.FromError(error);

            var sb = new StringBuilder();

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"public partial class {parsedSheet.Name}Wrapper");
            sb.AppendLine("{");
            
            sb.AppendLine("    public string Name { get; set; }");
            sb.AppendLine($"    public List<{parsedSheet.Name}> Entities {{ get; set; }} = new List<{parsedSheet.Name}>();");

            sb.AppendLine("}");

            string generatedClass = sb.ToString();

            return Result<string, EC4GError>.FromValue(generatedClass);
        }

        private static EC4GError ValidateParsedSheet(ParsedSheet parsedSheet)
        {
            if (string.IsNullOrEmpty(parsedSheet.Name))
                return EC4GError.CG_ParsedSheetNameNullOrEmpty;

            if (parsedSheet.Properties == null)
                return EC4GError.CG_ParsedSheetPropertiesNull;

            if (parsedSheet.Entities == null)
                return EC4GError.CG_ParsedSheetEntitiesNull;

            return EC4GError.None;
        }
    }
}