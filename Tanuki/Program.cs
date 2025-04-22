using CommandLine;
using Tanuki.CodeQuality;
using Tanuki.Transformers;

Parser.Default.ParseArguments<CodeQuality.Options, Transform.Options>(args)
	.WithParsed<Transform.Options>(x => new Transform(x).OnParse())
	.WithParsed<CodeQuality.Options>(x => new CodeQuality(x).OnParse());