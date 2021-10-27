using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using DataCommunication_ProjectData;
using Online_Editor_2._0.Util;
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
		}

		public void Add(Element element)
		{
			if (element.HasImage()) RenderImage(element);

			if (element.HasText()) RenderText(element);
		}

		private void RenderImage(Element element)
		{
		}

		private void RenderText(Element element)
		{
			var text = element.GetText();
			var x = element.X;
			var y = element.Y;

			var symbols = SymbolStorage.Instance;

			foreach (var t in text)
			{
				var src = Imaging.CreateBitmapSourceFromHBitmap(symbols.GetImage(t).GetHbitmap(),
						IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

				var bmp = new Image { Source = src };
				Canvas.SetLeft(bmp, x);
				Canvas.SetTop(bmp, y);
				Canvas.Children.Add(bmp);

				x += src.PixelWidth;
			}
		}
	}
}
