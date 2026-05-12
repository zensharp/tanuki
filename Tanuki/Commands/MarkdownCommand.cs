using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;
using Newtonsoft.Json;
using Tanuki.Core;
using Tanuki.Models.CodeClimate;

namespace Tanuki.Commands
{
    public class MarkdownCommand
	{
		[Verb("markdown", HelpText = "Generate a static Markdown report.")]
		public class Options
		{
			[Value(0, HelpText = "The Code Quality report.", Required = true)]
			public string inputPath { get; set; }
			[Option('o', "output", HelpText = "Path to output folder.", Required = false)]
			public string outputPath { get; set; } = "report";
			[Option("base-url")]
			public string baseUrl { get; set; }
			[Option("title")]
			public string title { get; set; } = "Code Quality Report";
		}
		
		private readonly Options options;
		
		public MarkdownCommand(Options options)
		{
			this.options = options;
		}

		public void OnParse()
		{
			var reportText = File.ReadAllText(options.inputPath);
			var issues = JsonConvert.DeserializeObject<List<Issue>>(reportText);

			// Generate index page
			{
				var builder = new MarkdownBuilder(options.title, options.baseUrl, "All Linters");
				var text = builder.Build(issues);
				var filePath = Path.Combine(options.outputPath, "index.md");
				Macros.WriteAllTextOrConsole(filePath, text.ToString());
				
				Console.WriteLine(filePath);
			}

			// Generate linter pages
			var linterGroups = issues.GroupBy(x => x.linter)
				.ToList();
			for (int i = 0; i < linterGroups.Count; i++)
			{
				var linterGroup = linterGroups[i];
				var name = linterGroup.Key;
				var builder = new MarkdownBuilder(options.title, options.baseUrl, name);
				var text = builder.Build(linterGroup.ToList());
				var filePath = Path.Combine(options.outputPath, name + ".md");
				Macros.WriteAllTextOrConsole(filePath, text.ToString());
				
				Console.WriteLine(filePath);
			}
		}
	}
}
