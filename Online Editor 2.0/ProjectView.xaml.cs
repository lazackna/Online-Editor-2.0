using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DataCommunication_ProjectData;
using Online_Editor.Util;
using Online_Editor_2._0.Util;
using Storage;
using Button = System.Windows.Controls.Button;
using Image = System.Windows.Controls.Image;

namespace Online_Editor
{
	/// <summary>
	/// Interaction logic for ProjectView.xaml
	/// </summary>
	public partial class ProjectView : Window, ICanvasFiller
	{

		public MainWindowViewModel.UpdatePage updatePage { get; set; }
		public ProjectView()
		{
			InitializeComponent();
		}

		private Dictionary<UIElement, Element> dictionary = new Dictionary<UIElement, Element>();

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
			dictionary.Add(bmp, element as Element);
		}

		private void RenderText(ITextProvider element)
		{
			var text = element.GetText();
			var x = 0;
			var y = 0;

			var symbols = SymbolStorage.Instance;
			List<Image> list = new List<Image>();
			Canvas TextCanvas = new Canvas();
			foreach (var t in text)
			{
				var src = Imaging.CreateBitmapSourceFromHBitmap(symbols.GetImage(t).GetHbitmap(),
					IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

				var bmp = new Image {Source = src};
				Canvas.SetLeft(bmp, x);
				Canvas.SetTop(bmp, y);
				//TextCanvas.
				list.Add(bmp);
				TextCanvas.Children.Add(bmp);
				//dictionary.Add(bmp, element as Element);

				x += src.PixelWidth;
			}
			Canvas.SetLeft(TextCanvas, element.GetX());
			Canvas.SetTop(TextCanvas, element.GetY());
			Canvas.Children.Add(TextCanvas);
			dictionary.Add(TextCanvas, element as Element);
		}

		private Element _selectedElement;

		private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Source is Button b) {
				if (b.Content.ToString() == nameof(Text)) _selectedElement = new Text(-1, -1, "abc");
			}
		}

		private void True(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (_selectedElement != null && _selectedElement.GetX() == -1 && _selectedElement.GetY() == -1)
			{
				var pos = e.GetPosition(Canvas);
				_selectedElement._x = (int) pos.X;
				_selectedElement._y = (int) pos.Y;

				Add(_selectedElement);
				return;
			}

			if (_selectedElement == null)
				foreach (UIElement u in Canvas.Children)
					if (u.IsMouseOver)
					{
						Debug.WriteLine("found something");
						_selectedElement = dictionary[u];
						break;
					}
		}
	}
}