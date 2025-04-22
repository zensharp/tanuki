using System.Collections.Generic;
using System.IO;
using CommandLine;
using Newtonsoft.Json;
using Tanuki.Models;

namespace Tanuki.Operations
{
	public class Edit
	{
		[Verb("edit", HelpText = "Transforms a report into a Code Climate report.")]
		public class Options
		{
			[Value(0)]
			public string inputPath { get; set; }
			[Option('o', "output", HelpText = "Path to destination file.", Required = false)]
			public string outputPath { get; set; }
			[Option("location-prefix", HelpText = "Prefix to prepend to each file path.")]
			public string locationPrefix { get; set; }
			[Option("engine", HelpText = "Set the Engine.")]
			public string engine { get; set; }
		}
		
		private readonly Options options;
		
		public Edit(Options options)
		{
			this.options = options;
		}

		public void OnParse()
		{
			var srcText = File.ReadAllText(options.inputPath);
			var issues = JsonConvert.DeserializeObject<List<Issue>>(srcText);
			
			foreach (var issue in issues)
			{
				// Set engine
				if (!string.IsNullOrEmpty(options.engine))
				{
					issue.engine = options.engine;
				}
				
				// Prepend base URL
				if (!string.IsNullOrEmpty(options.locationPrefix))
				{
					if (issue.location is not null)
					{
						issue.location.path = Path.Combine(options.locationPrefix, issue.location.path);
					}
				}
			}
			
			var destText = JsonConvert.SerializeObject(issues);
			Macros.WriteAllTextOrConsole(options.outputPath, destText);
		}
	}
}