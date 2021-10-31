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
			_image = FromBase64(base64Image);
		}

		public Bitmap FromBase64(string base64Image)
		{
			var bytes = Convert.FromBase64String(base64Image);
			using var ms = new MemoryStream(bytes);
			try
			{
				return new Bitmap(ms);
			}
			catch
			{
				return ImagePictureStorage.Instance.Image;
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