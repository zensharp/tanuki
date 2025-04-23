using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace Tanuki.Core
{
	[Serializable]
	public class Config
	{
		public static Config Instance
		{
			get
			{
				Require();
				return instance;
			}
		}
		private static Config instance;
		private static bool triedLoadingConfig;
		
		public List<Linter> linters;
		
		[Serializable]
		public class Linter
		{
			public string name;
			public string url;
		}
		
		static void Require()
		{
			if (triedLoadingConfig)
			{
				return;
			}
			
			var searchPaths = new List<string>()
			{
				".tanuki.yml",
				".tanuki/tanuki.yml",
				".config/tanuki.yml",
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "tanuki.yml"),
			};
			
			var deserializer = new DeserializerBuilder()
				.Build();
			foreach (var path in searchPaths)
			{
				try
				{
					var text = File.ReadAllText(path);
					instance = deserializer
						.Deserialize<Config>(text);
					break;
				}
				catch
				{
					
				}
			}
			
			triedLoadingConfig = true;
		}
		
		public string LookupLinterUrl(string key)
		{
			key = Macros.Slugify(key);
			foreach (var linter in linters)
			{
				if (Macros.Slugify(linter.name) == key)
				{
					return linter.url;
				}
			}
			
			return null;
		}
	}
}