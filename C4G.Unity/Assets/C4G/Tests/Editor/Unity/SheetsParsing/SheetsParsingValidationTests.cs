using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.SheetsParsing
{
	[TestFixture]
	public class SheetsParsingValidationTests
	{
		private C4G.Core.SheetsParsing.SheetsParsing _sheetsParsing;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_sheetsParsing = new C4G.Core.SheetsParsing.SheetsParsing();
		}

		[Test]
		public void ParseSheetToList_NullSheetName_ReturnsError()
		{
			var configs = new List<ParsedConfig>();
			var parser = new VerticalSheetParser();
			var data = new List<IList<object>>();

			Result<string> result = _sheetsParsing.ParseSheetToList(null, data, parser, configs);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void ParseSheetToList_EmptySheetName_ReturnsError()
		{
			var configs = new List<ParsedConfig>();
			var parser = new VerticalSheetParser();
			var data = new List<IList<object>>();

			Result<string> result = _sheetsParsing.ParseSheetToList(string.Empty, data, parser, configs);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void ParseSheetToList_NullParser_ReturnsError()
		{
			var configs = new List<ParsedConfig>();
			var data = new List<IList<object>>();

			Result<string> result = _sheetsParsing.ParseSheetToList("Sheet1", data, null, configs);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void ParseSheetToList_NullSheetData_ReturnsError()
		{
			var configs = new List<ParsedConfig>();
			var parser = new VerticalSheetParser();

			Result<string> result = _sheetsParsing.ParseSheetToList("Sheet1", null, parser, configs);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void ParseSheetToList_NullParsedConfigsList_ReturnsError()
		{
			var parser = new VerticalSheetParser();
			var data = new List<IList<object>>();

			Result<string> result = _sheetsParsing.ParseSheetToList("Sheet1", data, parser, null);

			Assert.IsFalse(result.IsOk);
		}
	}
}
