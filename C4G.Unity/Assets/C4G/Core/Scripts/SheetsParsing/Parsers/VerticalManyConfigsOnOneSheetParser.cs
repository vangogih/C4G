using System;
using System.Collections.Generic;
using C4G.Core.Errors;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    [Serializable]
    public sealed class VerticalManyConfigsOnOneSheetParser : SheetParserBase
    {
        public override Result<C4GSheetsParsingError> ParseToList(string sheetName, IList<IList<object>> sheetData, List<ParsedConfig> parsedConfigs)
        {
            Result<List<ConfigFrame>, C4GSheetsParsingError> parseConfigFramesResult = SheetsParsingUtils.ParseConfigFrames(sheetName, sheetData);
            if (!parseConfigFramesResult.IsOk)
                return parseConfigFramesResult.WithoutValue();

            List<ConfigFrame> configFrames = parseConfigFramesResult.Value;

            for (int sheetIndex = 0; sheetIndex < configFrames.Count; sheetIndex++)
            {
                ConfigFrame configFrame = configFrames[sheetIndex];

                Result<ParsedConfig, C4GSheetsParsingError> parseVerticalResult = SheetsParsingUtils.ParseVertical(
                    configFrame.Name,
                    sheetData,
                    startRowIndex: configFrame.StartRowIndex + 1,
                    startColumnIndex: configFrame.StartColumnIndex + 1,
                    endRowIndex: configFrame.EndRowIndex - 1,
                    endColumnIndex: configFrame.EndColumnIndex - 1);

                if (!parseVerticalResult.IsOk)
                    return parseVerticalResult.WithoutValue();

                parsedConfigs.Add(parseVerticalResult.Value);
            }

            return Result<C4GSheetsParsingError>.Ok;
        }
    }
}