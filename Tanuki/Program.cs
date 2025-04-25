using CommandLine;
using Tanuki.Commands;

Parser.Default.ParseArguments<CoverageCommand.Options, DeleteCommand.Options, EditCommand.Options, HTMLCommand.Options, MergeCommand.Options, TransformCommand.Options>(args)
	.WithParsed<CoverageCommand.Options>(x => new CoverageCommand(x).OnParse())
	.WithParsed<DeleteCommand.Options>(x => new DeleteCommand(x).OnParse())
	.WithParsed<EditCommand.Options>(x => new EditCommand(x).OnParse())
	.WithParsed<HTMLCommand.Options>(x => new HTMLCommand(x).OnParse())
	.WithParsed<MergeCommand.Options>(x => new MergeCommand(x).OnParse())
	.WithParsed<TransformCommand.Options>(x => new TransformCommand(x).OnParse());
