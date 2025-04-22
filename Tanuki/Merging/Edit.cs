using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tanuki.Models;

namespace Tanuki.Merging
{
	public class Edit
	{
		[Verb("edit", HelpText = "Transforms a report into a Code Climate report.")]
		public class Options
		{
			[Option('i', "input", HelpText = "Path to source file.", Required = false)]
			public string inputPath { get; set; }
			[Option('o', "output", HelpText = "Path to destination file.", Required = false)]
			public string outputPath { get; set; }
			[Option("base-url", HelpText = "Prefix to prepend to each file path.")]
			public string baseUrl { get; set; }
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
				if (!string.IsNullOrEmpty(options.baseUrl))
				{
					if (issue.location is not null)
					{
						issue.location.path = Path.Combine(options.baseUrl, issue.location.path);
					}
				}
			}
			
			var destText = JsonConvert.SerializeObject(issues);
			File.WriteAllText(GetOutputPath(), destText);
		}
		
		string GetOutputPath()
		{
			if (string.IsNullOrEmpty(options.outputPath))
			{
				return "merged.json";
			}
			
			return options.outputPath;
		}
	}
}