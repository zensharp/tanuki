using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tanuki.CodeQuality.Models;

namespace Tanuki.Transformers
{
	public class Transform
	{
		[Verb("transform", HelpText = "Transforms a report into a Code Climate report.")]
		public class Options
		{
			[Option('i', "input", HelpText = "Source file path")]
			public string input { get; set; }
			[Option('o', "output", HelpText = "Destination file path")]
			public string output { get; set; }
			[Option("from", HelpText = "Source format")]
			public string from { get; set; }
		}
		
		private readonly Options options;
		private CodeClimateTransformer transformer;
		
		public Transform(Options options)
		{
			this.options = options;
			transformer = new CodeClimateTransformer();
		}

		public void OnParse()
		{
			var srcText = File.ReadAllText(options.input);
			
			// Strip any leading/trailing text
			var match = Regex.Match(srcText, @"^.*?(?<json>{.*}).*$", RegexOptions.Singleline);
			if (match.Success)
			{
				srcText = match.Groups["json"].Value;	
			}
			
			var jobject = JObject.Parse(srcText);
			
			var models = new List<Issue>();
			foreach (var t in jobject["issues"])
			{
				// Parse severity
				var severity = transformer.TransformSeverity(t["severity"].ToString());
				// Parse location
				Issue.Location location = null;
				try
				{
					location = new Issue.Location()
					{
						path = t["location"]["path"].ToString(),
					};
					int begin = int.Parse(t["location"]["line"].ToString());
					location.lines = new Issue.Location.Lines();
					location.lines.begin = begin;
				}
				catch
				{
					
				}
				
				// Create object
				var model = new Issue()
				{
					code = t["descriptorId"].ToString(),
					category = t["category"].ToString(),
					description = t["description"].ToString(),
					location = location,
					engine = "Project Auditor",
					severity = severity,
				};
				
				// Compute fingerprint
				var hashString = model.code;
				hashString += model.location?.path;
				hashString += model.location?.lines?.begin;
				model.fingerprint = Macros.CreateMD5Hash(hashString);
				
				models.Add(model);
			}
			
			var destText = JsonConvert.SerializeObject(models);
			File.WriteAllText(GetOutputPath(), destText);
		}
		
		string GetOutputPath()
		{
			if (string.IsNullOrEmpty(options.output))
			{
				return "codeclimate.json";
			}
			
			return options.output;
		}
	}
}