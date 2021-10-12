using System;
using System.Collections.Generic;
using System.Text;

namespace DataCommunication_ProjectData

{
	public class Button : TextElement
	{
		public int Width { get; set; }
		public int Height { get; set; }

		public Button(int x, int y, string value, int width, int height) : base(x, y, value) { this.Width = width; this.Height = Height; }
	}
}
