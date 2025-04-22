using CommandLine;
using Tanuki.HTML;
using Tanuki.Transformers;

Parser.Default.ParseArguments<Transform.Options, HTML.Options>(args)
	.WithParsed<Transform.Options>(x => new Transform(x).OnParse())
	.WithParsed<HTML.Options>(x => new HTML(x).OnParse());