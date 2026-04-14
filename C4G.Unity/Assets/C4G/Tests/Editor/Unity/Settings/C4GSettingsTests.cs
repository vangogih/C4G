using System.Collections.Generic;
using C4G.Core.ConfigsSerialization;
using C4G.Core.Settings;
using C4G.Core.SheetsParsing;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.Settings
{
	[TestFixture]
	public class C4GSettingsTests
	{
		[Test]
		public void Constructor_AssignsAllFields()
		{
			var sheetParsers = new Dictionary<string, SheetParserBase>();
			var aliasParsers = new Dictionary<string, IC4GTypeParser>();

			var settings = new C4GSettings(
				tableId: "table1",
				rootConfigName: "Root",
				clientSecret: "secret",
				generatedCodeFolderFullPath: "/gen",
				serializedConfigsFolderFullPath: "/ser",
				sheetParsersByName: sheetParsers,
				aliasParsersByName: aliasParsers);

			Assert.AreEqual("table1", settings.TableId);
			Assert.AreEqual("Root", settings.RootConfigName);
			Assert.AreEqual("secret", settings.ClientSecret);
			Assert.AreEqual("/gen", settings.GeneratedCodeFolderFullPath);
			Assert.AreEqual("/ser", settings.SerializedConfigsFolderFullPath);
			Assert.AreSame(sheetParsers, settings.SheetParsersByName);
			Assert.AreSame(aliasParsers, settings.AliasParsersByName);
		}
	}
}
