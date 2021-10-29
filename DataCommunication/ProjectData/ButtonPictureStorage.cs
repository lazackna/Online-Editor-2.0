using System.Drawing;
using System.IO;

namespace Storage
{
	public class ButtonPictureStorage : PictureStorage
	{
		private static ButtonPictureStorage _instance;
		public static ButtonPictureStorage Instance => _instance ??= new ButtonPictureStorage();

		public Bitmap Center { private set; get; }
		public Bitmap Left { private set; get; }
		public Bitmap Right { private set; get; }

		private ButtonPictureStorage()
		{
			GetFiles(Path.Combine(ResourceRoot, "button"));
		}

		~ButtonPictureStorage()
		{
			Center.Dispose();
			Left.Dispose();
			Right.Dispose();
		}

		protected sealed override void GetFiles(string path)
		{
			Center = new Bitmap(Image.FromFile(Path.Combine(path, "center.png")));
			Left = new Bitmap(Image.FromFile(Path.Combine(path, "left.png")));
			Right = new Bitmap(Image.FromFile(Path.Combine(path, "right.png")));
		}
	}
}