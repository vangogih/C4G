using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.SheetsParsing
{
	[TestFixture]
	public class SheetsParsingUtilsEdgeCaseTests
	{
		private C4G.Core.SheetsParsing.SheetsParsing _sheetsParsing;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_sheetsParsing = new C4G.Core.SheetsParsing.SheetsParsing();
		}

		[Test]
		public void HorizontalParser_TooFewRows_ReturnsError()
		{
			var sheetData = new List<IList<object>>();
			var configs = new List<ParsedConfig>();

			Result<string> result = _sheetsParsing.ParseSheetToList("S", sheetData, new HorizontalSheetParser(), configs);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void HorizontalParser_RowTooShort_ReturnsError()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "Name", "string", "Alice" },
				new List<object> { "Id", "int" }
			};
			var configs = new List<ParsedConfig>();

			Result<string> result = _sheetsParsing.ParseSheetToList("S", sheetData, new HorizontalSheetParser(), configs);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void VerticalParser_RowsTooFewForData_ReturnsError()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "Id" },
				new List<object> { "int" }
			};
			var configs = new List<ParsedConfig>();

			Result<string> result = _sheetsParsing.ParseSheetToList("S", sheetData, new VerticalSheetParser(), configs);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void HorizontalManyConfigs_EndWithMoreThanOneMatchingStart_ReturnsError()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "start.A" },
				new List<object> { "start.B" },
				new List<object> { "",       "",  "end.B" }
			};
			var configs = new List<ParsedConfig>();

			Result<string> result = _sheetsParsing.ParseSheetToList("S", sheetData, new HorizontalManyConfigsOnOneSheetParser(), configs);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void HorizontalManyConfigs_UnmatchedStart_ReturnsError()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "start.A" },
				new List<object> { "", "x" }
			};
			var configs = new List<ParsedConfig>();

			Result<string> result = _sheetsParsing.ParseSheetToList("S", sheetData, new HorizontalManyConfigsOnOneSheetParser(), configs);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void HorizontalManyConfigs_EndWithMismatchedName_ReturnsError()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "start.A" },
				new List<object> { "", "", "end.B" }
			};
			var configs = new List<ParsedConfig>();

			Result<string> result = _sheetsParsing.ParseSheetToList("S", sheetData, new HorizontalManyConfigsOnOneSheetParser(), configs);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void HorizontalManyConfigs_EndWithNoMatchingStart_ReturnsError()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "end.A" }
			};
			var configs = new List<ParsedConfig>();

			Result<string> result = _sheetsParsing.ParseSheetToList("S", sheetData, new HorizontalManyConfigsOnOneSheetParser(), configs);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void HorizontalSheetParser_TooFewColumns_TriggersColumnValidation()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "Name", "string" }
			};
			var configs = new List<ParsedConfig>();

			Result<string> result = _sheetsParsing.ParseSheetToList("S", sheetData, new HorizontalSheetParser(), configs);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("End column index"));
		}

		[Test]
		public void HorizontalSheetParser_SingleColumn_TriggersColumnValidation()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "Name" }
			};
			var configs = new List<ParsedConfig>();

			Result<string> result = _sheetsParsing.ParseSheetToList("S", sheetData, new HorizontalSheetParser(), configs);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("End column index"));
		}

		[Test]
		public void HorizontalManyConfigs_FrameTooNarrow_TriggersStartRowGtEndRow()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "start.A", "",       "",       "end.A" },
			};
			var configs = new List<ParsedConfig>();

			Result<string> result = _sheetsParsing.ParseSheetToList("S", sheetData, new HorizontalManyConfigsOnOneSheetParser(), configs);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Start row index"));
		}

		[Test]
		public void HorizontalManyConfigs_FrameTooFewColumns_TriggersColumnValidation()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "start.A", "",  ""      },
				new List<object> { "",        "x", ""      },
				new List<object> { "",        "",  "end.A"  }
			};
			var configs = new List<ParsedConfig>();

			Result<string> result = _sheetsParsing.ParseSheetToList("S", sheetData, new HorizontalManyConfigsOnOneSheetParser(), configs);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("End column index"));
		}

		[Test]
		public void VerticalManyConfigs_FrameTooFewRows_TriggersRowValidation()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "start.A", "",   ""      },
				new List<object> { "",        "",   ""      },
				new List<object> { "",        "",   "end.A" }
			};
			var configs = new List<ParsedConfig>();

			Result<string> result = _sheetsParsing.ParseSheetToList("S", sheetData, new VerticalManyConfigsOnOneSheetParser(), configs);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("End row index"));
		}

		[Test]
		public void VerticalManyConfigs_FrameTooNarrowColumns_TriggersColumnValidation()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "start.A", ""      },
				new List<object> { "",        ""      },
				new List<object> { "",        ""      },
				new List<object> { "",        ""      },
				new List<object> { "",        "end.A" }
			};
			var configs = new List<ParsedConfig>();

			Result<string> result = _sheetsParsing.ParseSheetToList("S", sheetData, new VerticalManyConfigsOnOneSheetParser(), configs);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Start column index"));
		}

		[Test]
		public void VerticalManyConfigs_NamesRowTooShortForFrame_ReturnsError()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "start.A", "",    "",    "",      ""       },
				new List<object> { "",        "Id"                            },
				new List<object> { "",        "int", "str", ""               },
				new List<object> { "",        "1",   "a",   ""               },
				new List<object> { "",        "",    "",    "",      "end.A"  }
			};
			var configs = new List<ParsedConfig>();

			Result<string> result = _sheetsParsing.ParseSheetToList("S", sheetData, new VerticalManyConfigsOnOneSheetParser(), configs);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public void VerticalManyConfigs_TypesRowTooShortForFrame_ReturnsError()
		{
			var sheetData = new List<IList<object>>
			{
				new List<object> { "start.A", "",    "",    "",      ""       },
				new List<object> { "",        "Id",  "Nm",  ""               },
				new List<object> { "",        "int"                           },
				new List<object> { "",        "1",   "a",   ""               },
				new List<object> { "",        "",    "",    "",      "end.A"  }
			};
			var configs = new List<ParsedConfig>();

			Result<string> result = _sheetsParsing.ParseSheetToList("S", sheetData, new VerticalManyConfigsOnOneSheetParser(), configs);

			Assert.IsFalse(result.IsOk);
		}

	}
}
