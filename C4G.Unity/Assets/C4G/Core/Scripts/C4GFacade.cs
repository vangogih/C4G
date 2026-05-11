using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using C4G.Core.CodeGeneration;
using C4G.Core.ConfigsSerialization;
using C4G.Core.Errors;
using C4G.Core.GoogleInteraction;
using C4G.Core.Settings;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;

namespace C4G.Core
{
    public sealed class C4GFacade
    {
        private readonly IC4GSettingsProvider _settingsProvider;
        private readonly IGoogleInteraction _googleInteraction;
        private readonly ICodeGenerator _codeGenerator;
        private readonly SheetsParsingFacade _sheetsParsingFacade;
        private readonly IConfigsSerializer _configsSerializer;
        private readonly IO.IIO _io;

        public C4GFacade(
            IC4GSettingsProvider settingsProvider,
            IGoogleInteraction googleInteraction,
            IO.IIO io,
            ICodeGenerator codeGenerator,
            IConfigsSerializer configsSerializer)
        {
            _settingsProvider = settingsProvider;
            _googleInteraction = googleInteraction;
            _codeGenerator = codeGenerator;
            _sheetsParsingFacade = new SheetsParsingFacade();
            _configsSerializer = configsSerializer;
            _io = io;
        }

        public async Task<Result<C4GErrorBase>> RunAsync(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return Result<C4GErrorBase>.FromError(new C4GCancellationError("C4G run called with already cancelled token", null));

            Result<C4GSettings, C4GSettingsError> getSettingsResult = _settingsProvider.GetSettings();
            if (!getSettingsResult.IsOk)
                return Result<C4GErrorBase>.FromError(getSettingsResult.Error);

            C4GSettings settings = getSettingsResult.Value;

            Result<C4GSettingsError> settingsValidationResult = C4GSettingsValidation.ValidateSettings(settings);
            if (!settingsValidationResult.IsOk)
                return Result<C4GErrorBase>.FromError(settingsValidationResult.Error);

            int sheetsCount = settings.SheetParsersByName.Count;

            var sheets = new List<(KeyValuePair<string, SheetParserBase> sheetName, IList<IList<object>> sheet)>(sheetsCount);

            foreach (KeyValuePair<string, SheetParserBase> parserByName in settings.SheetParsersByName)
            {
                Result<IList<IList<object>>, C4GGoogleInteractionError> loadSheetResult = await _googleInteraction.LoadSheetAsync(parserByName.Key, settings.TableId, settings.ClientSecret, ct);
                if (ct.IsCancellationRequested)
                    return Result<C4GErrorBase>.FromError(new C4GCancellationError("C4G run cancelled by token", null));
                if (!loadSheetResult.IsOk)
                    return Result<C4GErrorBase>.FromError(loadSheetResult.Error);
                sheets.Add((sheetName: parserByName, sheet: loadSheetResult.Value));
            }

            var parsedConfigs = new List<ParsedConfig>(sheetsCount);
            var cycleParsedConfigsBuffer = new List<ParsedConfig>(sheetsCount);

            foreach ((KeyValuePair<string, SheetParserBase> parserByName, IList<IList<object>> sheet) in sheets)
            {
                cycleParsedConfigsBuffer.Clear();

                Result<C4GSheetsParsingError> sheetParsingResult = _sheetsParsingFacade.ParseSheetToList(parserByName.Key, sheet, parserByName.Value, cycleParsedConfigsBuffer);
                if (!sheetParsingResult.IsOk)
                    return Result<C4GErrorBase>.FromError(sheetParsingResult.Error);

                parsedConfigs.AddRange(cycleParsedConfigsBuffer);
            }

            foreach (ParsedConfig parsedConfig in parsedConfigs)
            {
                Result<string, C4GCodeGenerationError> dtoClassGenerationResult = _codeGenerator.GenerateDTOClass(parsedConfig, settings.AliasParsersByName);
                if (!dtoClassGenerationResult.IsOk)
                    return Result<C4GErrorBase>.FromError(dtoClassGenerationResult.Error);

                Result<C4GIOError> writeDtoClassToFileResult = _io.WriteToFile(
                    settings.GeneratedCodeFolderFullPath,
                    $"{parsedConfig.Name}.cs",
                    dtoClassGenerationResult.Value);

                if (!writeDtoClassToFileResult.IsOk)
                    return Result<C4GErrorBase>.FromError(writeDtoClassToFileResult.Error);
            }

            Result<string, C4GCodeGenerationError> rootConfigClassGenerationResult = _codeGenerator.GenerateRootConfigClass(settings.RootConfigName, parsedConfigs);
            if (!rootConfigClassGenerationResult.IsOk)
                return Result<C4GErrorBase>.FromError(rootConfigClassGenerationResult.Error);

            Result<C4GIOError> writeRootConfigClassToFileResult = _io.WriteToFile(
                settings.GeneratedCodeFolderFullPath,
                $"{settings.RootConfigName}.cs",
                rootConfigClassGenerationResult.Value);

            if (!writeRootConfigClassToFileResult.IsOk)
                return Result<C4GErrorBase>.FromError(writeRootConfigClassToFileResult.Error);

            Result<string, C4GConfigsSerializationError> serializedConfigSerializationResult = _configsSerializer.SerializeParsedConfigsAsJsonObject(
                parsedConfigs,
                settings.AliasParsersByName);

            if (!serializedConfigSerializationResult.IsOk)
                return Result<C4GErrorBase>.FromError(serializedConfigSerializationResult.Error);

            Result<C4GIOError> writeSerializedConfigToFileResult = _io.WriteToFile(
                settings.SerializedConfigsFolderFullPath,
                $"{settings.RootConfigName}.json",
                serializedConfigSerializationResult.Value);

            if (!writeSerializedConfigToFileResult.IsOk)
                return Result<C4GErrorBase>.FromError(writeSerializedConfigToFileResult.Error);

            return Result<C4GErrorBase>.Ok;
        }
    }
}