using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Newtonsoft.Json;

namespace DataCommunication_ProjectData
{
	public class ImageToJSONConverter : JsonConverter
	{
		public override bool CanConvert(Type type) => type == typeof(Bitmap);

		public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer)
		{
			using var m = new MemoryStream(Convert.FromBase64String((string) reader.Value)) { Position = 0 };
			return Bitmap.FromStream(m);
		}

		[Obsolete]
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			using var bmp = (Bitmap) value;
			var bytes = (byte[]) new ImageConverter().ConvertTo(bmp, typeof(byte[]));
			writer.WriteValue(Convert.ToBase64String(bytes));
		}
	}
}