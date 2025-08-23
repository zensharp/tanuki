using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using Tanuki.Core;
using Tanuki.Models.CodeClimate;

namespace Tanuki.Commands
{
	public class TriggerCommand
	{
		[Verb("trigger", HelpText = "Runs the macros listed in the Tanukifile.", Hidden = true)]
		public class Options
		{
			[Value(0, HelpText = "The Code Quality report.", Required = true)]
			public string inputPath { get; set; }
			[Option('o', "output", HelpText = "Path to destination file.", Required = false)]
			public string outputPath { get; set; }
			[Option("tanukifile", HelpText = "Location to the Tanukifile.", Default = ".tanuki/Tanukifile")]
			public string tanukifile { get; set; }
		}

		private readonly Options options;

		public TriggerCommand(Options options)
		{
			this.options = options;
		}

		public void OnParse()
		{
			var inputText = File.ReadAllText(options.inputPath);
			var inputIssues = JsonConvert.DeserializeObject<List<Issue>>(inputText);
			var outputIssues = new HashSet<Issue>(inputIssues);
			var tanukifile = Tanukifile.Read(options.tanukifile);

			var filterers = tanukifile.filter.exclude
				.Select(x => new IssueFilterer(x))
				.ToList();

			for (int i = 0; i< inputIssues.Count; i++)
			{
				var issue = inputIssues[i];
				foreach (var filterer in filterers)
				{
					if (filterer.IsMatch(issue))
					{
						outputIssues.Remove(issue);
					}
				}
			}

			var formatting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, };
			var outputText = JsonConvert.SerializeObject(outputIssues, formatting);
			Macros.WriteAllTextOrConsole(options.outputPath, outputText);
		}
	}
}