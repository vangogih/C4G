using System;
using System.Collections.Generic;
using C4G.Core.Errors;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    [Serializable]
    public sealed class HorizontalManyConfigsOnOneSheetParser : SheetParserBase
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

                Result<ParsedConfig, C4GSheetsParsingError> parseHorizontalResult = SheetsParsingUtils.ParseHorizontal(
                    configFrame.Name,
                    sheetData,
                    startRowIndex: configFrame.StartRowIndex + 1,
                    startColumnIndex: configFrame.StartColumnIndex + 1,
                    endRowIndex: configFrame.EndRowIndex - 1,
                    endColumnIndex: configFrame.EndColumnIndex - 1);

                if (!parseHorizontalResult.IsOk)
                    return parseHorizontalResult.WithoutValue();

                parsedConfigs.Add(parseHorizontalResult.Value);
            }

            return Result<C4GSheetsParsingError>.Ok;
        }
    }
}