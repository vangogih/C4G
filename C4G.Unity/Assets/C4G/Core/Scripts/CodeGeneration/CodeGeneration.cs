using System.Collections.Generic;
using C4G.Core.ConfigsSerialization;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;

namespace C4G.Editor
{
    public sealed class CodeGeneration
    {
        private readonly CodeWriter _codeWriter = new CodeWriter("    ");
        private readonly IReadOnlyList<AliasDefinition> _aliasDefinitions;

        public CodeGeneration(IReadOnlyList<AliasDefinition> aliasDefinitions)
        {
            _aliasDefinitions = aliasDefinitions;
        }

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
                    for (int propertyIndex = 0; propertyIndex < parsedSheet.Properties.Count; propertyIndex++)
                    {
                        ParsedPropertyInfo property = parsedSheet.Properties[propertyIndex];
                        string actualType = ResolveType(property.Type);
                        w.WritePublicProperty(property.Name, actualType);
                    }
                });

            string generatedClass = _codeWriter.Build();

            return Result<string, string>.FromValue(generatedClass);
        }

        public Result<string, string> GenerateRootConfigClass(string name, List<ParsedSheet> parsedSheets)
        {
            _codeWriter.Clear();

            _codeWriter
                .AddUsing("System.Collections.Generic")
                .WritePublicClass(name: name, isPartial: true, baseClass: string.Empty, w =>
                {
                    for (int sheetIndex = 0; sheetIndex < parsedSheets.Count; sheetIndex++)
                    {
                        ParsedSheet parsedSheet = parsedSheets[sheetIndex];
                        w.WritePublicProperty(parsedSheet.Name, $"List<{parsedSheet.Name}>", $"new List<{parsedSheet.Name}>()");
                    }
                });

            string generatedClass = _codeWriter.Build();

            return Result<string, string>.FromValue(generatedClass);
        }

        private bool TryGetAliasDefinition(string alias, out AliasDefinition definition)
        {
            foreach (var aliasDef in _aliasDefinitions)
            {
                if (aliasDef.Alias.Equals(alias, System.StringComparison.OrdinalIgnoreCase))
                {
                    definition = aliasDef;
                    return true;
                }
            }
            
            definition = default;
            return false;
        }

        private string ResolveType(string type)
        {
            if (TryGetAliasDefinition(type, out AliasDefinition aliasDefinition))
            {
                // TODO handle cases when null being returned
                return aliasDefinition.UnderlyingType.FullName;
            }

            return type;
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