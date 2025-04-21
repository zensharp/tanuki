namespace Tanuki.CodeQuality.Models
{
	public class Issue
	{
		// CodeClimate specification
		public string code;
		public string description;
		public string fingerprint;
		public string severity;
		public Location location;
		// Extensions
		public string category;
		public string engine;
		public string body;
		
		public class Location
		{
			public string path;
			public Lines lines;
			
			public class Lines
			{
				public int begin;
			}
		}
		
		public static int SeverityToInt(string severity)
		{
			if (severity.Equals("info", System.StringComparison.InvariantCultureIgnoreCase))
			{
				return 0;
			}
			if (severity.Equals("minor", System.StringComparison.InvariantCultureIgnoreCase))
			{
				return 1;
			}
			if (severity.Equals("major", System.StringComparison.InvariantCultureIgnoreCase))
			{
				return 2;
			}
			if (severity.Equals("critical", System.StringComparison.InvariantCultureIgnoreCase))
			{
				return 3;
			}
			if (severity.Equals("blocker", System.StringComparison.InvariantCultureIgnoreCase))
			{
				return 4;
			}
			
			return -1;
		}
	}
}