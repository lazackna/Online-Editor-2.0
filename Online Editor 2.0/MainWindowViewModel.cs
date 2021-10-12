using System;
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

namespace Online_Editor
{
	class MainWindowViewModel : INotifyPropertyChanged
	{
		private static ClientMain client;
		private static bool connected;

		public ICommand Login { get; } = new RelayCommand(async e =>
		{
			if (!client.isConnected)
			{
				try
				{
					await client.ConnectToServerAsync();
					Login login = new Login { ShowInTaskbar = false, Owner = Application.Current.MainWindow};
					login.DataContext = new LoginViewModel(client);
					login.ShowDialog();
				}
				catch
				{

				}
			}
			else
			{
				Login login = new Login { ShowInTaskbar = false, Owner = Application.Current.MainWindow };
				login.DataContext = new LoginViewModel(client);
				login.ShowDialog();
			}

		});

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
			Values = new ObservableCollection<object>();
			client = new ClientMain("localhost", 34192);
			int connectionAttempts = 0;
			while (connectionAttempts <= 2 && !client.ConnectToServer())
			{
				connectionAttempts++;
			}

			GetFiles(Environment.CurrentDirectory);
		}

		private async void GetFiles(string path)
		{
			foreach (var d in Directory.GetDirectories(path))
			{
				GetFiles(d);
			}

			var files = Directory.GetFiles(path);
			foreach (var f in files)
			{
				Values.Add(new { path = f });
				await Task.Delay(10);
			}
		}

		public void OpenPath(object sender, MouseButtonEventArgs e)
		{
			if (!(sender is Label label)) return;
			var path = Directory.GetParent(label.Content.ToString() ?? string.Empty);
			if (path != null) Process.Start("explorer.exe", path.ToString());
		}

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