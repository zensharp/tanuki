using CommandLine;
using Tanuki.CodeQuality;

Parser.Default.ParseArguments<CodeQuality.Options>(args)
	.WithParsed(CodeQuality.OnParse);