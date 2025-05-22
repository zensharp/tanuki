using System.Collections.Generic;
using System.IO;
using CommandLine;
using Newtonsoft.Json;
using Tanuki.Models.CodeClimate;

namespace Tanuki.Commands
{
	public class EditCommand
	{
		[Verb("edit", HelpText = "Transform a report into a Code Climate report.")]
		public class Options
		{
			[Value(0, HelpText = "The Code Quality report.", Required = true)]
			public string inputPath { get; set; }
			[Option('o', "output", HelpText = "Path to destination file.", Required = false)]
			public string outputPath { get; set; }
			[Option("location-prefix", HelpText = "Prefix to prepend to each file path.")]
			public string locationPrefix { get; set; }
			[Option("linter", HelpText = "Set the Linter.")]
			public string linter { get; set; }
		}
		
		private readonly Options options;
		
		public EditCommand(Options options)
		{
			this.options = options;
		}

		public void OnParse()
		{
			var inputText = File.ReadAllText(options.inputPath);
			var issues = JsonConvert.DeserializeObject<List<Issue>>(inputText);
			
			foreach (var issue in issues)
			{
				// Set linter
				if (!string.IsNullOrEmpty(options.linter))
				{
					issue.linter = options.linter;
				}
				
				// Prepend prefix to path
				if (!string.IsNullOrEmpty(options.locationPrefix))
				{
					if (issue.location is not null)
					{
						issue.location.path = Path.Combine(options.locationPrefix, issue.location.path);
					}
				}
			}
			
			var formatting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, };
			var outputText = JsonConvert.SerializeObject(issues, formatting);
			Macros.WriteAllTextOrConsole(options.outputPath, outputText);
		}
	}
}