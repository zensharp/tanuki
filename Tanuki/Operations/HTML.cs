using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using CommandLine;
using Newtonsoft.Json;
using Tanuki.Core;
using Tanuki.Models;

namespace Tanuki.Operations
{
	public class HTML
	{
		[Verb("html", HelpText = "Code Quality operations.")]
		public class Options
		{
			[Value(0)]
			public string inputPath { get; set; }
			[Option('o', "output", HelpText = "Path to output file/folder.", Required = false)]
			public string outputPath { get; set; }
			[Option("base-url")]
			public string baseUrl { get; set; }
			[Option("templates-dir")]
			public string templatesDir { get; set; }
			public string title { get; set; } = "Code Quality Report";
		}
		
		private readonly Options options;
		
		
		private string[] severityIcons;
		private string smellPartial;
		private string noIssuesPartial;
		private string documentTemplate;
		
		public HTML(Options options)
		{
			this.options = options;
		}
		
		void LoadResources()
		{
			var templatesDir = string.IsNullOrEmpty(options.templatesDir) ? "templates" : options.templatesDir;
			smellPartial = File.ReadAllText($"{templatesDir}/html/partials/smell.html");
			noIssuesPartial = File.ReadAllText($"{templatesDir}/html/partials/no-issues.html");
			documentTemplate = File.ReadAllText($"{templatesDir}/html/gl-code-quality-report.html");
			severityIcons = new string[5];
			severityIcons[0] = File.ReadAllText($"{templatesDir}/html/assets/severity-info.svg");
			severityIcons[1] = File.ReadAllText($"{templatesDir}/html/assets/severity-minor.svg");
			severityIcons[2] = File.ReadAllText($"{templatesDir}/html/assets/severity-major.svg");
			severityIcons[3] = File.ReadAllText($"{templatesDir}/html/assets/severity-critical.svg");
			severityIcons[4] = File.ReadAllText($"{templatesDir}/html/assets/severity-blocker.svg");
		}

		public void OnParse()
		{
			LoadResources();
			
			BuildHTML(options.inputPath);
		}
		
		void BuildHTML(string path)
		{
			var reportText = File.ReadAllText(path);
			var issues = JsonConvert.DeserializeObject<List<Issue>>(reportText)
				.OrderByDescending(x => Issue.SeverityToInt(x.severity))
				.ThenBy(x => x.code)
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
			
			var document = documentTemplate;
			
			// Set title
			document = GetValueRegex("project.name").Replace(document, options.title);
			
			// Filters
			document = GetValueRegex("filter.categories").Replace(document, ToOptionslist(categories));
			document = GetValueRegex("filter.engines").Replace(document, ToOptionslist(engines));
			
			string ToOptionslist(List<string> values)
			{
				return string.Join(Environment.NewLine, values.Select(ToOption));
				
				string ToOption(string c)
				{
					return $"<option value=\"{Macros.Slugify(c)}\">{c}</option>";
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
			document = GetValueRegex("smells").Replace(document, issuesSection);
			
			// Filters
			document = EmitFilters(document, categories, engines);
			
			Macros.WriteAllTextOrConsole(options.outputPath, document);
		}
		
		string SmellString(string smellPartial, Issue issue)
		{
			var text = smellPartial;
			text = ReplaceOrEmpty(text, "issue.severity", issue.severity, "info");
			text = ReplaceOrEmpty(text, "issue.source_path", issue.location.path);
			text = ReplaceOrEmpty(text, "issue.body", issue.body, string.Empty);
			text = ReplaceOrEmpty(text, "issue.category", issue.category);
			text = ReplaceOrEmpty(text, "issue.category_slug", Macros.Slugify(issue.category));
			text = ReplaceOrEmpty(text, "issue.engine_slug", Macros.Slugify(issue.engine));
			
			// Icons
			var iconIndex = Issue.SeverityToInt(issue.severity);
			text = GetValueRegex("issue.icon").Replace(text, severityIcons[iconIndex]);
			
			// Issue title
			string titleString = null;
			if (!string.IsNullOrEmpty(issue.code))
			{
				titleString = issue.code;
			}
			if (!string.IsNullOrWhiteSpace(issue.description))
			{
				titleString += " " + issue.description;
			}
			titleString = HttpUtility.HtmlEncode(titleString.Trim());
			text = ReplaceOrEmpty(text, "issue.title", titleString);
			
			// Issue "found in"
			string foundInString = null;
			if (issue.location is not null)
			{
				var sourceFileRelativePath = issue.location.path;
				var sourceFileRelativePathWithLineNumber = sourceFileRelativePath;
				if (issue.location?.lines?.begin is not null)
				{
					sourceFileRelativePathWithLineNumber += $"#L{issue.location.lines.begin}";
				}
				var sourceFileUrl = $"{options.baseUrl}/{sourceFileRelativePathWithLineNumber}";
				
				// Create HTML
				foundInString = $"Found in <a href=\"{sourceFileUrl}\">{sourceFileRelativePath}</a>";
				
				// Engine
				if (!string.IsNullOrEmpty(issue.engine))
				{
					var config = Config.Instance;
					var engineUrl = config?.GetEngineUrl(issue.engine) ?? string.Empty;
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
		
		string EmitFilters(string document, List<string> categories, List<string> engines)
		{
			// Locals
			categories = categories.Select(Macros.Slugify).ToList();
			engines = engines.Select(Macros.Slugify).ToList();
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
			
			return GetValueRegex("filter.css").Replace(document, writer.ToString());
		}
				
		static Regex GetValueRegex(string text, RegexOptions options = RegexOptions.Multiline)
		{
			text = Regex.Escape(text);
			return new Regex(@$"<%= {text} -?%>", options);
		}
	}
}