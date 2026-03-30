#!/usr/bin/env dotnet run

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

const double DefaultThresholdPercent = 97.0;

var argsDict = ParseNamedArgs(args);
if (!argsDict.TryGetValue("summary-path", out string? summaryPath) || string.IsNullOrWhiteSpace(summaryPath))
	Fail(BuildMissingSummaryPathMessage(args));

string summaryFilePath = summaryPath!;

double thresholdPercent = DefaultThresholdPercent;
if (argsDict.TryGetValue("threshold", out string? thresholdRaw) && !string.IsNullOrWhiteSpace(thresholdRaw))
{
	if (!double.TryParse(thresholdRaw.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out thresholdPercent))
		Fail(BuildInvalidThresholdMessage(thresholdRaw));
}

if (!File.Exists(summaryFilePath))
	Fail(BuildMissingFileMessage(summaryFilePath));

SummaryJsonRoot? doc = null;
try
{
	doc = JsonSerializer.Deserialize(File.ReadAllText(summaryFilePath), SummaryJsonContext.Default.SummaryJsonRoot);
}
catch (Exception ex)
{
	Fail(BuildJsonParseErrorMessage(summaryFilePath, ex));
}

if (doc is null || doc.Summary is null)
{
	Fail(
		$"Summary.json has no \"summary\" object or it is null.{Environment.NewLine}Path: {summaryFilePath}");
}

Summary summary = doc!.Summary!;

var violations = new List<string>();
CheckMetric("summary.linecoverage", summary.Linecoverage, thresholdPercent, violations);
CheckMetric("summary.branchcoverage", summary.Branchcoverage, thresholdPercent, violations);
CheckMetric("summary.methodcoverage", summary.Methodcoverage, thresholdPercent, violations);

if (violations.Count > 0)
{
	var sb = new StringBuilder();
	sb.AppendLine("Coverage threshold check failed.");
	sb.AppendLine($"Summary file: {summaryFilePath}");
	sb.AppendLine($"Required minimum (each present metric): >= {thresholdPercent.ToString(CultureInfo.InvariantCulture)}%");
	sb.AppendLine($"Checked: summary.linecoverage, summary.branchcoverage, summary.methodcoverage (null/absent skipped).");
	sb.AppendLine($"Violations ({violations.Count}):");
	for (int i = 0; i < violations.Count; i++)
		sb.AppendLine($"  {i + 1}. {violations[i]}");
	Fail(sb.ToString().TrimEnd());
}

Success(
	$"Coverage threshold OK: checked summary metrics are >= {thresholdPercent.ToString(CultureInfo.InvariantCulture)}% (file: {summaryFilePath})");

static void CheckMetric(string path, double? value, double thresholdPercent, List<string> violations)
{
	if (!value.HasValue)
		return;

	double actual = value.Value;
	if (actual < thresholdPercent)
	{
		violations.Add(
			$"{path} is {actual.ToString(CultureInfo.InvariantCulture)}%, required >= {thresholdPercent.ToString(CultureInfo.InvariantCulture)}%.");
	}
}

static Dictionary<string, string> ParseNamedArgs(string[] args)
{
	var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	for (int i = 0; i < args.Length; i++)
	{
		string a = args[i];
		if (a.Equals("--summary-path", StringComparison.OrdinalIgnoreCase))
		{
			if (i + 1 >= args.Length)
				Fail("Missing value for --summary-path.");
			dict["summary-path"] = args[++i];
			continue;
		}

		if (a.Equals("--threshold", StringComparison.OrdinalIgnoreCase))
		{
			if (i + 1 >= args.Length)
				Fail("Missing value for --threshold.");
			dict["threshold"] = args[++i];
			continue;
		}

		Fail($"Unexpected argument \"{a}\". Use --summary-path <path> and optionally --threshold <percent> (default {DefaultThresholdPercent}).");
	}

	return dict;
}

static string BuildMissingSummaryPathMessage(string[] args)
{
	var sb = new StringBuilder();
	sb.AppendLine("Missing required --summary-path argument.");
	sb.AppendLine($"Received {args.Length} argument(s).");
	sb.AppendLine("Usage: dotnet run --file ./CheckCoverageThreshold.cs -- --summary-path <path-to-Summary.json> [--threshold <percent>]");
	sb.AppendLine($"Default --threshold is {DefaultThresholdPercent}% if omitted.");
	return sb.ToString().TrimEnd();
}

static string BuildInvalidThresholdMessage(string raw)
{
	var sb = new StringBuilder();
	sb.AppendLine("Invalid --threshold value: it must be a number (percent), using invariant culture (e.g. 97 or 97.1).");
	sb.AppendLine($"Given value: \"{raw}\"");
	return sb.ToString().TrimEnd();
}

static string BuildMissingFileMessage(string path)
{
	var sb = new StringBuilder();
	sb.AppendLine("Summary.json was not found.");
	sb.AppendLine($"Path: {path}");
	sb.AppendLine("Ensure tests ran with coverage and ReportGenerator wrote JsonSummary to that location (see C4G.Standalone.Tests.csproj GenerateCoverageReport target).");
	return sb.ToString().TrimEnd();
}

static string BuildJsonParseErrorMessage(string path, Exception ex)
{
	var sb = new StringBuilder();
	sb.AppendLine("Failed to deserialize Summary.json.");
	sb.AppendLine($"Path: {path}");
	sb.AppendLine("Details: " + ex.Message);
	return sb.ToString().TrimEnd();
}

static void Fail(string message)
{
	Console.WriteLine(message);
	Environment.Exit(1);
}

static void Success(string message)
{
	Console.WriteLine(message);
	Environment.Exit(0);
}

public sealed class SummaryJsonRoot
{
	[JsonPropertyName("summary")]
	public Summary? Summary { get; set; }

	[JsonPropertyName("coverage")]
	public Coverage? Coverage { get; set; }
}

public sealed class Summary
{
	[JsonPropertyName("linecoverage")]
	public double? Linecoverage { get; set; }

	[JsonPropertyName("branchcoverage")]
	public double? Branchcoverage { get; set; }

	[JsonPropertyName("methodcoverage")]
	public double? Methodcoverage { get; set; }
}

public sealed class Coverage
{
}

[JsonSerializable(typeof(SummaryJsonRoot))]
internal sealed partial class SummaryJsonContext : JsonSerializerContext
{
}
