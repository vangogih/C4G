using System;
using System.Collections;
using System.Collections.Generic;
using C4G.Editor;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity
{
    public class SheetParserTests
    {
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
            var parsedSheet = SheetParser.ParseSheet(sheetName, sheetData);

            // Assert
            Assert.AreEqual(sheetName, parsedSheet.Name);
            CollectionAssert.AreEqual(expectedProperties, parsedSheet.Properties, new ParsedPropertyInfoComparer());
            CollectionAssert.AreEqual(expectedEntities, parsedSheet.Entities);
        }

        [Test]
        public void ParseSheet_InvalidHeader_ThrowsException()
        {
            // Arrange
            var sheetName = "TestSheet";
            var invalidHeaderData = new List<IList<object>>
            {
                new List<object> { "INVALID_NAME", "C4G_TYPE" },
                new List<object> { "Id", "int", "1" }
            };

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => SheetParser.ParseSheet(sheetName, invalidHeaderData));
            Assert.AreEqual("Expected 'C4G_NAME' as first header, but got 'INVALID_NAME'", ex.Message);
        }

        [Test]
        public void ParseSheet_InvalidDataLength_ThrowsException()
        {
            // Arrange
            var sheetName = "TestSheet";
            var invalidData = new List<IList<object>>
            {
                new List<object> { "C4G_NAME", "C4G_TYPE" },
                new List<object> { "Id", "int", "1" },
                new List<object> { "Name", "string" } // Data row length mismatch
            };

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => SheetParser.ParseSheet(sheetName, invalidData));
            Assert.AreEqual("Non uniform data row with length '2' found at index '2'.Expected length as the first data row at index '1' - '3'", ex.Message);
        }

        [Test]
        public void ParseSheet_NullSheetName_ThrowsException()
        {
            // Arrange
            string sheetName = null;
            var sheetData = new List<IList<object>>
            {
                new List<object> { "C4G_NAME", "C4G_TYPE" },
                new List<object> { "Id", "int", "1" }
            };

            // Act & Assert
            var ex = Assert.Throws<NullReferenceException>(() => SheetParser.ParseSheet(sheetName, sheetData));
            Assert.AreEqual("Parameter 'sheetName' is null or empty", ex.Message);
        }

        [Test]
        public void ParseSheet_NullSheetData_ThrowsException()
        {
            // Arrange
            var sheetName = "TestSheet";
            IList<IList<object>> sheetData = null;

            // Act & Assert
            var ex = Assert.Throws<NullReferenceException>(() => SheetParser.ParseSheet(sheetName, sheetData));
            Assert.AreEqual("Parameter 'sheetData' is null", ex.Message);
        }

        [Test]
        public void ParseSheet_EmptySheet_ThrowsException()
        {
            // Arrange
            var sheetName = "TestSheet";
            var emptySheetData = new List<IList<object>>();

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => SheetParser.ParseSheet(sheetName, emptySheetData));
            Assert.AreEqual($"At least two rows required in a sheet '{sheetName}'", ex.Message);
        }

        private sealed class ParsedPropertyInfoComparer : IComparer, IComparer<ParsedPropertyInfo>, IEqualityComparer<ParsedPropertyInfo>
        {
            public int Compare(object x, object y)
            {
                if (x is ParsedPropertyInfo first && y is ParsedPropertyInfo second)
                {
                    return Compare(first, second);
                }
                throw new ArgumentException("Both objects must be of type ParsedPropertyInfo");
            }

            public int Compare(ParsedPropertyInfo x, ParsedPropertyInfo y)
            {
                if (x == null && y == null) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int nameComparison = string.Compare(x.Name, y.Name, StringComparison.Ordinal);
                if (nameComparison != 0)
                    return nameComparison;

                return string.Compare(x.Type, y.Type, StringComparison.Ordinal);
            }

            public bool Equals(ParsedPropertyInfo x, ParsedPropertyInfo y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                return string.Equals(x.Name, y.Name, StringComparison.Ordinal) &&
                       string.Equals(x.Type, y.Type, StringComparison.Ordinal);
            }

            public int GetHashCode(ParsedPropertyInfo obj)
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + (obj.Name?.GetHashCode() ?? 0);
                    hash = hash * 23 + (obj.Type?.GetHashCode() ?? 0);
                    return hash;
                }
            }
        }
    }
}
