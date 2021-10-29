namespace DataCommunication_ProjectData
{
	public abstract class TextElement : Element, ITextProvider
	{
		private readonly string _value;

		protected TextElement(int x, int y, string value) : base(x, y) => _value = value;

		public string GetText() => _value;
	}
}
