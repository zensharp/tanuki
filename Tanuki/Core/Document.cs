
namespace Tanuki.Core
{
    public class Document
	{
		private string text;
		
		public Document(string text = null)
		{
			this.text = text ?? string.Empty;
		}
		
		public void Replace(string shortcode, string replacement, string fallback = null)
		{
			if (string.IsNullOrEmpty(replacement))
			{
				replacement = fallback ?? string.Empty;
			}
			
			text = Macros.ShortcodeRegex(shortcode).Replace(text, replacement);
		}

		public override string ToString()
		{
			return text;
		}
	}
}