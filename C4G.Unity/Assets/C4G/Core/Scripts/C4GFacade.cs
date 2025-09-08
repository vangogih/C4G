using System.Threading;
using System.Threading.Tasks;
using C4G.Core.GoogleInteraction;
using C4G.Core.IO;
using C4G.Core.Settings;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using C4G.Editor;

namespace C4G.Core
{
    public sealed class C4GFacade
    {
        private readonly IC4GSettingsFacade _settingsFacade;
        private readonly GoogleInteractionFacade _googleInteractionFacade;
        private readonly CodeGenerationFacade _codeGenerationFacade;
        private readonly SheetsParsingFacade _sheetsParsingFacade;
        private readonly ConfigsSerializationFacade _configsSerializationFacade;
        private readonly IOFacade _ioFacade;

        public C4GFacade(IC4GSettingsFacade settingsFacade)
        {
            _settingsFacade = settingsFacade;
            _googleInteractionFacade = new GoogleInteractionFacade(_settingsFacade.TableId, _settingsFacade.SheetName, _settingsFacade.ClientSecret);
            _codeGenerationFacade = new CodeGenerationFacade();
            _sheetsParsingFacade = new SheetsParsingFacade();
            _configsSerializationFacade = new ConfigsSerializationFacade();
            _ioFacade = new IOFacade();
        }

        public async Task<Result<C4GError>> RunAsync(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return Result<C4GError>.FromError(C4GError.TaskCancelled);

            var rawConfigsResult = await _googleInteractionFacade.LoadRawConfigAsync(ct);
            if (ct.IsCancellationRequested)
                return Result<C4GError>.FromError(C4GError.TaskCancelled);
            if (!rawConfigsResult.IsOk)
                return rawConfigsResult.WithoutValue();

            var sheetParsingResult = _sheetsParsingFacade.ParseSheet(_settingsFacade.SheetName, rawConfigsResult.Value);
            if (!sheetParsingResult.IsOk)
                return sheetParsingResult.WithoutValue();

            var dtoClassGenerationResult = _codeGenerationFacade.GenerateDTOClass(sheetParsingResult.Value);
            if (!dtoClassGenerationResult.IsOk)
                return dtoClassGenerationResult.WithoutValue();

            var wrapperClassGenerationResult = _codeGenerationFacade.GenerateWrapperClass(sheetParsingResult.Value);
            if (!wrapperClassGenerationResult.IsOk)
                return wrapperClassGenerationResult.WithoutValue();

            var jsonGenerationResult = _configsSerializationFacade.Serialize(sheetParsingResult.Value);
            if (!jsonGenerationResult.IsOk)
                return jsonGenerationResult.WithoutValue();

            var writeToFilesResult = _ioFacade.WriteToFiles(
                _settingsFacade.GeneratedCodeFolderFullPath,
                $"{sheetParsingResult.Value.Name}.cs",
                dtoClassGenerationResult.Value,
                $"{sheetParsingResult.Value.Name}Wrapper.cs",
                wrapperClassGenerationResult.Value,
                _settingsFacade.SerializedConfigsFolderFullPath,
                $"{sheetParsingResult.Value.Name}.json",
                jsonGenerationResult.Value);
            if (!writeToFilesResult.IsOk)
                return writeToFilesResult;

            return Result<EC4GError>.Ok;
        }
    }
}