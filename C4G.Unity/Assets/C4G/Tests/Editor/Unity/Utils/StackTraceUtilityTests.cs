using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.Utils
{
	[TestFixture]
	public class StackTraceUtilityTests
	{
		[Test]
		public void GetAssetPathsOnly_NegativeSkipFrames_DoesNotThrow()
		{
			string result = StackTraceUtility.GetAssetPathsOnly(-1);
			Assert.IsNotNull(result);
		}

		[Test]
		public void FormatMethodName_NullMethod_ReturnsUnknown()
		{
			string result = StackTraceUtility.FormatMethodName(null);
			Assert.AreEqual("Unknown type.Unknown method", result);
		}

		[Test]
		public void FormatMethodName_ValidMethod_ReturnsTypeDotName()
		{
			var method = typeof(StackTraceUtilityTests).GetMethod(nameof(FormatMethodName_ValidMethod_ReturnsTypeDotName));
			string result = StackTraceUtility.FormatMethodName(method);
			Assert.AreEqual("StackTraceUtilityTests.FormatMethodName_ValidMethod_ReturnsTypeDotName", result);
		}

		[Test]
		public void FormatMethodName_GlobalMethod_ReturnsUnknownType()
		{
			var dynamicMethod = new System.Reflection.Emit.DynamicMethod("TestDyn", typeof(void), null);
			string result = StackTraceUtility.FormatMethodName(dynamicMethod);
			Assert.AreEqual("Unknown type.TestDyn", result);
		}
	}
}
