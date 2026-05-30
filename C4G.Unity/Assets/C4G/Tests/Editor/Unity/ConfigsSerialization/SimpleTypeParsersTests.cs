using C4G.Core.ConfigsSerialization;
using C4G.Core.ConfigsSerialization.SimpleTypeParsers;
using C4G.Core.Errors;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.ConfigsSerialization
{
	[TestFixture]
	public class SimpleTypeParsersTests
	{
		[Test]
		public void BoolParser_InvalidInput_ReturnsError()
		{
			IC4GTypeParser parser = new BoolParser();
			Result<object, C4GConfigsSerializationError> result = parser.Parse("notabool");
			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void FloatParser_InvalidInput_ReturnsError()
		{
			IC4GTypeParser parser = new FloatParser();
			Result<object, C4GConfigsSerializationError> result = parser.Parse("notafloat");
			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void DoubleParser_InvalidInput_ReturnsError()
		{
			IC4GTypeParser parser = new DoubleParser();
			Result<object, C4GConfigsSerializationError> result = parser.Parse("notadouble");
			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void IntParser_ValidInput_ReturnsValue()
		{
			IC4GTypeParser parser = new IntParser();
			Result<object, C4GConfigsSerializationError> result = parser.Parse("42");
			Assert.IsTrue(result.IsOk);
			Assert.AreEqual(42, result.Value);
		}

		[Test]
		public void IntParser_InvalidInput_ReturnsError()
		{
			IC4GTypeParser parser = new IntParser();
			Result<object, C4GConfigsSerializationError> result = parser.Parse("notanint");
			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void StringParser_AnyInput_ReturnsValue()
		{
			IC4GTypeParser parser = new StringParser();
			Result<object, C4GConfigsSerializationError> result = parser.Parse("hello");
			Assert.IsTrue(result.IsOk);
			Assert.AreEqual("hello", result.Value);
		}

		[Test]
		public void StringParser_EmptyInput_ReturnsEmptyValue()
		{
			IC4GTypeParser parser = new StringParser();
			Result<object, C4GConfigsSerializationError> result = parser.Parse(string.Empty);
			Assert.IsTrue(result.IsOk);
			Assert.AreEqual(string.Empty, result.Value);
		}
	}
}
