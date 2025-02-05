using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace C4G.Editor
{
    public static class CodeGenerator
    {
        private const string CollectionsNamespace = "using System.Collections.Generic;";

        public static string GenerateDTOClass(ParsedSheet parsedSheet)
        {
            ValidateParsedSheet(parsedSheet);

            var builder = new StringBuilder();

            AddCollectionsNamespaceIfNecessary(parsedSheet, builder);

            builder.AppendLine($"public partial class {parsedSheet.Name}");
            builder.AppendLine("{");
            foreach (var property in parsedSheet.Properties)
            {
                builder.AppendLine($"    public {property.Type} {property.Name} {{ get; set; }}");
            }

            builder.AppendLine("}");

            return builder.ToString();
        }

        public static string GenerateWrapperClass(ParsedSheet parsedSheet)
        {
            ValidateParsedSheet(parsedSheet);

            var builder = new StringBuilder();

            builder.AppendLine(CollectionsNamespace);
            builder.AppendLine();
            builder.AppendLine($"public partial class {parsedSheet.Name}Wrapper");
            builder.AppendLine("{");
            builder.AppendLine("    public string Name { get; set; }");
            builder.AppendLine(
                $"    public List<{parsedSheet.Name}> Entities {{ get; set; }} = new List<{parsedSheet.Name}>();");
            builder.AppendLine("}");

            return builder.ToString();
        }

        private static void AddCollectionsNamespaceIfNecessary(ParsedSheet parsedSheet, StringBuilder builder)
        {
            string[] collectionTypes = { "List", "HashSet", "Dictionary" };
            
            // Check if any property type matches the generic collections robustly
            if (!parsedSheet.Properties.Any(property =>
                    collectionTypes.Any(type =>
                        property.Type.StartsWith(type, StringComparison.Ordinal) ||
                        property.Type.StartsWith($"System.Collections.Generic.{type}", StringComparison.Ordinal))))
            {
                return;
            }

            builder.AppendLine(CollectionsNamespace);
            builder.AppendLine();
        }

        private static void ValidateParsedSheet(ParsedSheet parsedSheet)
        {
            if (parsedSheet == null)
                throw new NullReferenceException($"Parameter '{nameof(parsedSheet)}' is null");

            if (string.IsNullOrEmpty(parsedSheet.Name))
                throw new NullReferenceException(
                    $"Property '{nameof(parsedSheet)}.{nameof(parsedSheet.Name)}' is null or empty");

            if (parsedSheet.Properties == null)
                throw new NullReferenceException(
                    $"Property '{nameof(parsedSheet)}.{nameof(parsedSheet.Properties)}' is null");

            if (parsedSheet.Entities == null)
                throw new NullReferenceException(
                    $"Property '{nameof(parsedSheet)}.{nameof(parsedSheet.Entities)}' is null");
        }
    }
}