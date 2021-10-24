using System.Windows.Input;
using DataCommunication_ProjectData;
using Online_Editor.Util;
using Online_Editor_2._0.Util;

namespace Online_Editor
{
	public class ProjectViewModel
	{
		private ICanvasFiller _canvasFiller;
		private int i;

		public ProjectViewModel(ICanvasFiller canvasFiller)
		{
			_canvasFiller = canvasFiller;
			AddText = new RelayCommand(e =>
				_canvasFiller.Add(new Text(0, i += 15, "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890*\\:\"/<|?> ")));
		}

		public ICommand AddText { get; }
	}
}