using System;
using System.Collections.Generic;
using System.Linq;
using C4G.Core.ConfigsSerialization;
using UnityEditor;

namespace C4G.Editor
{
    internal static class C4GTypeParserSerializationHelper
    {
        internal static readonly Type[] ParserTypes;
        internal static readonly Dictionary<string, int> TypeIndexByAssemblyQualifiedNameMap;
        internal static readonly string[] ParserTypeNamesWithLeadingNone;

        static C4GTypeParserSerializationHelper()
        {
            ParserTypes = TypeCache.GetTypesDerivedFrom<IC4GTypeParser>()
                .Where(t => !t.IsAbstract && !t.IsGenericType && !t.IsInterface)
                .OrderBy(t => t.Name)
                .ToArray();

            TypeIndexByAssemblyQualifiedNameMap = new Dictionary<string, int>(ParserTypes.Length);
            ParserTypeNamesWithLeadingNone = new string[ParserTypes.Length + 1];
            ParserTypeNamesWithLeadingNone[0] = "None";
            for (int typeIndex = 0; typeIndex < ParserTypes.Length; typeIndex++)
            {
                Type parserType = ParserTypes[typeIndex];
                ParserTypeNamesWithLeadingNone[typeIndex + 1] = parserType.Name;
                TypeIndexByAssemblyQualifiedNameMap.Add(parserType.AssemblyQualifiedName, typeIndex);
            }
        }
    }
}