using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using C4G.Core.Settings;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using C4G.Editor;

namespace C4G.Core
{
    public sealed class C4GFacade
    {
        private readonly IC4GSettings _settings;
        private readonly GoogleInteraction.GoogleInteraction _googleInteraction;
        private readonly CodeGeneration _codeGeneration;
        private readonly SheetsParsing.SheetsParsing _sheetsParsing;
        private readonly ConfigsSerialization _configsSerialization;
        private readonly IO.IO _io;

        public C4GFacade(IC4GSettings settings)
        {
            _settings = settings;
            _googleInteraction = new GoogleInteraction.GoogleInteraction(_settings.TableId, _settings.ClientSecret);
            _codeGeneration = new CodeGeneration();
            _sheetsParsing = new SheetsParsing.SheetsParsing();
            _configsSerialization = new ConfigsSerialization();
            _io = new IO.IO();
        }

        public async Task<Result<string>> RunAsync(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return Result<string>.FromError("C4G Error. Task cancelled");

            var sheets = new List<(string sheetName, IList<IList<object>> sheet)>(_settings.SheetNames.Count);

            foreach (string sheetName in _settings.SheetNames)
            {
                Result<IList<IList<object>>, string> loadSheetResult = await _googleInteraction.LoadSheetAsync(sheetName, ct);
                if (ct.IsCancellationRequested)
                    return Result<string>.FromError("C4G Error. Task cancelled");
                if (!loadSheetResult.IsOk)
                    return loadSheetResult.WithoutValue();
                sheets.Add((sheetName: sheetName, sheet: loadSheetResult.Value));
            }

            var parsedSheets = new List<ParsedSheet>(_settings.SheetNames.Count);

            foreach ((string sheetName, IList<IList<object>> sheet) in sheets)
            {
                var sheetParsingResult = _sheetsParsing.ParseSheet(sheetName, sheet);
                if (!sheetParsingResult.IsOk)
                    return sheetParsingResult.WithoutValue();

                parsedSheets.Add(sheetParsingResult.Value);

                var dtoClassGenerationResult = _codeGeneration.GenerateDTOClass(sheetParsingResult.Value);
                if (!dtoClassGenerationResult.IsOk)
                    return dtoClassGenerationResult.WithoutValue();

                var writeDtoClassToFileResult = _io.WriteToFile(
                    _settings.GeneratedCodeFolderFullPath,
                    $"{sheetParsingResult.Value.Name}.cs",
                    dtoClassGenerationResult.Value);
                if (!writeDtoClassToFileResult.IsOk)
                    return writeDtoClassToFileResult;
            }

            var rootConfigClassGenerationResult = _codeGeneration.GenerateRootConfigClass(_settings.RootConfigName, parsedSheets);
            if (!rootConfigClassGenerationResult.IsOk)
                return rootConfigClassGenerationResult.WithoutValue();

            var writeRootConfigClassToFileResult = _io.WriteToFile(
                _settings.GeneratedCodeFolderFullPath,
                $"{_settings.RootConfigName}.cs",
                rootConfigClassGenerationResult.Value);
            if (!writeRootConfigClassToFileResult.IsOk)
                return writeRootConfigClassToFileResult;

            var serializedConfigSerializationResult = _configsSerialization.SerializeMultipleSheetsAsJsonObject(parsedSheets);
            if (!serializedConfigSerializationResult.IsOk)
                return serializedConfigSerializationResult.WithoutValue();

            var writeSerializedConfigToFileResult = _io.WriteToFile(
                _settings.SerializedConfigsFolderFullPath,
                $"{_settings.RootConfigName}.json",
                serializedConfigSerializationResult.Value);
            if (!writeSerializedConfigToFileResult.IsOk)
                return writeSerializedConfigToFileResult;

            return Result<string>.Ok;
        }
    }
}