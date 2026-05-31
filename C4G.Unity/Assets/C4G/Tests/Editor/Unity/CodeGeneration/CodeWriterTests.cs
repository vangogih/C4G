using System;
using C4G.Core.CodeGeneration;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.CodeGeneration
{
    public class CodeWriterTests
    {
        [Test]
        public void AddUsing_NullOrWhitespaceDirective_IsIgnored()
        {
            // Arrange
            var writer = new CodeWriter("    ");

            // Act
            writer.AddUsing(null);
            writer.AddUsing(string.Empty);
            writer.AddUsing("   ");
            string result = writer.Build();

            // Assert
            Assert.IsFalse(result.Contains("using "), "Null or whitespace directives should not appear in output");
        }

        [Test]
        public void AddUsing_DuplicateDirective_IsAddedOnce()
        {
            // Arrange
            var writer = new CodeWriter("    ");

            // Act
            writer.AddUsing("System");
            writer.AddUsing("System");
            string result = writer.Build();

            // Assert
            int firstIndex = result.IndexOf("using System;", StringComparison.Ordinal);
            int lastIndex = result.LastIndexOf("using System;", StringComparison.Ordinal);
            Assert.AreEqual(firstIndex, lastIndex, "Duplicate directive should be written only once");
        }
    }
}
