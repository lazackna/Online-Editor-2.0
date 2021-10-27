using System;
using System.Drawing;

namespace DataCommunication_ProjectData
{
	public class Image : Element, IImageProvider
	{
		private readonly int _width;
		private readonly int _height;

		public Image(int x, int y, int width, int height) : base(x, y) { _width = width; _height = height; }

		public Bitmap GetImage()
		{
			throw new NotImplementedException();
		}
		public int GetWidth() => _width;
		public int GetHeight() => _height;
	}
}