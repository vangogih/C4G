using System.Linq;
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
        private readonly CodeGeneration _codeGeneration;
        private readonly SheetsParsing.SheetsParsing _sheetsParsing;
        private readonly ConfigsSerialization _configsSerialization;
        private readonly IO.IO _io;

        private GoogleInteraction.GoogleInteraction _googleInteraction;

        public C4GFacade(IC4GSettings settings)
        {
            _settings = settings;
            _codeGeneration = new CodeGeneration();
            _sheetsParsing = new SheetsParsing.SheetsParsing();
            _configsSerialization = new ConfigsSerialization();
            _io = new IO.IO();
        }

        public async Task<Result<string>> RunAsync(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return Result<string>.FromError("C4G Error. Task cancelled");

            if (_settings.SheetConfigurations == null || _settings.SheetConfigurations.Count == 0)
                return Result<string>.FromError(
                    "C4G Error. No sheet configurations defined. Please add at least one sheet with a parser in settings.");

            var firstSheet = _settings.SheetConfigurations.First();
            string sheetName = firstSheet.Key;
            SheetParserBase parser = firstSheet.Value;

            _googleInteraction =
                new GoogleInteraction.GoogleInteraction(_settings.TableId, sheetName, _settings.ClientSecret);

            var rawConfigsResult = await _googleInteraction.LoadRawConfigAsync(ct);
            if (ct.IsCancellationRequested)
                return Result<string>.FromError("C4G Error. Task cancelled");
            if (!rawConfigsResult.IsOk)
                return rawConfigsResult.WithoutValue();

            var sheetParsingResult = _sheetsParsing.ParseSheet(sheetName, rawConfigsResult.Value, parser);
            if (!sheetParsingResult.IsOk)
                return sheetParsingResult.WithoutValue();

            var dtoClassGenerationResult = _codeGeneration.GenerateDTOClass(sheetParsingResult.Value);
            if (!dtoClassGenerationResult.IsOk)
                return dtoClassGenerationResult.WithoutValue();

            var wrapperClassGenerationResult = _codeGeneration.GenerateWrapperClass(sheetParsingResult.Value);
            if (!wrapperClassGenerationResult.IsOk)
                return wrapperClassGenerationResult.WithoutValue();

            var jsonGenerationResult = _configsSerialization.Serialize(sheetParsingResult.Value);
            if (!jsonGenerationResult.IsOk)
                return jsonGenerationResult.WithoutValue();

            var writeToFilesResult = _io.WriteToFiles(
                _settings.GeneratedCodeFolderFullPath,
                $"{sheetParsingResult.Value.Name}.cs",
                dtoClassGenerationResult.Value,
                $"{sheetParsingResult.Value.Name}Wrapper.cs",
                wrapperClassGenerationResult.Value,
                _settings.SerializedConfigsFolderFullPath,
                $"{sheetParsingResult.Value.Name}.json",
                jsonGenerationResult.Value);
            if (!writeToFilesResult.IsOk)
                return writeToFilesResult;

            return Result<string>.Ok;
        }
    }
}