using System;
using System.IO;
using System.Text.RegularExpressions;
using CommandLine;

namespace Tanuki.Commands
{
	public class CoverageCommand
	{
		[Verb("coverage", HelpText = "Print code coverage percent to STDOUT.")]
		public class Options
		{
			[Value(0, HelpText = "The Cobertura coverage report.", Required = true)]
			public string inputPath { get; set; }
			[Option("multiplier", HelpText = "Multiply the coverage value by this factor.")]
			public double multiplier { get; set; } = 1.0;
		}
		
		private readonly Options options;
		
		public CoverageCommand(Options options)
		{
			this.options = options;
		}

		public void OnParse()
		{
			var text = File.ReadAllText(options.inputPath);
			var match = Regex.Match(text, @"coverage line-rate=\""(?<linerate>.*?)\""");
			if (!match.Success)
			{
				throw new InvalidOperationException("Coverage value could not be parsed!");
			}
			
			var coverage = double.Parse(match.Groups["linerate"].Value);
			coverage *= options.multiplier;
			Console.WriteLine($"Code coverage is: {coverage:0.0###}");
		}
	}
}