using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;
using Newtonsoft.Json;
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
			[Option('o', "output", HelpText = "Path to output file/folder.", Required = false)]
			public string outputPath { get; set; }
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

			PrintSummary(issues);
		}

		void PrintSummary(List<Issue> issues)
		{
			var output = new StringBuilder();

			output.AppendLine($"{issues.Count()} total items found");

			var severityGroups = issues.GroupBy(x => x.severity)
				.OrderByDescending(x => Enum.Parse<Severity>(x.Key))
				.ToList();
			// Do severity group
			foreach (var severityGroup in severityGroups)
			{
				var title = SentenceCase(severityGroup.Key);
				var admonition = LookupAdmonition(severityGroup.Key);
				output.AppendLine($"???+ {admonition} \"{title} ({severityGroup.Count()} items)\"");

				// Do check_name groups
				var checkGroups = severityGroup.GroupBy(x => x.check_name)
					.OrderBy(x => x.Key);
				foreach (var checkGroup in checkGroups)
				{
					output.AppendLine($"	???+ {admonition} \"{checkGroup.Key} ({checkGroup.Count()} items)\"");

					foreach (var issue in checkGroup)
					{
						var isUniqueFile = checkGroup.Where(x => x.location.path == issue.location.path).Distinct().Count() <= 2;

						string pathText = FoundIn(issue.location, forceLineNumbers: !isUniqueFile);
						var description = issue.description.TrimEnd(".");
						output.AppendLine($"		* [ ] {description} {pathText}.");
					}
				}
			}

			// Summary
			File.WriteAllText("summary.md", output.ToString());
		}

		static string LookupAdmonition(string severity)
		{
			switch (severity)
			{
				case "blocker":
					return "danger";
				case "critical":
					return "failure";
				case "major":
					return "warning";
				case "minor":
					return "note";
				case "info":
					return "quote";
			}

			return severity;
		}
		static string SentenceCase(string input)
		{
			if (input.Length < 1) return input;

			string sentence = input.ToLower();
			return sentence[0].ToString().ToUpper() +
			sentence.Substring(1);
		}
	
		string FoundIn(Issue.Location location, bool forceLineNumbers = false)
		{
			var filename = Path.GetFileName(location.path);
			var sourceFileUrl = location.path;
			/// Prepend base URL
			if (!string.IsNullOrEmpty(options.baseUrl))
			{
				sourceFileUrl = $"{options.baseUrl}/{sourceFileUrl}";
			}
			/// Append line number
			if (location?.lines?.begin is not null)
			{
				sourceFileUrl += $"#L{location.lines.begin}";
				if (forceLineNumbers)
				{
					filename += $":{location.lines.begin}";
				}
			}
			
			return $"in [{filename}]({sourceFileUrl})";
		}
	}
}