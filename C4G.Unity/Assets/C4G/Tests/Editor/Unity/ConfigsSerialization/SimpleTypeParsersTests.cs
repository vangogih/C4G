using C4G.Core.ConfigsSerialization;
using C4G.Core.ConfigsSerialization.SimpleTypeParsers;
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
			Result<object, string> result = parser.Parse("notabool");
			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void FloatParser_InvalidInput_ReturnsError()
		{
			IC4GTypeParser parser = new FloatParser();
			Result<object, string> result = parser.Parse("notafloat");
			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void DoubleParser_InvalidInput_ReturnsError()
		{
			IC4GTypeParser parser = new DoubleParser();
			Result<object, string> result = parser.Parse("notadouble");
			Assert.IsFalse(result.IsOk);
		}
	}
}
