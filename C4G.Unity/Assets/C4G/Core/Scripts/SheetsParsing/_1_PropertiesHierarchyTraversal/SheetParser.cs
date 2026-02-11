using System.IO.Pipelines;
using C4G.Core.Utils;

namespace C4G.Core.Scripts.SheetsParsing._1_PropertiesHierarchyTraversal
{
    public sealed class SheetParser
    {
        public enum EPropertyNestingType : byte
        {
            NotNested = 0,
            Nested = 1
        }

        public readonly struct ParsedProperty
        {
            public string 
        }

        public Result<string> ParseIncludingNestedPropertiesNonAlloc(string sheetName, List<RawParsedConfig> rawParsedConfigs, List<ParsedConfig> parsedConfigs)
        {
            for (int i = 0; i < rawParsedConfigs.Count; i++)
            {
                RawParsedConfig rawParsedConfig = rawParsedConfigs[i];
                IReadOnlyList<RawParsedProperty> rawParsedConfigProperties = rawParsedConfig.Properties;
                int notNestedPropertiesAmount = 0;
                int nestedPropertiesAmount = 0;
                EPropertyNestingType[] propertiesNestingTypes = new EPropertyNestingType[rawParsedConfigProperties.Count];
                string[] propertiesNames = new string[rawParsedConfigProperties.Count];
                for (int j = 0; j < rawParsedConfigProperties.Count; j++)
                {
                    RawParsedProperty rawParsedProperty = rawParsedConfigProperties[j];
                    NestingCheckResultAndDotIndex nestingCheckResultAndDotIndex = NestingCheckResultAndDotIndex(rawParsedProperty.NameWithPossibleHierarchy);
                    if (!nestingCheckResultAndDotIndex.NestingCheckResult.IsValid())
                        return Result.WithError($"C4G Error. Sheet name '{sheetName}'. Property '{rawParsedProperty.NameWithPossibleHierarchy}' is invalid '{nestingCheckResultAndDotIndex.NestingCheckResult}'");
                    if (nestingCheckResultAndDotIndex.NestingCheckResult == ERawParsedPropertyNestingCheckResult.NotNested)
                    {
                        ++notNestedPropertiesAmount;
                        propertiesNestingTypes[j] = EPropertyNestingType.NotNested;
                        propertiesNames[j] = rawParsedProperty.NameWithPossibleHierarchy;
                    }
                    else
                    {
                        ++nestedPropertiesAmount;
                        propertiesNestingTypes[j] = EPropertyNestingType.Nested;
                        string nestedPropertyName = rawParsedProperty.NameWithPossibleHierarchy.Substring(nestingCheckResultAndDotIndex.DotIndex + 1);
                        propertiesNames[j] = nestedPropertyName;
                    }
                }

                string[] notNestedPropertiesNames
                

            }
        }

        private static NestingCheckResultAndDotIndex CheckForNestingAndFindDotIndex(string nameWithPossibleHierarchy)
        {
            ERawParsedPropertyNestingCheckResult nestingCheckResult = ERawParsedPropertyNestingCheckResult.NotNested;
            int dotIndex = -1;

            for (int i = 0; i < nameWithPossibleHierarchy.Length; i++)
            {
                if (nameWithPossibleHierarchy[i] != '.')
                    continue;

                if (i == 0)
                    return new NestingCheckResultAndDotIndex(ERawParsedPropertyNestingCheckResult.Invalid_FirstCharIsDot, -1);

                if (i == nameWithPossibleHierarchy.Length - 1)
                    return new NestingCheckResultAndDotIndex(ERawParsedPropertyNestingCheckResult.Invalid_LastCharIsDot, -1);

                if (dotIndex != -1)
                    return new NestingCheckResultAndDotIndex(ERawParsedPropertyNestingCheckResult.Invalid_LastCharIsDot, -1);

                dotIndex = i;
            }
            if (dotIndex == -1)
                return new NestingCheckResultAndDotIndex(ERawParsedPropertyNestingCheckResult.NotNested, -1);
            return new NestingCheckResultAndDotIndex(ERawParsedPropertyNestingCheckResult.NotNested, dotIndex);
        }
    }

    public readonly struct NestingCheckResultAndDotIndex
    {
        public readonly ERawParsedPropertyNestingCheckResult NestingCheckResult;
        public readonly int DotIndex;

        public NestingCheckResultAndDotIndex(ERawParsedPropertyNestingCheckResult nestingCheckResult, int dotIndex)
        {
            NestingCheckResult = nestingCheckResult;
            DotIndex = dotIndex;
        }
    }

    public enum ERawParsedPropertyNestingCheckResult : byte
    {
        NotNested = 0,
        Nested = 1,
        Invalid_FirstCharIsDot = 2,
        Invalid_LastCharIsDot = 3,
        Ivalid_MoreThanOneDot = 4
    }

    public static class ERawParsedPropertyNestingCheckResultExtensions
    {
        public static bool IsValid(this ERawParsedPropertyNestingCheckResult nestedCheckResult)
        {
            return nestedCheckResult is ERawParsedPropertyNestingCheckResult.Nested or ERawParsedPropertyNestingCheckResult.NotNested;
        }
    }
}