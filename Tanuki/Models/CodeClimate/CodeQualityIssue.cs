using System;

namespace Tanuki.Models.CodeClimate
{
	[Serializable]
	public class Issue
	{
		// CodeClimate specification
		public string check_name;
		public string description;
		public string fingerprint;
		public string severity;
		public Location location = new Location();
		// Extensions
		public string category;
		public string linter;
		public string body;
		
		[Serializable]
		public class Location
		{
			public string path;
			public Lines lines = new Lines();

			[Serializable]
			public class Lines
			{
				public int begin = 0;
			}
		}
	}
}
