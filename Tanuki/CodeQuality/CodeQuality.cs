using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;
using Newtonsoft.Json;
using Tanuki.CodeQuality.Models;

namespace Tanuki.CodeQuality
{
	public class CodeQuality
	{
		[Verb("codequality", HelpText = "Code Quality operations.")]
		public class Options
		{
			[Value(0, MetaName = "command", HelpText = "Code Quality command.")]
			public string command { get; set; }
			[Value(1, MetaName = "path", HelpText = "Path to target file/folder.", Required = false)]
			public string path { get; set; }
			
			[Option("urlPrefix")]
			public string urlPrefix { get; set; }
		}
		
		private readonly Options options;
		
		public CodeQuality(Options options)
		{
			this.options = options;
		}

		public void OnParse()
		{
			Console.WriteLine(options.command);
			Console.WriteLine(options.path ?? "No target");
			
			BuildHTML(options.path);
		}
		
		void BuildHTML(string path)
		{
			var reportText = File.ReadAllText(path);
			var issues = JsonConvert.DeserializeObject<List<Issue>>(reportText)
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
			
			var smellPartial = File.ReadAllText("templates/html/partials/smell.html");
			var noIssuesPartial = File.ReadAllText("templates/html/partials/no-issues.html");
			var htmlText = File.ReadAllText("templates/html/gl-code-quality-report.html");
			
			// Set title
			htmlText = GetValueRegex("project.name").Replace(htmlText, "Jontron");
			
			// Filters
			htmlText = GetValueRegex("filter.categories").Replace(htmlText, ToOptionslist(categories));
			htmlText = GetValueRegex("filter.engines").Replace(htmlText, ToOptionslist(engines));
			
			string ToOptionslist(List<string> values)
			{
				return string.Join(Environment.NewLine, values.Select(ToOption));
				
				string ToOption(string c)
				{
					return $"<option value=\"{Slugify(c)}\">{c}</option>";
				}
			}
			
			// Smells sections
			string issuesSection;
			if (issues.Count == 0)
			{
				issuesSection = noIssuesPartial;
			}
			else
			{
				issuesSection = string.Empty;
				foreach (var issue in issues)
				{
					issuesSection += SmellString(smellPartial, issue);	
				}
			}
			htmlText = GetValueRegex("smells").Replace(htmlText, issuesSection);
			
			Macros.CopyDirectory("templates/html", "public");
			File.WriteAllText("public/index.html", htmlText);
			
			EmitFilters(categories, engines);
		}
		
		string SmellString(string smellPartial, Issue issue)
		{
			var text = smellPartial;
			text = ReplaceOrEmpty(text, "issue.severity", issue.severity, "info");
			text = ReplaceOrEmpty(text, "issue.description", issue.description);
			text = ReplaceOrEmpty(text, "issue.source_path", issue.location.path);
			text = ReplaceOrEmpty(text, "issue.body", issue.body, string.Empty);
			text = ReplaceOrEmpty(text, "issue.category", issue.category);
			text = ReplaceOrEmpty(text, "issue.category_slug", Slugify(issue.category));
			text = ReplaceOrEmpty(text, "issue.engine_slug", Slugify(issue.engine));
			
			// Found In
			string foundInString = null;
			if (issue.location is not null)
			{
				var sourceFileRelativePath = issue.location.path;
				if (issue.location?.lines?.begin is not null)
				{
					sourceFileRelativePath += $"#L{issue.location.lines.begin}";
				}
				var sourceFileUrl = $"{options.urlPrefix}/{sourceFileRelativePath}";
				
				// Create HTML
				foundInString = $"Found in <a href=\"{sourceFileUrl}\">{sourceFileRelativePath}</a>";
				
				// Engine
				if (!string.IsNullOrEmpty(issue.engine))
				{
					var engineUrl = "https://gitlab.com";
					foundInString += $" by <a href=\"{engineUrl}\">{issue.engine}</a>";
				}
			}
			text = GetValueRegex("found-in").Replace(text, foundInString);
			
			static string ReplaceOrEmpty(string msg, string exp, string x, string fallback = null)
			{
				if (string.IsNullOrEmpty(x))
				{
					if (fallback is null)
					{
						return msg;
					}
					
					x = fallback;
				}
				
				return GetValueRegex(exp).Replace(msg, x);
			}

			return text;
		}
		
		void EmitFilters(List<string> categories, List<string> engines)
		{
			// Locals
			categories = categories.Select(Slugify).ToList();
			engines = engines.Select(Slugify).ToList();
			var categoriesWithAll = categories.Prepend("all").ToList();
			var enginesWithAll = engines.Prepend("all").ToList();
			
			var writer = new StringWriter();
			foreach (var category in categories)
			{
				writer.WriteLine($".filter-category-{category} > li,");	
			}
			foreach (var engine in engines)
			{
				writer.WriteLine($".filter-engine-{engine} > li,");	
			}
			writer.WriteLine(".filter-none > li { display: none; }");
			writer.WriteLine();
			
			// Engines
			foreach (var engine in engines)
			{
				writer.WriteLine($".filter-category-all.filter-engine-{engine} > li[data-engine=\"{engine}\"],");
			}
			// Categories
			foreach (var category in categories)
			{
				writer.WriteLine($".filter-category-{category}.filter-engine-all > li[data-categories~=\"{category}\"],");
			}
			// Categories + engines
			foreach (var category in categoriesWithAll)
			{
				foreach (var engine in enginesWithAll)
				{
					writer.WriteLine($".filter-category-{category}.filter-engine-{engine} > li[data-categories~=\"{category}\"][data-engine=\"{engine}\"],");
				}
			}
			// Everything
			writer.WriteLine(".filter-category-all.filter-engine-all > li { display: block; }");
			
			File.WriteAllText("public/filter.css", writer.ToString());
		}
		
		static string Slugify(string x)
		{
			if (string.IsNullOrEmpty(x))
			{
				return x;
			}
			
			x = x.Replace(" ", "_");
			x = x.ToLower();
			return x;
		}
		
		static Regex GetValueRegex(string text, RegexOptions options = RegexOptions.Multiline)
		{
			text = Regex.Escape(text);
			return new Regex(@$"<%= {text} -?%>", options);
		}
	}
}