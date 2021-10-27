using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using DataCommunication_ProjectData;
using Online_Editor_2._0.Util;
using Storage;
using Button = DataCommunication_ProjectData.Button;
using Image = System.Windows.Controls.Image;

namespace Online_Editor
{
	/// <summary>
	/// Interaction logic for ProjectView.xaml
	/// </summary>
	public partial class ProjectView : Window, ICanvasFiller
	{
		public ProjectView()
		{
			InitializeComponent();
			DataContext = new ProjectViewModel(this);

			Add(new Button(10, 10, "Merijn en Jasper Project"));
		}

		public void Add(Element element)
		{
			if (element is IImageProvider image) RenderImage(image);

			if (element is ITextProvider text) RenderText(text);
		}

		private void RenderImage(IImageProvider element)
		{
			var image = element.GetImage();
			var x = element.GetX();
			var y = element.GetY();
			var xOff = (element is IOffsetProvider xOffset) ? xOffset.GetOffsetX() : 0;
			var yOff = (element is IOffsetProvider yOffset) ? yOffset.GetOffsetY() : 0;

			var src = Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(),
				IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

			var bmp = new Image { Source = src };
			Canvas.SetLeft(bmp, x + xOff);
			Canvas.SetTop(bmp, y + yOff);
			Canvas.Children.Add(bmp);
		}

		private void RenderText(ITextProvider element)
		{
			var text = element.GetText();
			var x = element.GetX();
			var y = element.GetY();

			var symbols = SymbolStorage.Instance;

			foreach (var t in text)
			{
				var src = Imaging.CreateBitmapSourceFromHBitmap(symbols.GetImage(t).GetHbitmap(),
					IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

				var bmp = new Image {Source = src};
				Canvas.SetLeft(bmp, x);
				Canvas.SetTop(bmp, y);
				Canvas.Children.Add(bmp);

				x += src.PixelWidth;
			}
		}
	}
}
