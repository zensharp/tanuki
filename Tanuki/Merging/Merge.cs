using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tanuki.Models;

namespace Tanuki.Merging
{
	public class Merge
	{
		[Verb("merge", HelpText = "Transforms a report into a Code Climate report.")]
		public class Options
		{
			[Option('i', "input", HelpText = "Path to source file.", Required = false)]
			public string inputPath { get; set; }
			[Option('o', "output", HelpText = "Path to destination file.", Required = false)]
			public string outputPath { get; set; }
			[Value(0)]
			public IEnumerable<string> operands { get; set; }
		}
		
		private readonly Options options;
		
		public Merge(Options options)
		{
			this.options = options;
		}

		public void OnParse()
		{
			var operands = new List<string>();
			if (!string.IsNullOrEmpty(options.inputPath))
			{
				operands.Add(options.inputPath);
			}
			foreach (var operand in options.operands)
			{
				operands.Add(operand);
			}
			
			// Gather issues
			var issues = new List<Issue>();
			foreach (var op in operands)
			{
				var srcText = File.ReadAllText(op);
				var elements = JsonConvert.DeserializeObject<List<Issue>>(srcText);
				issues.AddRange(elements);
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