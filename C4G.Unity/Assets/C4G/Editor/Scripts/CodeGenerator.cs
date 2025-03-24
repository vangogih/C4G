using System;
using System.Linq;
using System.Text;

namespace C4G.Editor
{
    public static class CodeGenerator
    {
        private const string CollectionsNamespace = "System.Collections.Generic";

        public static string GenerateDTOClass(ParsedSheet parsedSheet)
        {
            ValidateParsedSheet(parsedSheet);

            var codeWriter = new CodeWriter();

            AddCollectionsNamespaceIfNecessary(parsedSheet, codeWriter);
            codeWriter.WritePartialClass(parsedSheet.Name, () =>
            {
                foreach (var property in parsedSheet.Properties)
                    codeWriter.WritePublicProperty(property.Name, property.Type);
            });

            return codeWriter.ToString();
        }

        public static string GenerateWrapperClass(ParsedSheet parsedSheet)
        {
            ValidateParsedSheet(parsedSheet);

            var codeWriter = new CodeWriter();

            codeWriter.AddUsing(CollectionsNamespace);
            codeWriter.WritePartialClass($"{parsedSheet.Name}Wrapper", () =>
            {
                codeWriter.WritePublicProperty("Name", "string");
                codeWriter.WritePublicProperty("Entities", $"List<{parsedSheet.Name}>", result: $"new List<{parsedSheet.Name}>()");
            });

            return codeWriter.ToString();
        }

        private static void AddCollectionsNamespaceIfNecessary(ParsedSheet parsedSheet, CodeWriter codeWriter)
        {
            string[] collectionTypes = { "List", "HashSet", "Dictionary" };

            if (!parsedSheet.Properties.Any(property =>
                    collectionTypes.Any(type =>
                        property.Type.StartsWith(type, StringComparison.Ordinal) ||
                        property.Type.StartsWith($"System.Collections.Generic.{type}", StringComparison.Ordinal))))
            {
                return;
            }

            codeWriter.AddUsing(CollectionsNamespace);
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