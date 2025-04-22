using CommandLine;
using Tanuki.HTML;
using Tanuki.Merging;
using Tanuki.Transformers;

Parser.Default.ParseArguments<Transform.Options, Merge.Options, HTML.Options>(args)
	.WithParsed<Transform.Options>(x => new Transform(x).OnParse())
	.WithParsed<Merge.Options>(x => new Merge(x).OnParse())
	.WithParsed<HTML.Options>(x => new HTML(x).OnParse());