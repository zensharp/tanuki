using CommandLine;
using Tanuki.Operations;

Parser.Default.ParseArguments<Transform.Options, Merge.Options, Edit.Options, HTML.Options>(args)
	.WithParsed<Transform.Options>(x => new Transform(x).OnParse())
	.WithParsed<Merge.Options>(x => new Merge(x).OnParse())
	.WithParsed<Edit.Options>(x => new Edit(x).OnParse())
	.WithParsed<HTML.Options>(x => new HTML(x).OnParse());