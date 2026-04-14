using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.SheetsParsing
{
	[TestFixture]
	public class SheetsParsingUtilsDirectTests
	{
		[Test]
		public void ParseHorizontal_NegativeStartRowIndex_ReturnsError()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "Name", "string", "Alice" }
			};

			var result = SheetsParsingUtils.ParseHorizontal("S", sheetData,
				startRowIndex: -1, startColumnIndex: 0, endRowIndex: 0, endColumnIndex: 2);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Start row index"));
		}

		[Test]
		public void ParseHorizontal_NegativeStartColumnIndex_ReturnsError()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "Name", "string", "Alice" }
			};

			var result = SheetsParsingUtils.ParseHorizontal("S", sheetData,
				startRowIndex: 0, startColumnIndex: -1, endRowIndex: 0, endColumnIndex: 2);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Start column index"));
		}

		[Test]
		public void ParseHorizontal_EndRowIndexPastData_ReturnsError()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "Name", "string", "Alice" }
			};

			var result = SheetsParsingUtils.ParseHorizontal("S", sheetData,
				startRowIndex: 0, startColumnIndex: 0, endRowIndex: 5, endColumnIndex: 2);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Rows amount"));
		}

		[Test]
		public void ParseVertical_NegativeStartRowIndex_ReturnsError()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "Id" },
				new List<object> { "int" },
				new List<object> { "1" }
			};

			var result = SheetsParsingUtils.ParseVertical("S", sheetData,
				startRowIndex: -1, startColumnIndex: 0, endRowIndex: 2, endColumnIndex: 0);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Start row index"));
		}

		[Test]
		public void ParseVertical_NegativeStartColumnIndex_ReturnsError()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "Id" },
				new List<object> { "int" },
				new List<object> { "1" }
			};

			var result = SheetsParsingUtils.ParseVertical("S", sheetData,
				startRowIndex: 0, startColumnIndex: -1, endRowIndex: 2, endColumnIndex: 0);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Start column index"));
		}

		[Test]
		public void ParseVertical_EndRowIndexPastData_ReturnsError()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "Id" },
				new List<object> { "int" },
				new List<object> { "1" }
			};

			var result = SheetsParsingUtils.ParseVertical("S", sheetData,
				startRowIndex: 0, startColumnIndex: 0, endRowIndex: 10, endColumnIndex: 0);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Rows amount"));
		}

		[Test]
		public void ParseConfigFrames_EndColumnBeforeStartColumn_NotFound()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "",      "start.A" },
				new List<object> { "end.A", ""        }
			};

			var result = SheetsParsingUtils.ParseConfigFrames("S", sheetData);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("no matching starts"));
		}
	}
}
