
using System;
using System.Text.RegularExpressions;
using Tanuki.Models.CodeClimate;

namespace Tanuki.Core
{
	public class IssueFilterer
	{
		private Regex selectorRegex;
		private string template;
		private bool negate;
		private bool isWildcard;
		
		public IssueFilterer(string filterExpression)
		{
			if (filterExpression == "*")
			{
				isWildcard = true;
				return;
			}
			
			var parseRegex = new Regex(@"^(?<left>.+)\s*(?<operator>==|!=)\s*(?<right>.+)$");
			var match = parseRegex.Match(filterExpression);
			if (!match.Success)
			{
				throw new ArgumentException($"Malformed expression: filterExpression");
			}
			template = match.Groups["left"].Value;
			selectorRegex = new Regex(match.Groups["right"].Value);
			negate = match.Groups["operator"].Value == "!=";
		}
		
		public bool IsMatch(Issue issue)
		{
			if (isWildcard)
			{
				return true;
			}
			
			var text = template;
			text = text.Replace(".check_name", issue.check_name ?? string.Empty);
			text = text.Replace(".body", issue.body ?? string.Empty);
			text = text.Replace(".category", issue.category ?? string.Empty);
			text = text.Replace(".description", issue.description ?? string.Empty);
			text = text.Replace(".fingerprint", issue.fingerprint ?? string.Empty);
			text = text.Replace(".linter", issue.linter ?? string.Empty);
			text = text.Replace(".severity", issue.severity ?? string.Empty);
			text = text.Replace(".location.path", issue.location?.path ?? string.Empty);
			
			return selectorRegex.IsMatch(text) == !negate;
		}
	}
}