using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using Tanuki.Core;
using Tanuki.Models.CodeClimate;

namespace Tanuki.Commands
{
	public class AssertCommand
	{
		[Verb("assert", HelpText = "Assert a certain condition is true.")]
		public class Options
		{
			[Value(0, HelpText = "The Code Quality report.", Required = true)]
			public string inputPath { get; set; }
			[Option("where", HelpText = "Where selector regex.")]
			public string where { get; set; } = "*";
			[Option("all", HelpText = "Predicate which all elements must pass.")]
			public string all { get; set; }
			[Option("any", HelpText = "Predicate which any element must pass.")]
			public string any { get; set; }
			[Option("none", HelpText = "Predicate which no element must pass.")]
			public string none { get; set; }
		}
		
		public enum Mode
		{
			Null,
			All,
			Any,
			None,
		}
		
		private readonly Options options;
		
		public AssertCommand(Options options)
		{
			this.options = options;
		}

		public void OnParse()
		{
			IssueFilterer predicate;
			var mode = Mode.Null;
			if (!string.IsNullOrEmpty(options.all))
			{
				mode = Mode.All;
				predicate = new IssueFilterer(options.all);
			}
			else if (!string.IsNullOrEmpty(options.any))
			{
				mode = Mode.Any;
				predicate = new IssueFilterer(options.any);
			}
			else if (!string.IsNullOrEmpty(options.none))
			{
				mode = Mode.None;
				predicate = new IssueFilterer(options.none);
			}
			else
			{
				throw new ArgumentNullException("Please specify a predicate: all, any, none");
			}
			
			var inputText = File.ReadAllText(options.inputPath);
			var filterer = new IssueFilterer(options.where);
			
			var issues = JsonConvert.DeserializeObject<List<Issue>>(inputText)
				.Where(filterer.IsMatch);
			
			bool pass = true;
			switch (mode)
			{
				case Mode.All:
					pass = issues.All(predicate.IsMatch);
					break;
				case Mode.Any:
					pass = issues.Any(predicate.IsMatch);
					break;
				case Mode.None:
					pass = !issues.All(predicate.IsMatch);
					break;
			}
			
			Environment.ExitCode = pass ? 0 : 1;
		}
	}
}