using System;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using Storage;

namespace DataCommunication_ProjectData
{
	public class Image : Element, IImageProvider
	{
		[JsonProperty]
		[JsonConverter(typeof(ImageToJSONConverter))]
		public Bitmap _image;

		private Image(int x, int y, string base64Image) : base(x, y)
		{
			var bytes = Convert.FromBase64String(base64Image);
			using var ms = new MemoryStream(bytes);
			try
			{
				_image = new Bitmap(ms);
			}
			catch
			{
				_image = ImagePictureStorage.Instance.Image;
			}
		}

		~Image()
		{
			_image.Dispose();
		}

		public Bitmap GetImage() => _image;
		public int GetWidth() => _image.Width;
		public int GetHeight() => _image.Height;
	}
}