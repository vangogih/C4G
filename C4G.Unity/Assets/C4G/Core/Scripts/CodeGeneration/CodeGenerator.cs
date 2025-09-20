using System;
using System.Collections.Generic;
using System.Text;
using C4G.Core.ConfigsSerialization;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;

namespace C4G.Core.CodeGeneration
{
    public sealed class CodeGenerator
    {
        private readonly CodeWriter _codeWriter = new CodeWriter("    ");

        public Result<string, string> GenerateDTOClass(ParsedSheet parsedSheet, IReadOnlyDictionary<string, IC4GTypeParser> aliasParsersByName)
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
                        string actualType = ResolveType(property.Type, aliasParsersByName);
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

        private string ResolveType(string type, IReadOnlyDictionary<string, IC4GTypeParser> aliasParsersByName)
        {
            if (aliasParsersByName.TryGetValue(type, out IC4GTypeParser parser))
            {
                var stack = new Stack<object>();
                var result = new StringBuilder();

                stack.Push(parser.ParsingType);

                while (stack.Count > 0)
                {
                    var current = stack.Pop();

                    if (current is string str)
                    {
                        result.Append(str);
                        continue;
                    }

                    var currentType = (Type)current;

                    if (currentType.IsArray)
                    {
                        var elementType = currentType.GetElementType();
                        var rank = currentType.GetArrayRank();
                        var brackets = rank == 1 ? "[]" : $"[{new string(',', rank - 1)}]";

                        stack.Push(brackets);
                        stack.Push(elementType);
                        continue;
                    }

                    if (!currentType.IsGenericType)
                    {
                        result.Append(currentType.FullName ?? currentType.Name);
                        continue;
                    }

                    var genericTypeDef = currentType.GetGenericTypeDefinition();
                    var genericArgs = currentType.GetGenericArguments();

                    var fullName = genericTypeDef.FullName ?? genericTypeDef.Name;
                    var baseName = fullName.Split('`')[0];

                    stack.Push(">");

                    for (int i = genericArgs.Length - 1; i >= 0; i--)
                    {
                        if (i < genericArgs.Length - 1)
                            stack.Push(", ");
                        stack.Push(genericArgs[i]);
                    }

                    stack.Push("<");
                    stack.Push(baseName);
                }

                return result.ToString();
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