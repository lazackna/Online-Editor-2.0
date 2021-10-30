using System;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace DataCommunication_ProjectData
{
	public class ImageConverter : Newtonsoft.Json.JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Bitmap);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var m = new MemoryStream(Convert.FromBase64String((string) reader.Value));
			return (Bitmap) System.Drawing.Image.FromStream(m);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			Bitmap bmp = (Bitmap) value;
			MemoryStream m = new MemoryStream();
			bmp.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);

			writer.WriteValue(Convert.ToBase64String(m.ToArray()));
		}
	}
}