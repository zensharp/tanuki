using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tanuki.Models;
using Tanuki.Models.CodeClimate;

namespace Tanuki.Core
{
	public class UnityProjectAuditorTransformer
	{
		public void Transform(string inputPath, string outputPath)
		{
			var srcText = File.ReadAllText(inputPath);
			
			// Strip any leading/trailing text
			var match = Regex.Match(srcText, @"^.*?(?<json>{.*}).*$", RegexOptions.Singleline);
			if (match.Success)
			{
				srcText = match.Groups["json"].Value;
			}
			
			var json = JObject.Parse(srcText);
			var issues = new List<Issue>();
			foreach (var t in json["m_Issues"])
			{
				var issue = new Issue();

				// Parse severity
				var severity = UnityProjectAuditor.TransformSeverity(t["severity"]["m_String"].ToString());
				
				// Parse location
				Issue.Location location = null;
				try
				{
					location = new Issue.Location()
					{
						path = t["location"]["path"].ToString(),
					};
					location.lines = new Issue.Location.Lines
					{
							begin = int.Parse(t["location"]["line"].ToString()),
					};
				}
				catch
				{
					
				}
				
				// Create object
				issue.check_name = t["descriptorId"]["m_AsString"].ToString();
				issue.category = t["category"]["m_String"].ToString();
				issue.description = t["description"].ToString();
				issue.location = location;
				issue.linter = "Project Auditor";
				issue.severity = severity;
				
				// Compute fingerprint
				var hashString = issue.check_name;
				hashString += issue.location?.path;
				hashString += issue.location?.lines?.begin;
				issue.fingerprint = Macros.CreateMD5Hash(hashString);
				
				issues.Add(issue);
			}

			var formatting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, };
			var outputText = JsonConvert.SerializeObject(issues, formatting);
			Macros.WriteAllTextOrConsole(outputPath, outputText);
		}
	}
}