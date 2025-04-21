namespace Tanuki.Models
{
	public class CodeQualityReport
	{
		public class Issue
		{
			public string code;
			public string description;
			public string fingerprint;
			public string severity;
			public string category;
			public string engine;
			public Location location;
			
			public class Location
			{
				public string path;
				public Lines lines;
				
				public class Lines
				{
					public int begin;
				}
			}
		}
	}
}