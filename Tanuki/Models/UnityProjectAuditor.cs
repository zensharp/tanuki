namespace Tanuki.Models
{
	public static class UnityProjectAuditor
	{
		public static string TransformSeverity(string text)
		{
			if (text.Equals("info", System.StringComparison.InvariantCultureIgnoreCase))
			{
				return "info";
			}
			if (text.Equals("moderate", System.StringComparison.InvariantCultureIgnoreCase))
			{
				return "major";
			}
			if (text.Equals("major", System.StringComparison.InvariantCultureIgnoreCase))
			{
				return "critical";
			}
			
			return text.ToLower();
		}
	}
}