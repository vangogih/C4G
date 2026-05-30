using System.Collections.Generic;
using System.IO;
using C4G.Core.ConfigsSerialization;
using C4G.Core.Errors;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;

namespace C4G.Core.Settings
{
    public static class C4GSettingsValidation
    {
        public static Result<C4GSettingsError> ValidateSettings(in C4GSettings settings)
        {
            if (string.IsNullOrEmpty(settings.TableId))
            {
                return Result<C4GSettingsError>.FromError(new C4GSettingsError("Table id is null or empty", null));
            }

            if (string.IsNullOrEmpty(settings.ClientSecret))
            {
                return Result<C4GSettingsError>.FromError(new C4GSettingsError("Client secret is null or empty", null));
            }

            if (string.IsNullOrEmpty(settings.RootConfigName))
            {
                return Result<C4GSettingsError>.FromError(new C4GSettingsError("Root config name is null or empty", null));
            }

            if (string.IsNullOrEmpty(settings.GeneratedCodeFolderFullPath))
            {
                return Result<C4GSettingsError>.FromError(new C4GSettingsError("Generated code folder full path is null or empty", null));
            }

            if (!Directory.Exists(settings.GeneratedCodeFolderFullPath))
            {
                return Result<C4GSettingsError>.FromError(new C4GSettingsError($"Generated code folder '{settings.GeneratedCodeFolderFullPath}' is not exist", null));
            }

            if (string.IsNullOrEmpty(settings.SerializedConfigsFolderFullPath))
            {
                return Result<C4GSettingsError>.FromError(new C4GSettingsError("Serialized configs folder full path is null or empty", null));
            }

            if (!Directory.Exists(settings.SerializedConfigsFolderFullPath))
            {
                return Result<C4GSettingsError>.FromError(new C4GSettingsError($"Serialized configs folder '{settings.SerializedConfigsFolderFullPath}' is not exist", null));
            }

            if (settings.SheetParsersByName == null)
            {
                return Result<C4GSettingsError>.FromError(new C4GSettingsError("Sheet parsers by name is null or empty", null));
            }

            foreach (KeyValuePair<string, SheetParserBase> sheetParserByName in settings.SheetParsersByName)
            {
                if (string.IsNullOrEmpty(sheetParserByName.Key))
                    return Result<C4GSettingsError>.FromError(new C4GSettingsError($"Sheet name is null or empty", null));

                if (sheetParserByName.Value == null)
                    return Result<C4GSettingsError>.FromError(new C4GSettingsError($"Sheet parser for sheet name '{sheetParserByName.Key}' is null", null));
            }

            if (settings.AliasParsersByName == null)
            {
                return Result<C4GSettingsError>.FromError(new C4GSettingsError("Alias parser by name is null or empty", null));
            }

            foreach (KeyValuePair<string, IC4GTypeParser> parserByName in settings.AliasParsersByName)
            {
                if (parserByName.Value == null)
                    return Result<C4GSettingsError>.FromError(new C4GSettingsError($"Alias parser with name '{parserByName.Key}' is null or empty'", null));
            }

            return Result<C4GSettingsError>.Ok;
        }
    }
}