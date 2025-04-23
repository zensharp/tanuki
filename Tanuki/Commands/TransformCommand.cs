using System.Text.RegularExpressions;
using CommandLine;
using Tanuki.Core;

namespace Tanuki.Commands
{
	public class TransformCommand
	{
		[Verb("transform", HelpText = "Transform a report into GitLab formats.")]
		public class Options
		{
			[Value(0)]
			public string inputPath { get; set; }
			[Option('o', "output", HelpText = "Path to destination file.", Required = false)]
			public string outputPath { get; set; }
			[Option("junit", HelpText = "Convert the input file into a JUnit XML report.")]
			public bool junit { get; set; }
			[Option("codeclimate", HelpText = "Convert the input file into a CodeClimate report.")]
			public bool codeclimate { get; set; }
			[Option("auto", HelpText = "Attempt to determine source format automatically.")]
			public bool auto { get; set; } = true;
		}
		
		private readonly Options options;
		private readonly Regex CodeQualityRegex = new Regex(@".json$");
		private readonly Regex TestReportRegex = new Regex(@".xml$");
		
		public TransformCommand(Options options)
		{
			this.options = options;
		}

		public void OnParse()
		{
			if (options.codeclimate)
			{
				TransformFromUnityProjectAuditor();
			}
			else if (options.junit)
			{
				TransformFromUnityTestRunner();
			}
			else if (options.auto)
			{
				if (CodeQualityRegex.IsMatch(options.inputPath))
				{
					TransformFromUnityProjectAuditor();
					return;
				}
				else if (TestReportRegex.IsMatch(options.inputPath))
				{
					TransformFromUnityTestRunner();
					return;
				}
				else
				{
					throw new System.InvalidOperationException("Cannot determine a format for the transformation.");
				}
			}
			else
			{
				throw new System.ArgumentException("Please specify a valid format for the transformation.");
			}
		}
			
		void TransformFromUnityProjectAuditor()
		{
			new UnityProjectAuditorTransformer()
				.Transform(options.inputPath, options.outputPath);
		}
		
		void TransformFromUnityTestRunner()
		{
			new UnityTestRunnerTransformer()
				.Transform(options.inputPath, options.outputPath);
		}
	}
}