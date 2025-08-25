namespace C4G.Core
{
    public enum EC4GError
    {
        None = 0,

        // C4GFacade 1 - 99
        TaskCancelled = 1,
        IOException = 2,

        // SheetsParsing 100-199
        SP_SheetNameNullOrEmpty = 100,
        SP_SheetDataNull = 101,
        SP_SheetDataCountLowerThanTwo = 102,
        SP_HeadersRowNull = 103,
        SP_HeadersRowElementsCountIsNotTwo = 104,
        SP_FirstHeaderInvalid = 105,
        SP_SecondHeaderInvalid = 106,
        SP_FirstDataRowNull = 107,
        SP_FirstDataRowElementsCountLowerThanTwo = 108,
        SP_DataRowNull = 109,
        SP_DataRowElementsCountInvalid = 110,

        // CodeGeneration 200-299
        CG_ParsedSheetNameNullOrEmpty = 200,
        CG_ParsedSheetPropertiesNull = 201,
        CG_ParsedSheetEntitiesNull = 202,

        // JsonGeneration 300-399
        CS_ParsedSheetNameNullOrEmpty = 300,
        CS_ParsedSheetPropertiesNull = 301,
        CS_ParsedSheetEntitiesNull = 302,
        CS_ParsedSheetDuplicatedPropertyName = 303,
        CS_ParsedSheetMismatchedEntitiesCount = 304
    }
}