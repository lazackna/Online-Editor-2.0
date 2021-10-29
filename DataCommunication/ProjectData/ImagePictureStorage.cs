using System.Drawing;
using System.IO;

namespace Storage
{
	public class ImagePictureStorage : PictureStorage
	{
		private static ImagePictureStorage _instance;
		public static ImagePictureStorage Instance => _instance ??= new ImagePictureStorage();

		public Bitmap Image { private set; get; }

		private ImagePictureStorage()
		{
			GetFiles(ResourceRoot);
		}

		~ImagePictureStorage()
		{
			Image.Dispose();
		}

		protected sealed override void GetFiles(string path)
		{
			Image = new Bitmap(System.Drawing.Image.FromFile(Path.Combine(path, "invalid_image.png")));
		}
	}
}