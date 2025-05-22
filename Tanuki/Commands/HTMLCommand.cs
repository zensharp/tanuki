using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using Tanuki.Core;
using Tanuki.Models;
using Tanuki.Models.CodeClimate;

namespace Tanuki.Commands
{

    public class HTMLCommand
	{
		[Verb("html", HelpText = "Generate a static HTML report.")]
		public class Options
		{
			[Value(0, HelpText = "The Code Quality report.", Required = true)]
			public string inputPath { get; set; }
			[Option('o', "output", HelpText = "Path to output file/folder.", Required = false)]
			public string outputPath { get; set; }
			[Option("base-url")]
			public string baseUrl { get; set; }
			[Option("title")]
			public string title { get; set; } = "Code Quality Report";
		}
		
		private readonly Options options;
		
		public HTMLCommand(Options options)
		{
			this.options = options;
		}

		public void OnParse()
		{
			var template = new HtmlTemplate();
			template.LoadResources();
			var builder = new DocumentBuilder(template, options.title, options.baseUrl);
			
			var reportText = File.ReadAllText(options.inputPath);
			var issues = JsonConvert.DeserializeObject<List<Issue>>(reportText)
				.OrderByDescending(x => Enum.Parse<Severity>(x.severity))
				.ThenBy(x => x.check_name)
				.ToList();
			
			var document = builder.Build(issues);
			
			Macros.WriteAllTextOrConsole(options.outputPath, document.ToString());
		}
	}
}