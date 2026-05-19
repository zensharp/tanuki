namespace Tanuki.Models
{
	public static class UnityProjectAuditor
	{
		public static string TransformSeverity(string text)
		{
			if (text.Equals("major", System.StringComparison.InvariantCultureIgnoreCase))
			{
				return "critical";
			}
			if (text.Equals("moderate", System.StringComparison.InvariantCultureIgnoreCase))
			{
				return "major";
			}
			
			return "info";
		}
	}
}