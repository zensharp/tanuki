using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using Tanuki.CodeQuality.Models;

namespace Tanuki.CodeQuality
{
	public static class CodeQuality
	{
		[Verb("codequality", HelpText = "Code Quality operations.")]
		public class Options
		{
			[Value(0, MetaName = "command", HelpText = "Code Quality command.")]
			public string command { get; set; }
			[Value(1, MetaName = "path", HelpText = "Path to target file/folder.", Required = false)]
			public string path { get; set; }
		}

		public static void OnParse(Options options)
		{
			Console.WriteLine(options.command);
			Console.WriteLine(options.path ?? "No target");
			
			BuildHTML(options.path);
		}
		
		static void BuildHTML(string path)
		{
			var text = File.ReadAllText(path);
			var issues = JsonConvert.DeserializeObject<List<Issue>>(text)
				.OrderByDescending(x => Issue.SeverityToInt(x.severity))
				.ToList();
			var categories = issues
				.Select(x => x.category)
				.Where(x => !string.IsNullOrEmpty(x))
				.Distinct()
				.ToList();
			var engines = issues
				.Select(x => x.engine)
				.Where(x => !string.IsNullOrEmpty(x))
				.Distinct()
				.ToList();
			
			Console.WriteLine("Issues:");
			foreach (var i in issues)
			{
				Console.WriteLine($"{i.severity} {i.code} {i.description}");
			}
			
			Console.WriteLine("filter.css");
			foreach (var category in categories)
			{
				Console.WriteLine($".filter-category-{category} > li,");	
			}
			foreach (var engine in engines)
			{
				var slug = engine.Replace(" ", "_");
				Console.WriteLine($".filter-engine-{slug} > li,");	
			}
		}
	}
}