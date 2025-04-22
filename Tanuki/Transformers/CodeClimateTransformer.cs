
namespace Tanuki.Transformers
{
	public class CodeClimateTransformer
	{
		
		public string TransformSeverity(string text)
		{
			if (text.Equals("info", System.StringComparison.InvariantCultureIgnoreCase))
			{
				return "info";
			}
			if (text.Equals("moderate", System.StringComparison.InvariantCultureIgnoreCase))
			{
				return "minor";
			}
			if (text.Equals("major", System.StringComparison.InvariantCultureIgnoreCase))
			{
				return "major";
			}
			
			return text.ToLower();
		}
	}
}