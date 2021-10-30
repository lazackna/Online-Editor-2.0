using System.Windows.Input;
using DataCommunication_ProjectData;
using Online_Editor.Util;
using Online_Editor_2._0.Util;

namespace Online_Editor
{
	public class ProjectViewModel
	{
		public ProjectViewModel(ICanvasFiller canvasFiller, Page page)
		{
			foreach (var element in page.Elements) canvasFiller.Add(element);
		}

		public ICommand GoBack { get; set; }
	}
}