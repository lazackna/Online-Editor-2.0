using System.Drawing;

namespace DataCommunication_ProjectData
{
	public interface IImageProvider : IPositionProvider, ISizeProvider
	{
		Bitmap GetImage();
	}
}