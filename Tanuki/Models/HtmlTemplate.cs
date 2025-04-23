using System.IO;
using System.Reflection;

namespace Tanuki.Models
{
	
	public class HtmlTemplate
	{
		public string document;
		public string smellPartial;
		public string noIssuesPartial;
		public string[] severityIcons;
		
		public HtmlTemplate()
		{
			
		}
		
		public void LoadResources()
		{
			static string ResourceReadAllText(Assembly assembly, string resourceName)
			{
				using Stream stream = assembly.GetManifestResourceStream(resourceName);
				if (stream == null)
				{
					throw new FileNotFoundException($"Resource {resourceName} not found.");
				}

				using StreamReader reader = new StreamReader(stream);
				return reader.ReadToEnd();
			}
			
			var assembly = Assembly.GetExecutingAssembly();
			// HTML
			document = ResourceReadAllText(assembly, "Tanuki.templates.html.gl-code-quality-report.html");
			smellPartial = ResourceReadAllText(assembly, "Tanuki.templates.html.partials.smell.html");
			noIssuesPartial = ResourceReadAllText(assembly, "Tanuki.templates.html.partials.no-issues.html");
			// Icons
			severityIcons = new string[5];
			severityIcons[0] = ResourceReadAllText(assembly, "Tanuki.templates.html.assets.severity-info.svg");
			severityIcons[1] = ResourceReadAllText(assembly, "Tanuki.templates.html.assets.severity-minor.svg");
			severityIcons[2] = ResourceReadAllText(assembly, "Tanuki.templates.html.assets.severity-major.svg");
			severityIcons[3] = ResourceReadAllText(assembly, "Tanuki.templates.html.assets.severity-critical.svg");
			severityIcons[4] = ResourceReadAllText(assembly, "Tanuki.templates.html.assets.severity-blocker.svg");
		}
	}
}