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

		class FilterGroup
		{
			readonly List<IssueFilterer> filterers;

			public FilterGroup(Tanukifile.Filter.Exclude exclude)
			{
				filterers = exclude.conditions
					.Select(x => new IssueFilterer(x))
					.ToList();
			}

			public bool IsMatch(Issue issue)
			{
				return filterers.All(x => x.IsMatch(issue));
			}
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

			var filtererGroups = tanukifile.filter.exclude
				.Select(x => new FilterGroup(x))
				.ToList();

			for (int i = 0; i < inputIssues.Count; i++)
			{
				var issue = inputIssues[i];
				foreach (var filterer in filtererGroups)
				{
					if (filterer.IsMatch(issue))
					{
						outputIssues.Remove(issue);
						break;
					}
				}
			}

			var formatting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, };
			var outputText = JsonConvert.SerializeObject(outputIssues, formatting);
			Macros.WriteAllTextOrConsole(options.outputPath, outputText);
		}
	}
}