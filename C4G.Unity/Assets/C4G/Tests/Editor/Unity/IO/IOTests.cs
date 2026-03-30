using System;
using System.IO;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.IO
{
	[TestFixture]
	public class IOTests
	{
		private C4G.Core.IO.IO _io;
		private string _tempRoot;

		[SetUp]
		public void SetUp()
		{
			_io = new C4G.Core.IO.IO();
			_tempRoot = Path.Combine(Path.GetTempPath(), "C4G_IOTests_" + Guid.NewGuid().ToString("N"));
		}

		[TearDown]
		public void TearDown()
		{
			if (Directory.Exists(_tempRoot))
				Directory.Delete(_tempRoot, recursive: true);
		}

		[Test]
		public void WriteToFile_ExistingFolder_WritesContent()
		{
			Directory.CreateDirectory(_tempRoot);

			Result<string> result = _io.WriteToFile(_tempRoot, "test.txt", "hello");

			Assert.IsTrue(result.IsOk);
			string content = File.ReadAllText(Path.Combine(_tempRoot, "test.txt"));
			Assert.AreEqual("hello", content);
		}

		[Test]
		public void WriteToFile_NonExistingFolder_CreatesDirectoryAndWrites()
		{
			string nested = Path.Combine(_tempRoot, "sub", "deep");

			Result<string> result = _io.WriteToFile(nested, "data.txt", "content");

			Assert.IsTrue(result.IsOk);
			Assert.IsTrue(Directory.Exists(nested));
			Assert.AreEqual("content", File.ReadAllText(Path.Combine(nested, "data.txt")));
		}

		[Test]
		public void WriteToFile_InvalidPath_ReturnsError()
		{
			string invalidFolder = _tempRoot + Path.DirectorySeparatorChar + "::??**";

			Result<string> result = _io.WriteToFile(invalidFolder, "f.txt", "x");

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.StartWith("IO error."));
		}
	}
}
