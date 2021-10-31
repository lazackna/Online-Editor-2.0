using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Client;
using DataCommunication;
using DataCommunication_ProjectData;
using Online_Editor.Util;
using Online_Editor_2._0.Util;

namespace Online_Editor
{
	public class ProjectViewModel
	{
		private MainWindowViewModel.Back back;
		private Page selectedPage;
		private ClientMain client;

		private Element _selectedElement;
		private ICommand _addElement;
		public ICommand AddElement => _addElement ??= new RelayCommand(e =>
		{
			if (e is string s)
			{
				if (s == typeof(Text).Name) _selectedElement = new Text(-1, -1, "");
			}
		});

		private ICommand _canvasClicked;
		public ICommand CanvasClicked => _canvasClicked ??= new RelayCommand(e =>
		{
			Console.WriteLine(e);
			if (_selectedElement != null && _selectedElement.GetX() == -1 && _selectedElement.GetY() == -1)
			{

			}
		});


		public ProjectViewModel(ICanvasFiller canvasFiller, Page page, MainWindowViewModel.Back back, ClientMain client)
		{
			this.back = back;
			this.selectedPage = page;
			this.client = client;
			foreach (var element in page.Elements) canvasFiller.Add(element);
		}

		public void UpdatePage(Element oldElement, Element newElement)
		{
			//newElement.SetLocked(client.name);
			selectedPage.Elements.Remove(oldElement);
			selectedPage.Elements.Add(newElement);
			UploadPage(selectedPage);
		}

		public async Task UploadPage(Page page)
		{
			await client.SendSegments(new ByteData(Messages.UploadPage(page)));
		}

		private ICommand goBack;
		public ICommand GoBack => goBack ??= new RelayCommand(async e =>
		{
			back();
		});

		public void Window_Closed(object sender, EventArgs e)
		{
			client.Dispose();
		}
	}
}