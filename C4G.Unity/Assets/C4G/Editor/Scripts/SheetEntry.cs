using System;
using C4G.Core.Settings;
using C4G.Core.SheetsParsing;
using UnityEngine;

namespace C4G.Editor
{
    [Serializable]
    public class SheetEntry
    {
        public SheetInfo sheetInfo = new SheetInfo("");
        
        [SerializeReference]
        public SheetParserBase parserBase;
    }
}
