using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Tanuki.Core;

namespace Tanuki
{
	public static class Macros
	{
		public static string CreateMD5Hash(string input)
		{
			byte[] inputBytes = Encoding.ASCII.GetBytes(input);
			byte[] hashBytes = System.Security.Cryptography.MD5.HashData(inputBytes);

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
			
			x = x.Replace(" ", string.Empty);
			x = x.ToLower();
			return x;
		}
		
		public static Regex ShortcodeRegex(string identifier, RegexOptions options = RegexOptions.Multiline)
		{
			identifier = Regex.Escape(identifier);
			return new Regex(@$"<%= {identifier} -?%>", options);
		}
	
		public static void WriteAllTextOrConsole(string path, string content)
		{
			if (string.IsNullOrEmpty(path))
			{
				// Print to STDOUT
				Console.WriteLine(content);
			}
			else
			{
				// Create destination and write to file
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
			string url;
			url = Config.Instance?.LookupLinterUrl(linter);
			if (string.IsNullOrEmpty(url))
			{
				url = LookupKnownLinter(linter);
			}
			
			return url;
		}
		
		public static string LookupKnownLinter(string linter)
		{
			switch (Slugify(linter))
			{
				case "projectauditor":
					return "https://docs.unity3d.com/Packages/com.unity.project-auditor@1.0/manual/index.html";
				case "enforcer":
					return "https://github.com/zensharp/enforcer";
			}
			
			return null;
		}
	}
}