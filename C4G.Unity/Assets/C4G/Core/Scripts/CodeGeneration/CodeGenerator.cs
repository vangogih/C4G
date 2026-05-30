using System;
using System.Collections.Generic;
using System.Text;
using C4G.Core.ConfigsSerialization;
using C4G.Core.Errors;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;

namespace C4G.Core.CodeGeneration
{
    public sealed class CodeGenerator : ICodeGenerator
    {
        private readonly CodeWriter _codeWriter = new CodeWriter("    ");
        private readonly List<string> _resolvedTypes = new(); 

        public Result<string, C4GCodeGenerationError> GenerateDTOClass(ParsedConfig parsedConfig, IReadOnlyDictionary<string, IC4GTypeParser> aliasParsersByName)
        {
            bool isValid = ValidateParsedConfig(parsedConfig, out string error);
            if (!isValid)
                return Result<string, C4GCodeGenerationError>.FromError(new C4GCodeGenerationError(error, null));

            _codeWriter.Clear();
            _resolvedTypes.Clear();

            for (int i = 0; i < parsedConfig.Properties.Length; i++)
            {
                ParsedPropertyInfo property = parsedConfig.Properties[i];
                string actualType = ResolveType(property.Type, aliasParsersByName, out bool isAlias);
                if (isAlias && !string.Equals(property.Type, actualType, StringComparison.Ordinal))
                {
                    _codeWriter.AddUsing($"{property.Type} = {actualType}");
                    _resolvedTypes.Add(property.Type);
                }
                else
                {
                    _resolvedTypes.Add(actualType);
                }
            }

            _codeWriter
                .AddUsing("System.Collections.Generic")
                .WritePublicClass(name: parsedConfig.Name, isPartial: true, baseClass: string.Empty, w =>
                {
                    for (int i = 0; i < parsedConfig.SubTypes.Count; i++)
                    {
                        string subType = parsedConfig.SubTypes[i];
                        w.WritePublicClass(name: subType, isPartial: true, baseClass: string.Empty, w1 =>
                        {
                            for (int j = 0; j < parsedConfig.Properties.Length; j++)
                            {
                                ParsedPropertyInfo property = parsedConfig.Properties[j];
                                if (property.SubTypeIndex == i)
                                {
                                    string actualType = _resolvedTypes[j];
                                    w1.WritePublicProperty(property.Name, actualType);
                                }
                            }
                        });
                    }
                    for (int i = 0; i < parsedConfig.Properties.Length; i++)
                    {
                        ParsedPropertyInfo property = parsedConfig.Properties[i];
                        if (property.SubTypeIndex < 0)
                        {
                            string actualType = _resolvedTypes[i];
                            w.WritePublicProperty(property.Name, actualType);
                        }
                    }
                    for (int i = 0; i < parsedConfig.SubTypes.Count; i++)
                    {
                        string subType = parsedConfig.SubTypes[i];
                        w.WritePublicProperty($"{subType}_Instance", subType);
                    }
                });

            string generatedClass = _codeWriter.Build();

            return Result<string, C4GCodeGenerationError>.FromValue(generatedClass);
        }

        public Result<string, C4GCodeGenerationError> GenerateRootConfigClass(string name, List<ParsedConfig> parsedConfigs)
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

            return Result<string, C4GCodeGenerationError>.FromValue(generatedClass);
        }

        private string ResolveType(string type, IReadOnlyDictionary<string, IC4GTypeParser> aliasParsersByName, out bool isAlias)
        {
            isAlias = false;

            if (aliasParsersByName.TryGetValue(type, out IC4GTypeParser parser))
            {
                isAlias = true;

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
                error = "ParsedConfig name is null or empty";
            else if (parsedConfig.Properties == null)
                error = "ParsedConfig properties are null";
            else if (parsedConfig.Entities == null)
                error = "ParsedConfig entities are null";

            return string.IsNullOrEmpty(error);
        }
    }
}