using System.Linq;
using System.Xml.Linq;

namespace Tanuki.Core
{
	public class UnityTestRunnerTransformer
	{	
		public void Transform(string inputPath, string outputPath)
		{
			var document = XDocument.Load(inputPath);

			var rootElement = new XElement("testsuites");
			var testSuites = document
				.Descendants("test-suite")
				.Where(x => x.Attribute("type").Value == "TestFixture");
			foreach (var testSuite in testSuites)
			{
				var suiteName = testSuite.Attribute("fullname").Value;
				var testSuiteElement = new XElement("testsuite");
				rootElement.Add(testSuiteElement);

				var testCases = testSuite
					.Descendants("test-case");
				foreach (var testCase in testCases)
				{
					var testCaseElement = new XElement("testcase");
					testCaseElement.SetAttributeValue("name", testCase.Attribute("fullname").Value);
					testCaseElement.SetAttributeValue("classname", suiteName);
					testCaseElement.SetAttributeValue("time", testCase.Attribute("duration").Value);
					testSuiteElement.Add(testCaseElement);

					var hasFailure = testCase.Elements("failure").Any();
					if (hasFailure)
					{
						var failure = testCase.Element("failure");
						var statusString = testCase.Attributes().Any(IsErrorLabel) ? "error" : "failure";

						var statusElement = new XElement(statusString);
						testCaseElement.Add(statusElement);

						statusElement.Value = failure.Element("message").Value.Trim();

						static bool IsErrorLabel(XAttribute attribute)
						{
							if (attribute.Name == "label" && attribute.Value == "Error")
							{
								return true;
							}

							return false;
						}
					}
				}
			}

			var outputText = rootElement.ToString();
			Macros.WriteAllTextOrConsole(outputPath, outputText);
		}
	}
}