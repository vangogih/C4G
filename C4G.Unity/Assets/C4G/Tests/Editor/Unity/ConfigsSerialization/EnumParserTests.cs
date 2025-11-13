using System;
using C4G.Core.ConfigsSerialization;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.ConfigsSerialization
{
    public class EnumParserTests
    {
        private TestRarityParser _rarityParser;
        private TestFlagsParser _flagsParser;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _rarityParser = new TestRarityParser();
            _flagsParser = new TestFlagsParser();
        }

        [Test]
        public void Parse_ValidEnumName_ReturnsEnumValue()
        {
            // Arrange
            IC4GTypeParser parser = _rarityParser;

            // Act
            Result<object, string> result = parser.Parse("Rare");

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(TestRarity.Rare, result.Value);
        }

        [Test]
        public void Parse_UndefinedNumericValue_ReturnsError()
        {
            // Arrange
            IC4GTypeParser parser = _rarityParser;

            // Act
            Result<object, string> result = parser.Parse("999");

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.That(result.Error, Does.Contain("is not defined in enum"));
        }

        [Test]
        public void Parse_InvalidEnumName_ReturnsError()
        {
            // Arrange
            IC4GTypeParser parser = _rarityParser;

            // Act
            Result<object, string> result = parser.Parse("InvalidValue");

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.That(result.Error, Does.Contain("Exception during enum parsing"));
        }

        [Test]
        public void ParsingType_ReturnsCorrectType()
        {
            // Arrange
            IC4GTypeParser parser = _rarityParser;

            // Act
            Type parsingType = parser.ParsingType;

            // Assert
            Assert.AreEqual(typeof(TestRarity), parsingType);
        }

        [Test]
        public void Parse_ValidFlagsCombination_ReturnsEnumValue()
        {
            // Arrange
            IC4GTypeParser parser = _flagsParser;

            // Act
            Result<object, string> result = parser.Parse("Flag1, Flag2");

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(TestFlags.Flag1 | TestFlags.Flag2, result.Value);
        }

        [Test]
        public void Parse_UndefinedFlagsCombination_ReturnsSuccess()
        {
            // Arrange
            IC4GTypeParser parser = _flagsParser;

            // Act
            Result<object, string> result = parser.Parse("8");

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual((TestFlags)8, result.Value);
        }
        private enum TestRarity
        {
            // ReSharper disable UnusedMember.Local
            Common = 0,
            Rare = 1,
            Epic = 2
            // ReSharper restore UnusedMember.Local
        }

        [Flags]
        private enum TestFlags
        {
            // ReSharper disable UnusedMember.Local
            None = 0,
            Flag1 = 1,
            Flag2 = 2,
            Flag3 = 4
            // ReSharper restore UnusedMember.Local
        }

        private class TestRarityParser : EnumParser<TestRarity> { }
        private class TestFlagsParser : EnumParser<TestFlags> { }
    }
}