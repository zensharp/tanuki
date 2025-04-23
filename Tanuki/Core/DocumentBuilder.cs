using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Tanuki.Models;
using Tanuki.Models.CodeClimate;

namespace Tanuki.Core
{
	public class DocumentBuilder
	{
		private readonly HtmlTemplate template;
		private readonly Document document;
		private readonly string title;
		private readonly string baseUrl;
		
		public DocumentBuilder(HtmlTemplate template, string title, string baseUrl)
		{
			this.template = template;
			this.title = title;
			this.baseUrl = baseUrl;
			document = new Document(template.document);
		}
		
		public Document Build(List<Issue> issues)
		{
			// Gather categories, linters
			var categories = issues
				.Select(x => x.category)
				.Where(x => !string.IsNullOrEmpty(x))
				.Distinct()
				.ToList();
			var linters = issues
				.Select(x => x.linter)
				.Where(x => !string.IsNullOrEmpty(x))
				.Distinct()
				.ToList();
			
			// Rewrite title
			document.Replace("project.name", title);
			
			// Build filters CSS
			document.Replace("filter.css", BuildFiltersCss(categories, linters));
			
			// Build filters dropdown
			document.Replace("filter.categories", BuildHtmlOptionsList(categories));
			document.Replace("filter.engines", BuildHtmlOptionsList(linters));
			
			// Build smells rows
			document.Replace("smells", BuildSmellsList(issues));
			
			return document;
		}
			
		static string BuildHtmlOptionsList(List<string> options)
		{
			return string.Join(Environment.NewLine, options.Select(ToHtmlOption));
		}
		
		static string ToHtmlOption(string name)
		{
			return $"<option value=\"{Macros.Slugify(name)}\">{name}</option>";
		}
		
		static string BuildFiltersCss(List<string> categories, List<string> linters)
		{
			// Locals
			var writer = new StringWriter();
			categories = categories
				.Select(Macros.Slugify)
				.ToList();
			linters = linters
				.Select(Macros.Slugify)
				.ToList();
			var categoriesWithAll = categories
				.Prepend("all")
				.ToList();
			var lintersWithAll = linters
				.Prepend("all")
				.ToList();
			
			// category_i
			foreach (var category in categories)
			{
				writer.WriteLine($".filter-category-{category} > li,");	
			}
			// linter_i
			foreach (var linter in linters)
			{
				writer.WriteLine($".filter-engine-{linter} > li,");	
			}
			writer.WriteLine(".filter-none > li { display: none; }");
			writer.WriteLine();
			
			// category_* + linter_i
			foreach (var linter in linters)
			{
				writer.WriteLine($".filter-category-all.filter-engine-{linter} > li[data-engine=\"{linter}\"],");
			}
			// category_i + linter_*
			foreach (var category in categories)
			{
				writer.WriteLine($".filter-category-{category}.filter-engine-all > li[data-categories~=\"{category}\"],");
			}
			// category_i + linter_j
			foreach (var category in categoriesWithAll)
			{
				foreach (var linter in lintersWithAll)
				{
					writer.WriteLine($".filter-category-{category}.filter-engine-{linter} > li[data-categories~=\"{category}\"][data-engine=\"{linter}\"],");
				}
			}
			// category_* + linter_*
			writer.WriteLine(".filter-category-all.filter-engine-all > li { display: block; }");
			
			return writer.ToString();
		}
		
		string BuildSmellsList(List<Issue> issues)
		{
			string text;
			if (issues.Count == 0)
			{
				text = template.noIssuesPartial;
			}
			else
			{
				text = string.Empty;
				foreach (var issue in issues)
				{
					text += ListItem(issue);	
				}
			}
			
			return text;
			
			string ListItem(Issue issue)
			{
				var subDocument = new Document(template.smellPartial);
				
				// Icon
				subDocument.Replace("issue.icon", SeverityIcon());
				
				// Header
				subDocument.Replace("issue.title", Header());
				
				// Issue "found in"
				subDocument.Replace("issue.location", FoundIn());
				
				// Generic replacements
				subDocument.Replace("issue.body", issue.body);
				subDocument.Replace("issue.category_slug", Macros.Slugify(issue.category));
				subDocument.Replace("issue.engine_slug", Macros.Slugify(issue.linter));

				return subDocument.ToString();
				
				string Header()
				{
					string text = null;
					if (!string.IsNullOrEmpty(issue.code))
					{
						text = issue.code;
					}
					if (!string.IsNullOrWhiteSpace(issue.description))
					{
						text += " " + issue.description;
					}
					text = HttpUtility.HtmlEncode(text.Trim());
					
					return text;
				}
				
				string SeverityIcon()
				{
					if (!Enum.TryParse<Severity>(issue.severity, out var severity))
					{
						severity = Severity.major;
					}
					return template.severityIcons[(int)severity];
				}
				
				string FoundIn()
				{
					string text = null;
					if (issue.location is not null)
					{
						var sourceFileRelativePath = issue.location.path;
						var sourceFileRelativePathWithLineNumber = sourceFileRelativePath;
						if (issue.location?.lines?.begin is not null)
						{
							sourceFileRelativePathWithLineNumber += $"#L{issue.location.lines.begin}";
						}
						var sourceFileUrl = $"{baseUrl}/{sourceFileRelativePathWithLineNumber}";
						
						// Create HTML
						text = $"Found in <a href=\"{sourceFileUrl}\">{sourceFileRelativePath}</a>";
						
						// Linter
						if (!string.IsNullOrEmpty(issue.linter))
						{
							var linterUrl = Macros.GetLinterUrl(issue.linter);
							text += $" by <a href=\"{linterUrl}\">{issue.linter}</a>";
						}
					}
					
					return text;
				}
			}
		}
	}
}