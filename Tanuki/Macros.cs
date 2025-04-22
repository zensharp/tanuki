using System.IO;
using System.Text;

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
		
		public static void CopyDirectory(string sourceDir, string destinationDir)
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDir);

			if (!dir.Exists)
			{
					throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
			}

			// If the destination directory doesn't exist, create it.
			Directory.CreateDirectory(destinationDir);

			// Copy files
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
					string tempPath = Path.Combine(destinationDir, file.Name);
					file.CopyTo(tempPath, overwrite: true);
			}

			// Copy subdirectories
			DirectoryInfo[] subDirs = dir.GetDirectories();
			foreach (DirectoryInfo subDir in subDirs)
			{
				string tempPath = Path.Combine(destinationDir, subDir.Name);
				CopyDirectory(subDir.FullName, tempPath);
			}
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
	}
}