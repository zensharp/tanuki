using System.IO;

namespace Tanuki
{
	public static class Macros
	{
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
	}
}