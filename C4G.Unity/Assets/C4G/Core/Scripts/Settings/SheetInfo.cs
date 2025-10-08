namespace C4G.Core.Settings
{
    [System.Serializable]
    public class SheetInfo
    {
        public string sheetName;
        public ParsingType parsingType;
        
        public SheetInfo(string sheetName, ParsingType parsingType)
        {
            this.sheetName = sheetName;
            this.parsingType = parsingType;
        }
    }
}

