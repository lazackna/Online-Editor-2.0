using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Client;
using Online_Editor.Util;
using Online_Editor_2._0.Util;

namespace Online_Editor
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private static ClientMain client;
		private static bool connected;
		public delegate void CloseLogin(List<string> projects);
		public static CloseLogin closeLogin;
		private static IWindowClose loginClose;

		private dynamic selectedProject;
		public object SelectedProject
		{
			get { return selectedProject; }
			set { selectedProject = value; NotifyPropertyChanged(); }
		}

		public ICommand Login { get; } = new RelayCommand(async e =>
		{
			
			if (!client.isConnected)
			{
				try
				{
					await client.ConnectToServerAsync();
					Login login = new Login { ShowInTaskbar = false, Owner = Application.Current.MainWindow};
					
					login.DataContext = new LoginViewModel(client, closeLogin);
					loginClose = login;
					login.ShowDialog();
					
				}
				catch
				{

				}
			}
			else
			{
				Login login = new Login { ShowInTaskbar = false, Owner = Application.Current.MainWindow };
				
				login.DataContext = new LoginViewModel(client, closeLogin);
				loginClose = login;
				login.ShowDialog();	
			}

		});

		

		private void CloseLoginWindow (List<string> projects)
		{
			selectedProject = null;
			loginClose.CloseWindow();
			Values.Clear();
			foreach (string s in projects) 
			{ 
				if (s != null && s.Length != 0)
				Values.Add(new { path = s });
			}
			NotifyPropertyChanged();
			

		}

		private ICommand openProjectCommand;
		public ICommand OpenProjectCommand
		{
			get
			{
				if (openProjectCommand == null)
				{
					openProjectCommand = new RelayCommand(e => OpenProject());
				}

				return openProjectCommand;
			}
			
		}

		public void OpenProject()
		{
			// Open project.
			
		}

		private ObservableCollection<object> _values;
		public ObservableCollection<object> Values
		{
			get => _values;
			set
			{
				_values = value;
				NotifyPropertyChanged();
			}
		}

		private Model model;

		public MainWindowViewModel()
		{
			this.model = new Model();
			closeLogin += CloseLoginWindow;
			Values = new ObservableCollection<object>();
			client = new ClientMain("localhost", 34192);
			int connectionAttempts = 0;
			while (connectionAttempts <= 2 && !client.ConnectToServer())
			{
				connectionAttempts++;
			}

			//GetFiles(Environment.CurrentDirectory);
		}

		//private async void GetFiles(string path)
		//{
		//	foreach (var d in Directory.GetDirectories(path))
		//	{
		//		GetFiles(d);
		//	}

		//	var files = Directory.GetFiles(path);
		//	foreach (var f in files)
		//	{
		//		Values.Add(new { path = f });
		//		await Task.Delay(10);
		//	}
		//}

		private ICommand _sendMessage;
		public ICommand SendMessage => _sendMessage ??= new RelayCommand(param => model.SendMessage());

		public void Window_Closed(object sender, EventArgs e)
		{

		}

		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}