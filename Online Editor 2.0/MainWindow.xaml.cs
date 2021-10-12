using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Online_Editor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void OpenPath(object sender, MouseButtonEventArgs e)
		{
			(DataContext as MainWindowViewModel)?.OpenPath(sender, e);
		}
	}
}
