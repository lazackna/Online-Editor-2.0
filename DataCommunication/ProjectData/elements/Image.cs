using System;
using System.Collections.Generic;
using System.Text;

namespace DataCommunication_ProjectData
{
	public class Image : Element
	{
		public int Width { get; set; }
		public int Height { get; set; }

		public Image (int x, int y, int width, int height) :base(x,y){ this.Width = width; this.Height = height; }
	}
}