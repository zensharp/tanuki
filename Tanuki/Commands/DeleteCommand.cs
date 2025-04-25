using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using CommandLine;
using Newtonsoft.Json;
using Tanuki.Core;
using Tanuki.Models.CodeClimate;

namespace Tanuki.Commands
{
	public class DeleteCommand
	{
		[Verb("delete", HelpText = "Delete elements in a report given some critieria (experimental).", Hidden = true)]
		public class Options
		{
			[Value(0)]
			public string inputPath { get; set; }
			[Option('o', "output", HelpText = "Path to destination file.", Required = false)]
			public string outputPath { get; set; }
			[Option("where", HelpText = "Where selector regex.")]
			public string where { get; set; }
		}
		
		private readonly Options options;
		
		public DeleteCommand(Options options)
		{
			this.options = options;
		}

		public void OnParse()
		{
			if (string.IsNullOrEmpty(options.where))
			{
				throw new ArgumentNullException("Please specify an expression with --where");
			}
			
			var inputText = File.ReadAllText(options.inputPath);
			var inputIssues = JsonConvert.DeserializeObject<List<Issue>>(inputText);
			
			var filterer = new IssueFilterer(options.where);
			
			var outputIssues = new HashSet<Issue>(inputIssues);
			foreach (var issue in inputIssues)
			{	
				if (filterer.IsMatch(issue))
				{
					outputIssues.Remove(issue);
				}
			}
			
			var outputText = JsonConvert.SerializeObject(outputIssues);
			Macros.WriteAllTextOrConsole(options.outputPath, outputText);
		}
	}
}