using C4G.Core.Errors;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.Errors
{
	[TestFixture]
	public class C4GErrorBaseTests
	{
		[Test]
		public void Constructor_SetsMessage()
		{
			var error = new C4GCancellationError("test message");

			Assert.AreEqual("test message", error.Message);
		}

		[Test]
		public void Constructor_WithoutCause_CauseIsNull()
		{
			var error = new C4GCancellationError("msg");

			Assert.IsNull(error.Cause);
		}

		[Test]
		public void Constructor_WithCause_SetsCause()
		{
			var cause = new C4GCancellationError("root cause");
			var error = new C4GCancellationError("outer", cause);

			Assert.AreEqual(cause, error.Cause);
		}

		[Test]
		public void Constructor_CapturesErrorTrace()
		{
			var error = new C4GCancellationError("msg");

			Assert.IsNotNull(error.ErrorTrace);
		}

		[Test]
		public void ToString_ContainsTypeName()
		{
			var error = new C4GCancellationError("msg");

			Assert.That(error.ToString(), Does.Contain(nameof(C4GCancellationError)));
		}

		[Test]
		public void ToString_ContainsMessage()
		{
			var error = new C4GCancellationError("my error message");

			Assert.That(error.ToString(), Does.Contain("my error message"));
		}

		[Test]
		public void ToString_ContainsErrorTrace()
		{
			var error = new C4GCancellationError("msg");

			Assert.That(error.ToString(), Does.Contain(error.ErrorTrace));
		}

		[Test]
		public void ToString_WithoutCause_DoesNotContainCausedBy()
		{
			var error = new C4GCancellationError("msg");

			Assert.That(error.ToString(), Does.Not.Contain("Caused by"));
		}

		[Test]
		public void ToString_WithCause_ContainsCausedBy()
		{
			var cause = new C4GCancellationError("root");
			var error = new C4GCancellationError("outer", cause);

			Assert.That(error.ToString(), Does.Contain("Caused by"));
		}

		[Test]
		public void ToString_WithCause_ContainsCauseMessage()
		{
			var cause = new C4GCancellationError("root cause message");
			var error = new C4GCancellationError("outer", cause);

			Assert.That(error.ToString(), Does.Contain("root cause message"));
		}

		[Test]
		public void ToString_WithDeepChain_ContainsAllMessages()
		{
			var root = new C4GCancellationError("level 0");
			var mid = new C4GCancellationError("level 1", root);
			var top = new C4GCancellationError("level 2", mid);

			string text = top.ToString();

			Assert.That(text, Does.Contain("level 0"));
			Assert.That(text, Does.Contain("level 1"));
			Assert.That(text, Does.Contain("level 2"));
		}
	}
}
