using System.Windows.Input;
using DataCommunication_ProjectData;
using Online_Editor.Util;
using Online_Editor_2._0.Util;

namespace Online_Editor
{
	public class ProjectViewModel
	{
		private MainWindowViewModel.Back back;
		public ProjectViewModel(ICanvasFiller canvasFiller, Page page, MainWindowViewModel.Back back)
		{
			this.back = back;
			foreach (var element in page.Elements) canvasFiller.Add(element);
		}

		private ICommand goBack;
		public ICommand GoBack => goBack ??= new RelayCommand(async e =>
		{
			back();
		});
	}
}