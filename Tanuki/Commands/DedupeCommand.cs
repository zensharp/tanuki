using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tanuki.Core;
using Tanuki.Models.CodeClimate;

namespace Tanuki.Commands
{
	public class DedupeCommand
	{
		[Verb("dedupe", HelpText = "Removes duplicate issues")]
		public class Options
		{
			[Value(0, HelpText = "The Code Quality report.", Required = true)]
			public string inputPath { get; set; }
			[Option('o', "output", HelpText = "Path to destination file.", Required = false)]
			public string outputPath { get; set; }
		}

        class Comparer : IEqualityComparer<JObject>
        {
            public bool Equals(JObject? x, JObject? y)
			{
				return x.ToString().Equals(y.ToString());
			}

            public int GetHashCode([DisallowNull] JObject obj)
			{
				return obj.ToString().GetHashCode();
			}
        }

        private readonly Options options;
		
		public DedupeCommand(Options options)
		{
			this.options = options;
		}

        public void OnParse()
		{			
			var inputText = File.ReadAllText(options.inputPath);
			var issues = JArray.Parse(inputText)
				.Select(x => (JObject)x);

			// Keep unique elements
			var comparer = new Comparer();
			var outputIssues = issues
				.Distinct(comparer);

			var formatting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, };
			var outputText = JsonConvert.SerializeObject(outputIssues, formatting);
			Macros.WriteAllTextOrConsole(options.outputPath, outputText);
		}
	}
}