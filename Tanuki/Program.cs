using CommandLine;
using Tanuki.CodeQuality;

Parser.Default.ParseArguments<CodeQuality.Options>(args)
	.WithParsed(x => new CodeQuality(x).OnParse());