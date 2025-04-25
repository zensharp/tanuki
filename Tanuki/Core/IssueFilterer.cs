
using System.Text.RegularExpressions;
using Tanuki.Models.CodeClimate;

namespace Tanuki.Core
{
	public class IssueFilterer
	{
		private Regex selectorRegex;
		private string template;
		private bool negate;
		
		public IssueFilterer(string filterExpression)
		{
			var parseRegex = new Regex(@"^(?<left>.+)\s*(?<operator>==|!=)\s*(?<right>.+)$");
			var match = parseRegex.Match(filterExpression);
			template = match.Groups["left"].Value;
			selectorRegex = new Regex(match.Groups["right"].Value);
			negate = match.Groups["operator"].Value == "!=";
		}
		
		public bool IsMatch(Issue issue)
		{
			var text = template;
			text = text.Replace(".code", issue.code ?? string.Empty);
			text = text.Replace(".body", issue.body ?? string.Empty);
			text = text.Replace(".category", issue.category ?? string.Empty);
			text = text.Replace(".description", issue.description ?? string.Empty);
			text = text.Replace(".fingerprint", issue.fingerprint ?? string.Empty);
			text = text.Replace(".linter", issue.linter ?? string.Empty);
			text = text.Replace(".location.path", issue.location?.path ?? string.Empty);
			
			return selectorRegex.IsMatch(text) == !negate;
		}
	}
}