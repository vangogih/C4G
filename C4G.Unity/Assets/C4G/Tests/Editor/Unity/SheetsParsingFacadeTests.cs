using System.Collections.Generic;
using C4G.Core;
using C4G.Core.SheetsParsing;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity
{
    public class SheetsParsingFacadeTests
    {
        private SheetsParsingFacade _sheetsParsingFacade;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sheetsParsingFacade = new SheetsParsingFacade();
        }

        [Test]
        public void ParseSheet_UsualCase()
        {
            // Arrange
            var sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "C4G_NAME", "C4G_TYPE" },
                new List<object> { "Id", "int", "1", "2", "3" },
                new List<object> { "Name", "string", "Alice", "Bob", "Charlie" },
                new List<object> { "Rank", "float", "5.3", "6.6", "3.5" },
                new List<object> { "Range", "double", "8.5", "35.1", "33" }
            };

            var expectedProperties = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("Id", "int"),
                new ParsedPropertyInfo("Name", "string"),
                new ParsedPropertyInfo("Rank", "float"),
                new ParsedPropertyInfo("Range", "double")
            };
            var expectedEntities = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "1", "Alice", "5.3", "8.5" },
                new List<string> { "2", "Bob", "6.6", "35.1" },
                new List<string> { "3", "Charlie", "3.5", "33" }
            };

            // Act
            var result = _sheetsParsingFacade.ParseSheet(sheetName, sheetData);

            // Assert
            Assert.IsTrue(result.IsOk);
            var parsedSheet = result.Value;
            Assert.AreEqual(sheetName, parsedSheet.Name);
            CollectionAssert.AreEqual(expectedProperties, parsedSheet.Properties);
            CollectionAssert.AreEqual(expectedEntities, parsedSheet.Entities);
        }

        [Test]
        public void ParseSheet_InvalidHeader_ReturnsError()
        {
            // Arrange
            var sheetName = "TestSheet";
            var invalidHeaderData = new List<IList<object>>
            {
                new List<object> { "INVALID_NAME", "C4G_TYPE" },
                new List<object> { "Id", "int", "1" }
            };

            // Act
            var result = _sheetsParsingFacade.ParseSheet(sheetName, invalidHeaderData);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.AreEqual(EC4GError.SP_FirstHeaderInvalid, result.Error);
        }

        [Test]
        public void ParseSheet_InvalidDataLength_ReturnsError()
        {
            // Arrange
            var sheetName = "TestSheet";
            var invalidData = new List<IList<object>>
            {
                new List<object> { "C4G_NAME", "C4G_TYPE" },
                new List<object> { "Id", "int", "1" },
                new List<object> { "Name", "string" } // Data row length mismatch
            };

            // Act
            var result = _sheetsParsingFacade.ParseSheet(sheetName, invalidData);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.AreEqual(EC4GError.SP_DataRowElementsCountInvalid, result.Error);
        }

        [Test]
        public void ParseSheet_NullSheetName_ReturnsError()
        {
            // Arrange
            string sheetName = null;
            var sheetData = new List<IList<object>>
            {
                new List<object> { "C4G_NAME", "C4G_TYPE" },
                new List<object> { "Id", "int", "1" }
            };

            // Act
            var result = _sheetsParsingFacade.ParseSheet(sheetName, sheetData);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.AreEqual(EC4GError.SP_SheetNameNullOrEmpty, result.Error);
        }

        [Test]
        public void ParseSheet_NullSheetData_ReturnsError()
        {
            // Arrange
            var sheetName = "TestSheet";
            IList<IList<object>> sheetData = null;

            // Act
            var result = _sheetsParsingFacade.ParseSheet(sheetName, sheetData);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.AreEqual(EC4GError.SP_SheetDataNull, result.Error);
        }

        [Test]
        public void ParseSheet_EmptySheet_ReturnsError()
        {
            // Arrange
            var sheetName = "TestSheet";
            var emptySheetData = new List<IList<object>>();

            // Act
            var result = _sheetsParsingFacade.ParseSheet(sheetName, emptySheetData);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.AreEqual(EC4GError.SP_SheetDataCountLowerThanTwo, result.Error);
        }
    }
}