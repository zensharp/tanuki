using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;
using Newtonsoft.Json;
using Tanuki.Core;
using Tanuki.Models.CodeClimate;

namespace Tanuki.Commands
{
	public partial class FingerprintCommand
	{
		[Verb("fingerprint", HelpText = "Generate automatic fingerprints.")]
		public class Options
		{
			[Value(0, HelpText = "The Code Quality report.", Required = true)]
			public string inputPath { get; set; }
			[Option('o', "output", HelpText = "Path to destination file.", Required = false)]
			public string outputPath { get; set; }
		}

		static readonly Regex MetaGuid = GetGuidRegex();
		[GeneratedRegex(@"guid:\s*(?<value>\w+)")]
		private static partial Regex GetGuidRegex();

		private readonly Options options;

		public FingerprintCommand(Options options)
		{
			this.options = options;
		}

		public void OnParse()
		{			
			var reportText = File.ReadAllText(options.inputPath);

			var issues = JsonConvert.DeserializeObject<List<Issue>>(reportText)
				.ToList();

			GenerateFingerprints(issues);

			var formatting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, };
			var outputText = JsonConvert.SerializeObject(issues, formatting);
			Macros.WriteAllTextOrConsole(options.outputPath, outputText);
		}

		void GenerateFingerprints(List<Issue> issues)
		{
			// Group issues by {check_name, filepath}
			var groupedIssues = issues.GroupBy(x => (x.check_name, x.location.path));

			foreach (var checkAndFileGroup in groupedIssues)
			{
				var baseFingerprint = checkAndFileGroup.Key.check_name;

				// Append META guid
				string guid = GetUnityGuid(checkAndFileGroup.Key.path);
				if (!string.IsNullOrEmpty(guid))
				{
					baseFingerprint += $" in {guid}";
				}

				// Append code fingerprint
				string GetCodeTextHash(Issue x)
				{
					var codeText = ReadCodeText(x.location);
					if (string.IsNullOrEmpty(codeText)) return null;

					return Macros.CreateMD5Hash(codeText);
				}
				var codeGroups = checkAndFileGroup.GroupBy(GetCodeTextHash);
				foreach (var codeGroup in codeGroups)
				{
					var hash = codeGroup.Key;

					var hasMultipleOccurances = codeGroup.Count() > 1;
					int distinctCounter = 0;
					foreach (var issue in codeGroup)
					{
						var fingerprint = baseFingerprint;

						// Append code hash
						if (!string.IsNullOrEmpty(hash))
						{
							fingerprint += $" with code {hash}";
						}
						// Append occurance index
						if (hasMultipleOccurances)
						{
							fingerprint += $" occurance #{++distinctCounter}";
						}

						issue.fingerprint = fingerprint;
					}
				}
			}
		}
	
		static string GetUnityGuid(string filepath)
		{
			var metafilePath = filepath + ".meta";
			if (!TryReadMetaGuid(metafilePath, out var guid)) return filepath; // No .META file found. Falling back to full file path...
			
			return guid;
		}
		
		static bool TryReadMetaGuid(string path, out string guid)
		{
			try
			{
				foreach (var line in File.ReadAllLines(path))
				{
					var m = MetaGuid.Match(line);
					if (m.Success)
					{
						guid = m.Groups["value"].Value;
						return true;
					}
				}
			}
			catch {}
			
			guid = default;
			return false;
		}

		static string ReadCodeText(Issue.Location location)
		{
			try
			{
				return File.ReadLines(location.path)
					.Skip(location.lines.begin - 1)
					.First();
			}
			catch {}
			
			return null;
		}
	}
}