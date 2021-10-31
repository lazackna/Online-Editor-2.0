using Newtonsoft.Json;

namespace DataCommunication_ProjectData
{
	public class TextElement : Element, ITextProvider
	{
		[JsonProperty]
		public string _value;

		public TextElement(int x, int y, string value) : base(x, y) => _value = value;

		public string GetText() => _value;
	}
}
