using System;
using System.Collections.Generic;
using System.Text;

namespace DataCommunication_ProjectData
{
	public abstract class TextElement : Element
	{
		public string Value { get; set; }

		public TextElement(int x, int y, string value) : base(x, y){ this.Value = value; }
	}
}
