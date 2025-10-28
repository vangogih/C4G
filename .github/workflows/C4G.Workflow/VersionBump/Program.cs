using System.Text.Json.Nodes;

if (args.Length != 1)
    Fail("Incorrect args amount. Should be 2: rootPath, semVer \'1.2.3\'");

if (!Version.TryParse(args[0], out Version? version))
    Fail($"Incorrect version format '{args[0]}. Should be, semVer: \'1.2.3\'");

var rootPath = Environment.GetEnvironmentVariable("GITHUB_WORKSPACE");

if (!Directory.Exists(rootPath))
    Fail($"Incorrect root path: {rootPath}");

var unityPackageJsonPath = Path.Combine(rootPath!, "C4G.Unity", "Assets", "C4G", "package.json");

if (!File.Exists(unityPackageJsonPath))
    Fail($"Incorrect package.json path: {unityPackageJsonPath}");

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