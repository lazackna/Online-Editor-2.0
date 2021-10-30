using System;
using System.Drawing;
using Newtonsoft.Json;
using Storage;

namespace DataCommunication_ProjectData
{
	public class Button : TextElement, IImageProvider, IOffsetProvider
	{
		private static readonly int offset = 3;

		[JsonProperty]
		[JsonConverter(typeof(ImageConverter))]
		public Bitmap _image;

		public Button(int x, int y, string value) : base(x, y, value)
		{
			var bps = ButtonPictureStorage.Instance;
			var ss = SymbolStorage.Instance;

			var text = GetText();
			if (string.IsNullOrEmpty(text)) _image = MakeImage(bps);
			else
			{
				var width = 0;
				foreach (var symbol in text) width += ss.GetImage(symbol).Width;

				width -= offset * 2;
				_image = MakeImage(bps, Math.Max(0, width));
			}
		}

		~Button()
		{
			_image.Dispose();
		}

		private Bitmap MakeImage(ButtonPictureStorage bps, int centerSegments = 0)
		{
			var image = new Bitmap(bps.Left, bps.Left.Width + centerSegments + bps.Right.Width, bps.Left.Height);

			for (var i = 0; i < bps.Left.Width; i++)
			for (var j = 0; j < image.Height; j++)
				image.SetPixel(i, j, bps.Left.GetPixel(i, j));

			for (var i = 0; i < centerSegments; i++)
			for (var j = 0; j < image.Height; j++)
				image.SetPixel(i + bps.Left.Width, j, bps.Center.GetPixel(0, j));

			for (var i = 0; i < bps.Right.Width; i++)
			for (var j = 0; j < image.Height; j++)
				image.SetPixel(i + bps.Left.Width + centerSegments, j, bps.Right.GetPixel(i, j));

			return image;
		}

		public Bitmap GetImage() => _image;
		public int GetWidth() => _image.Width;
		public int GetHeight() => _image.Height;
		public int GetOffsetX() => -offset;
		public int GetOffsetY() => -offset;
	}
}
