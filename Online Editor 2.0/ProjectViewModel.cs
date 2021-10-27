using System.Windows.Input;
using DataCommunication_ProjectData;
using Online_Editor.Util;
using Online_Editor_2._0.Util;

namespace Online_Editor
{
	public class ProjectViewModel
	{
		private int i;

		public ProjectViewModel(ICanvasFiller canvasFiller)
		{
			AddText = new RelayCommand(e =>
				canvasFiller.Add(new Text(0, i += 15, @"abcdefghijklmnopqrstuvwxyz1234567890*\:""./<|?>; !#%&(),'-@[]^_{}~+ABCDEFGHIJKLMNOPQRSTUVWXYZ" + '\n')));
		}

		public ICommand AddText { get; }
	}
}