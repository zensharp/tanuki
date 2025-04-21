using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using Newtonsoft.Json;
using Tanuki.Models;

Parser.Default.ParseArguments<CodeQualityOptions>(args)
	.WithParsed<CodeQualityOptions>(OnParse);



void OnParse(CodeQualityOptions options)
{
	Console.WriteLine(options.command);
	Console.WriteLine(options.path ?? "No target");
	
	var text = File.ReadAllText(options.path);
	var issues = JsonConvert.DeserializeObject<List<CodeQualityReport.Issue>>(text);
	foreach (var i in issues)
	{
		Console.WriteLine(i.description);
	}
}

[Verb("codequality", HelpText = "Code Quality operations.")]
public class CodeQualityOptions
{
	[Value(0, MetaName = "command", HelpText = "Code Quality command.")]
	public string command { get; set; }
	[Value(1, MetaName = "path", HelpText = "Path to target file/folder.", Required = false)]
	public string path { get; set; }
}