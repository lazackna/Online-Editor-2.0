using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Online_Editor
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{

        protected override void OnStartup(StartupEventArgs e)
        {
	        var window = new MainWindow();

			var vm = new MainWindowViewModel();

			window.DataContext = vm;
			window.Closed += vm.Window_Closed;

			window.Show();
		}
    }
}
