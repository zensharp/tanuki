using System.Collections.Generic;
using System.IO;
using CommandLine;
using Newtonsoft.Json;
using Tanuki.Models.CodeClimate;

namespace Tanuki.Commands
{
	public class MergeCommand
	{
		[Verb("merge", HelpText = "Combine code quality reports.")]
		public class Options
		{
			[Value(0, HelpText = "The Code Quality reports.", Required = true)]
			public IEnumerable<string> operands { get; set; }
			[Option('o', "output", HelpText = "Path to destination file.", Required = false)]
			public string outputPath { get; set; }
		}
		
		private readonly Options options;
		
		public MergeCommand(Options options)
		{
			this.options = options;
		}

		public void OnParse()
		{
			// Gather issues from all files
			var issues = new List<Issue>();
			foreach (var path in options.operands)
			{
				var text = File.ReadAllText(path);
				var models = JsonConvert.DeserializeObject<List<Issue>>(text);
				issues.AddRange(models);
			}
			
			var formatting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, };
			var outputText = JsonConvert.SerializeObject(issues, formatting);
			Macros.WriteAllTextOrConsole(options.outputPath, outputText);
		}
	}
}