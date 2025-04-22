using System;
using System.IO;
using System.Text;
using Tanuki.Core;

namespace Tanuki
{
	public static class Macros
	{
		public static string CreateMD5Hash(string input)
		{
			var md5 = System.Security.Cryptography.MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hashBytes = md5.ComputeHash(inputBytes);

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hashBytes.Length; i++)
			{
				sb.Append(hashBytes[i].ToString("X2"));
			}
			return sb.ToString();
		}
		
		public static string Slugify(string x)
		{
			if (string.IsNullOrEmpty(x))
			{
				return x;
			}
			
			x = x.Replace(" ", "_");
			x = x.ToLower();
			return x;
		}
	
		public static void WriteAllTextOrConsole(string path, string content)
		{
			if (string.IsNullOrEmpty(path))
			{
				Console.WriteLine(content);
			}
			else
			{
				var directory = Path.GetDirectoryName(path);
				if (!string.IsNullOrEmpty(directory))
				{
					Directory.CreateDirectory(directory);
				}
				
				File.WriteAllText(path, content);
			}
		}
	
		public static string GetLinterUrl(string linter)
		{
			var config = Config.Instance;
			
			linter = Slugify(linter);
			var url = config?.GetLinterUrl(linter) ?? null;
			if (string.IsNullOrEmpty(url))
			{
				switch (linter)
				{
					case "project_auditor":
						url = "https://docs.unity3d.com/Packages/com.unity.project-auditor@1.0/manual/index.html";
						break;
					case "enforcer":
						url = "https://github.com/zensharp/enforcer";
						break;
				}
			}
			
			return url;
		}
	}
}