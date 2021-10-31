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
			back();
		}
	}
}