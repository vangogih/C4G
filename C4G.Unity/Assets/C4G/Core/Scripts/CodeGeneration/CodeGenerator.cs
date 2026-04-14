using System;
using System.Collections.Generic;
using System.Text;
using C4G.Core.ConfigsSerialization;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;

namespace C4G.Core.CodeGeneration
{
    public sealed class CodeGenerator : ICodeGenerator
    {
        private readonly CodeWriter _codeWriter = new CodeWriter("    ");

        public Result<string, string> GenerateDTOClass(ParsedConfig parsedConfig, IReadOnlyDictionary<string, IC4GTypeParser> aliasParsersByName)
        {
            bool isValid = ValidateParsedConfig(parsedConfig, out string error);
            if (!isValid)
                return Result<string, string>.FromError(error);

            _codeWriter.Clear();

            _codeWriter
                .AddUsing("System.Collections.Generic")
                .WritePublicClass(name: parsedConfig.Name, isPartial: true, baseClass: string.Empty, w =>
                {
                    for (int propertyIndex = 0; propertyIndex < parsedConfig.Properties.Count; propertyIndex++)
                    {
                        ParsedPropertyInfo property = parsedConfig.Properties[propertyIndex];
                        string actualType = ResolveType(property.Type, aliasParsersByName);
                        w.WritePublicProperty(property.Name, actualType);
                    }
                });

            string generatedClass = _codeWriter.Build();

            return Result<string, string>.FromValue(generatedClass);
        }

        public Result<string, string> GenerateRootConfigClass(string name, List<ParsedConfig> parsedConfigs)
        {
            _codeWriter.Clear();

            _codeWriter
                .AddUsing("System.Collections.Generic")
                .WritePublicClass(name: name, isPartial: true, baseClass: string.Empty, w =>
                {
                    for (int configIndex = 0; configIndex < parsedConfigs.Count; configIndex++)
                    {
                        ParsedConfig parsedConfig = parsedConfigs[configIndex];
                        w.WritePublicProperty(parsedConfig.Name, $"List<{parsedConfig.Name}>", $"new List<{parsedConfig.Name}>()");
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
                        result.Append(GetTypeName(currentType));
                        continue;
                    }

                    var genericTypeDef = currentType.GetGenericTypeDefinition();
                    var genericArgs = currentType.GetGenericArguments();

                    var fullName = GetTypeName(genericTypeDef);
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

        private static string GetTypeName(Type type)
        {
            if (type.FullName != null)
                return type.FullName;
            return type.Name;
        }

        private static bool ValidateParsedConfig(ParsedConfig parsedConfig, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrEmpty(parsedConfig.Name))
                error = "Code generation error. ParsedConfig name is null or empty";
            else if (parsedConfig.Properties == null)
                error = "Code generation error. ParsedConfig properties are null";
            else if (parsedConfig.Entities == null)
                error = "Code generation error. ParsedConfig entities are null";

            return string.IsNullOrEmpty(error);
        }
    }
}