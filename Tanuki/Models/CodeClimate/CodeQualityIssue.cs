using System;

namespace Tanuki.Models.CodeClimate
{
	[Serializable]
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
		public string linter;
		public string body;
		
		[Serializable]
		public class Location
		{
			public string path;
			public Lines lines;
			
			[Serializable]
			public class Lines
			{
				public int begin;
			}
		}
	}
}