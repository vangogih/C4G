using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using C4G.Core.CodeGeneration;
using C4G.Core.ConfigsSerialization;
using C4G.Core.Settings;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;

namespace C4G.Core
{
    public sealed class C4GFacade
    {
        private readonly IC4GSettingsProvider _settingsProvider;
        private readonly GoogleInteraction.GoogleInteraction _googleInteraction;
        private readonly CodeGenerator _codeGenerator;
        private readonly SheetsParsing.SheetsParsing _sheetsParsing;
        private readonly ConfigsSerializer _configsSerializer;
        private readonly IO.IO _io;

        public C4GFacade(IC4GSettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            _googleInteraction = new GoogleInteraction.GoogleInteraction();
            _codeGenerator = new CodeGenerator();
            _sheetsParsing = new SheetsParsing.SheetsParsing();
            _configsSerializer = new ConfigsSerializer();
            _io = new IO.IO();
        }

        public async Task<Result<string>> RunAsync(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return Result<string>.FromError("C4G Error. Task cancelled");

            Result<C4GSettings, string> settingsResult = _settingsProvider.GetSettings();
            if (!settingsResult.IsOk)
                return Result<string>.FromError(settingsResult.Error);

            C4GSettings settings = settingsResult.Value;

            var aliasesValidationResult = ValidateAliases(settings);
            if (!aliasesValidationResult.IsOk)
                return aliasesValidationResult;

            int sheetsCount = settings.SheetParsersByName.Count;

            var sheets = new List<(KeyValuePair<string, SheetParserBase> sheetName, IList<IList<object>> sheet)>(sheetsCount);

            foreach (KeyValuePair<string, SheetParserBase> parserByName in settings.SheetParsersByName)
            {
                Result<IList<IList<object>>, string> loadSheetResult = await _googleInteraction.LoadSheetAsync(parserByName.Key, settings.TableId, settings.ClientSecret, ct);
                if (ct.IsCancellationRequested)
                    return Result<string>.FromError("C4G Error. Task cancelled");
                if (!loadSheetResult.IsOk)
                    return loadSheetResult.WithoutValue();
                sheets.Add((sheetName: parserByName, sheet: loadSheetResult.Value));
            }

            var parsedSheets = new List<ParsedSheet>(sheetsCount);

            foreach ((KeyValuePair<string, SheetParserBase> parserByName, IList<IList<object>> sheet) in sheets)
            {
                var sheetParsingResult = _sheetsParsing.ParseSheet(parserByName.Key, sheet, parserByName.Value);
                if (!sheetParsingResult.IsOk)
                    return sheetParsingResult.WithoutValue();

                parsedSheets.Add(sheetParsingResult.Value);

                var dtoClassGenerationResult = _codeGenerator.GenerateDTOClass(sheetParsingResult.Value, settings.AliasParsersByName);
                if (!dtoClassGenerationResult.IsOk)
                    return dtoClassGenerationResult.WithoutValue();

                var writeDtoClassToFileResult = _io.WriteToFile(
                    settings.GeneratedCodeFolderFullPath,
                    $"{sheetParsingResult.Value.Name}.cs",
                    dtoClassGenerationResult.Value);
                if (!writeDtoClassToFileResult.IsOk)
                    return writeDtoClassToFileResult;
            }

            var rootConfigClassGenerationResult = _codeGenerator.GenerateRootConfigClass(settings.RootConfigName, parsedSheets);
            if (!rootConfigClassGenerationResult.IsOk)
                return rootConfigClassGenerationResult.WithoutValue();

            var writeRootConfigClassToFileResult = _io.WriteToFile(
                settings.GeneratedCodeFolderFullPath,
                $"{settings.RootConfigName}.cs",
                rootConfigClassGenerationResult.Value);
            if (!writeRootConfigClassToFileResult.IsOk)
                return writeRootConfigClassToFileResult;

            var serializedConfigSerializationResult = _configsSerializer.SerializeMultipleSheetsAsJsonObject(parsedSheets, settings.AliasParsersByName);
            if (!serializedConfigSerializationResult.IsOk)
                return serializedConfigSerializationResult.WithoutValue();

            var writeSerializedConfigToFileResult = _io.WriteToFile(
                settings.SerializedConfigsFolderFullPath,
                $"{settings.RootConfigName}.json",
                serializedConfigSerializationResult.Value);
            if (!writeSerializedConfigToFileResult.IsOk)
                return writeSerializedConfigToFileResult;

            return Result<string>.Ok;
        }

        private Result<string> ValidateAliases(in C4GSettings settings)
        {
            if (string.IsNullOrEmpty(settings.TableId))
            {
                return Result<string>.FromError("C4G Error. Table id is null or empty");
            }

            if (string.IsNullOrEmpty(settings.ClientSecret))
            {
                return Result<string>.FromError("C4G Error. Client secret is null or empty");
            }

            if (string.IsNullOrEmpty(settings.RootConfigName))
            {
                return Result<string>.FromError("C4G Error. Root config name is null or empty");
            }

            if (string.IsNullOrEmpty(settings.GeneratedCodeFolderFullPath))
            {
                return Result<string>.FromError("C4G Error. Generated code folder full path is null or empty");
            }

            if (!Directory.Exists(settings.GeneratedCodeFolderFullPath))
            {
                return Result<string>.FromError($"C4G Error. Generated code folder '{settings.GeneratedCodeFolderFullPath}' is not exist");
            }

            if (string.IsNullOrEmpty(settings.SerializedConfigsFolderFullPath))
            {
                return Result<string>.FromError("C4G Error. Serialized configs folder full path is null or empty");
            }

            if (!Directory.Exists(settings.SerializedConfigsFolderFullPath))
            {
                return Result<string>.FromError($"C4G Error. Serialized configs folder '{settings.SerializedConfigsFolderFullPath}' is not exist");
            }

            if (settings.SheetParsersByName == null)
            {
                return Result<string>.FromError("C4G Error. Sheet parsers by name is null or empty");
            }

            foreach (KeyValuePair<string, SheetParserBase> sheetParserByName in settings.SheetParsersByName)
            {
                if (string.IsNullOrEmpty(sheetParserByName.Key))
                    return Result<string>.FromError($"C4G Error. Sheet name is null or empty");

                if (sheetParserByName.Value == null)
                    return Result<string>.FromError($"C4G Error. Sheet parser for sheet name '{sheetParserByName.Key}' is null");
            }

            if (settings.AliasParsersByName == null)
            {
                return Result<string>.FromError("C4G Error. Alias parser by name is null or empty");
            }

            foreach (KeyValuePair<string, IC4GTypeParser> parserByName in settings.AliasParsersByName)
            {
                if (parserByName.Value == null)
                    return Result<string>.FromError($"C4G Error. Alias parser with name '{parserByName.Key}' is null or empty'");
            }

            return Result<string>.Ok;
        }
    }
}