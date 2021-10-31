using System.IO;

namespace Storage
{
	public abstract class PictureStorage
	{
		protected static readonly string ResourceRoot = Path.Combine("Resources", "pictures");

		protected abstract void GetFiles(string path);
	}
}