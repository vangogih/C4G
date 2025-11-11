using System;
using C4G.Core.SheetsParsing;
using UnityEngine;

namespace C4G.Editor
{
    [Serializable]
    public class SheetEntry
    {
        public string sheetName = "";

        [SerializeReference] public SheetParserBase parserBase;
    }
}