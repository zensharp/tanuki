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
			foreach (var t in json["issues"])
			{
				// Parse severity
				var severity = UnityProjectAuditor.TransformSeverity(t["severity"].ToString());
				
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
				var model = new Issue()
				{
					check_name = t["descriptorId"].ToString(),
					category = t["category"].ToString(),
					description = t["description"].ToString(),
					location = location,
					linter = "Project Auditor",
					severity = severity,
				};
				
				// Compute fingerprint
				var hashString = model.check_name;
				hashString += model.location?.path;
				hashString += model.location?.lines?.begin;
				model.fingerprint = Macros.CreateMD5Hash(hashString);
				
				issues.Add(model);
			}

			var formatting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, };
			var outputText = JsonConvert.SerializeObject(issues, formatting);
			Macros.WriteAllTextOrConsole(outputPath, outputText);
		}
	}
}