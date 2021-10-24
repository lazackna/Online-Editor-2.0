using System;
using System.Collections.Generic;
using System.Text;

namespace DataCommunication_ProjectData
{
	public abstract class TextElement : Element
	{
		public string Value { get; set; }

		protected TextElement(int x, int y, string value) : base(x, y) => Value = value;

		public override bool HasText() => true;
		public override string GetText() => Value;
	}
}
