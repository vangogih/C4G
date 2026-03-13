#!/usr/bin/env dotnet run

// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/file-based-programs

using System.Text.Json.Nodes;
using System.Xml.Linq;

if (args.Length != 1)
    Fail("Incorrect args amount. Should be 1: semVer \'1.2.3\'");

if (!Version.TryParse(args[0], out Version? version))
    Fail($"Incorrect version format '{args[0]}. Should be, semVer: \'1.2.3\'");

var rootPath = Environment.GetEnvironmentVariable("GITHUB_WORKSPACE");

if (!Directory.Exists(rootPath))
    Fail($"Incorrect root path: {(string.IsNullOrEmpty(rootPath) ? "EMPTY" : rootPath)}");

var unityPackageJsonPath = Path.Combine(rootPath!, "C4G.Unity", "Assets", "C4G", "package.json");
var standaloneCsprojPath = Path.Combine(rootPath!, "C4G.Standalone", "C4G.Standalone.csproj");

if (!File.Exists(unityPackageJsonPath))
    Fail($"Incorrect package.json path: {unityPackageJsonPath}");

if (!File.Exists(standaloneCsprojPath))
    Fail($"Incorrect .csproj path: {standaloneCsprojPath}");

var from = string.Empty;
// package.json patch
{
    try
    {
        var packageObject = JsonNode.Parse(File.ReadAllText(unityPackageJsonPath));
        from = packageObject!["version"]!.GetValue<string>();
        packageObject["version"] = version!.ToString();
        File.WriteAllText(unityPackageJsonPath, packageObject.ToString());
    }
    catch (Exception e)
    {
        Fail(e.ToString());
    }
}

// C4G.Standalone.csproj patch
{
    try
    {
        var doc = XDocument.Load(standaloneCsprojPath);
        var versionElement = doc.Descendants("Version").FirstOrDefault();

        if (versionElement == null)
            Fail($"<Version> element not found in {standaloneCsprojPath}");

        versionElement!.Value = version!.ToString();
        doc.Save(standaloneCsprojPath);
    }
    catch (Exception e)
    {
        Fail(e.ToString());
    }
}

Success($"SUCCESS: Version patched from {from} to {version}");

void Fail(string message)
{
    Console.WriteLine(message);
    Environment.Exit(1);
}

void Success(string message)
{
    Console.WriteLine(message);
    Environment.Exit(0);
}