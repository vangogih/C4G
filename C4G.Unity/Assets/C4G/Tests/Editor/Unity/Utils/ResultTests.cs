using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.Utils
{
	[TestFixture]
	public class ResultTests
	{
		[Test]
		public void ResultTValueTError_FromValue_SetsValueAndIsOk()
		{
			var result = Result<int, string>.FromValue(42);

			Assert.IsTrue(result.IsOk);
			Assert.AreEqual(42, result.Value);
			Assert.IsNull(result.Error);
			Assert.IsNull(result.StackTrace);
		}

		[Test]
		public void ResultTValueTError_FromError_SetsErrorAndNotOk()
		{
			var result = Result<int, string>.FromError("fail");

			Assert.IsFalse(result.IsOk);
			Assert.AreEqual("fail", result.Error);
			Assert.IsNotNull(result.StackTrace);
			Assert.AreEqual(default(int), result.Value);
		}

		[Test]
		public void ResultTValueTError_ToString_OkPath()
		{
			var result = Result<int, string>.FromValue(10);

			string text = result.ToString();

			Assert.That(text, Does.Contain("IsOk - True"));
			Assert.That(text, Does.Contain("Value - 10"));
		}

		[Test]
		public void ResultTValueTError_ToString_ErrorPath()
		{
			var result = Result<int, string>.FromError("broken");

			string text = result.ToString();

			Assert.That(text, Does.Contain("IsOk - False"));
			Assert.That(text, Does.Contain("Error - broken"));
			Assert.That(text, Does.Contain("StackTrace"));
		}

		[Test]
		public void ResultTError_Ok_IsOkTrue()
		{
			var result = Result<string>.Ok;

			Assert.IsTrue(result.IsOk);
			Assert.IsNull(result.Error);
			Assert.IsNull(result.StackTrace);
		}

		[Test]
		public void ResultTError_FromError_SetsErrorAndNotOk()
		{
			var result = Result<string>.FromError("oops");

			Assert.IsFalse(result.IsOk);
			Assert.AreEqual("oops", result.Error);
			Assert.IsNotNull(result.StackTrace);
		}

		[Test]
		public void ResultTError_FromResultWithValue_OkInput()
		{
			var source = Result<int, string>.FromValue(5);

			var result = Result<string>.FromResultWithValue(source);

			Assert.IsTrue(result.IsOk);
		}

		[Test]
		public void ResultTError_FromResultWithValue_ErrorInput()
		{
			var source = Result<int, string>.FromError("inner");

			var result = Result<string>.FromResultWithValue(source);

			Assert.IsFalse(result.IsOk);
			Assert.AreEqual("inner", result.Error);
			Assert.AreEqual(source.StackTrace, result.StackTrace);
		}

		[Test]
		public void ResultTError_ToString_OkPath()
		{
			var result = Result<string>.Ok;

			string text = result.ToString();

			Assert.That(text, Does.Contain("IsOk - True"));
			Assert.That(text, Does.Not.Contain("Error -"));
		}

		[Test]
		public void ResultTError_ToString_ErrorPath()
		{
			var result = Result<string>.FromError("bad");

			string text = result.ToString();

			Assert.That(text, Does.Contain("IsOk - False"));
			Assert.That(text, Does.Contain("Error - bad"));
			Assert.That(text, Does.Contain("StackTrace"));
		}

		[Test]
		public void WithoutValue_OkResult_ReturnsOk()
		{
			var source = Result<int, string>.FromValue(99);

			var result = source.WithoutValue();

			Assert.IsTrue(result.IsOk);
		}

		[Test]
		public void WithoutValue_ErrorResult_ReturnsError()
		{
			var source = Result<int, string>.FromError("err");

			var result = source.WithoutValue();

			Assert.IsFalse(result.IsOk);
			Assert.AreEqual("err", result.Error);
		}
	}
}
