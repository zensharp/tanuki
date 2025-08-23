using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace Tanuki.Core
{
	[Serializable]
	public class Tanukifile
	{
		[Serializable]
		public class Filter
		{
			public List<Exclude> exclude = new List<Exclude>();

			public class Exclude
			{
				[YamlMember(Alias = "if")]
				public List<string> conditions = new List<string>();
			}
		}

		public Filter filter = new Filter();
		
		public static Tanukifile Read(string path)
		{	
			Tanukifile tanukifile;
			if (File.Exists(path))
			{
				var deserializer = new DeserializerBuilder()
					.Build();
				var text = File.ReadAllText(path);
				tanukifile = deserializer
					.Deserialize<Tanukifile>(text);
			}
			else
			{
				tanukifile = new Tanukifile();
			}

			return tanukifile;
		}
	}
}