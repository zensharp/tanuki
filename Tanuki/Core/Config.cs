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
			
			try
			{
				var configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "tanuki.yml");
				var text = File.ReadAllText(configPath);
				instance = new DeserializerBuilder()
					.Build()
					.Deserialize<Config>(text);
			}
			catch
			{
				
			}
			finally
			{
				triedLoadingConfig = true;
			}
		}
		
		public string GetLinterUrl(string key)
		{
			key = Macros.Slugify(key);
			foreach (var linter in linters)
			{
				if (linter.name == key)
				{
					return linter.url;
				}
			}
			
			return null;
		}
	}
}