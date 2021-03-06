using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Client;
using Online_Editor.Util;
using Online_Editor_2._0.Util;
using DataCommunication;
using Newtonsoft.Json;
using DataCommunication_ProjectData;

namespace Online_Editor
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private static ClientMain client;
		public delegate void CloseLogin(List<string> projects, string username);
		public static CloseLogin closeLogin;
		private static IWindowClose loginClose;
		private bool _loggedIn;
		private Visibility _wantToMakeProject = Visibility.Hidden;

		private string UserName;

		public string NewProjectName
		{
			get => _newProjectName;
			set { _newProjectName = value; NotifyPropertyChanged(); }
		}

		public Visibility WantToMakeProject
		{
			get => _wantToMakeProject;
			set { _wantToMakeProject = value; NotifyPropertyChanged(); if (value == Visibility.Hidden) NewProjectName = ""; }
		}
		public bool LoggedIn => _loggedIn;
		public bool LoggedOut => !_loggedIn;

		public ICommand CancelMakeProject => _cancelMakeProject ??= new RelayCommand(e => WantToMakeProject = Visibility.Hidden);
		public ICommand RequestMakeProject => _requestMakeProject ??= new RelayCommand(async e =>
		{
			if (NewProjectName == null || NewProjectName == "") return;
			await client.SendSegments(new ByteData(Messages.CreateProject(NewProjectName)));
			ByteData data = new ByteData(await client.Read());
			if (data.Id == Messages.Codes.ResponseOK)
			{
				List<string> list = await RequestPages();
				Values.Clear();
				foreach (string s in list)
				{
					Values.Add(new { path = s });
				}
				NotifyPropertyChanged();
			}
		});

		private dynamic selectedProject;
		public object SelectedProject
		{
			get => selectedProject;
			set { selectedProject = value; NotifyPropertyChanged(); }
		}


		private ICommand _makeNewProject;
		public ICommand MakeNewProject => _makeNewProject ??= new RelayCommand(e => WantToMakeProject = Visibility.Visible);

		public ICommand Login { get; } = new RelayCommand(async e =>
		{
			if (!client.isConnected) await client.ConnectToServerAsync();

			Login login = new Login { ShowInTaskbar = false, Owner = Application.Current.MainWindow, DataContext = new LoginViewModel(client, closeLogin) };

			loginClose = login;
			login.ShowDialog();

		});



		private void CloseLoginWindow(List<string> projects, string username)
		{
			Debug.WriteLine("loading projects");
			selectedProject = null;
			loginClose.CloseWindow();
			Values.Clear();
			foreach (string s in projects)
			{
				if (!string.IsNullOrEmpty(s))
					Values.Add(new { path = s });
			}

			_loggedIn = true;
			UserName = username;
			client.name = username;
			NotifyPropertyChanged(nameof(LoggedIn));
			NotifyPropertyChanged(nameof(LoggedOut));
			NotifyPropertyChanged();
		}

		private ICommand openProjectCommand;
		public ICommand OpenProjectCommand => openProjectCommand ??= new RelayCommand(async e => await OpenProject());

		private Visibility isvisible = Visibility.Visible;
		public Visibility IsVisible
		{
			get => isvisible;
			set { isvisible = value; NotifyPropertyChanged(); }
		}

		private bool showTaskbar;
		public bool ShowTaskbar { get { return showTaskbar; } set { showTaskbar = value; NotifyPropertyChanged(); } }

		public delegate void Back();
		public Back back;

		private ProjectView projectView;
		public delegate void UpdatePage(Element oldElement, Element newElement);
		public UpdatePage updatePage;
		public async Task OpenProject()
		{
			// Open project.
			if (selectedProject == null || selectedProject.path == null) return;
			await client.SendSegments(new ByteData(Messages.RequestPage(selectedProject.path)));

			if (ByteData.TryParse(out var data, await client.ReadSegments()))
			{
				//open the project view and load the project.
				if (data.Id == Messages.Codes.RequestPageResponse)
				{
					Debug.WriteLine(data.Message);

					Page page = JsonConvert.DeserializeObject<Page>(data.Message, new JsonSerializerSettings
					{
						TypeNameHandling = TypeNameHandling.Objects,
						SerializationBinder = new ElementsTypeBinder()
					});

					projectView = new ProjectView();
					var viewmodel = new ProjectViewModel(projectView, page, back, client);
					projectView.updatePage += viewmodel.UpdatePage;
					projectView.DataContext = viewmodel;
					projectView.Closed += viewmodel.Window_Closed;

					IsVisible = Visibility.Hidden;
					ShowTaskbar = false;
					projectView.ShowDialog();
				}
				else
				{
					// Did not find page or did not have permission (look if client has permission to see on server side to not show pointless projects.
					// Add a folder in each project for permission to see and edit project.
				}
			}
	}

		public void CloseProject()
		{
			projectView.Close();
			Debug.WriteLine("closed other");
			IsVisible = Visibility.Visible;
			ShowTaskbar = true;
		}

		public async Task<List<string>> RequestPages()
		{
			await client.SendSegments(new ByteData(Messages.RequestPages()));
			ByteData data = new ByteData(await client.Read());
			return JsonConvert.DeserializeObject<List<string>>(data.Message);
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

		public MainWindowViewModel(IClosable mainWindow)
		{
			this.ClosableWindow = mainWindow;
			closeLogin += CloseLoginWindow;
			Values = new ObservableCollection<object>();
			client = new ClientMain("localhost", 34192);
			_loggedIn = false;
			int connectionAttempts = 0;
			while (connectionAttempts++ <= 2 && !client.ConnectToServer()) { }
			this.UserName = "";
			ShowTaskbar = true;
			back += CloseProject;
			//GetFiles(Environment.CurrentDirectory);
		}

		private readonly IClosable ClosableWindow;

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
		private RelayCommand _cancelMakeProject;
		private RelayCommand _requestMakeProject;
		private string _newProjectName;

		public void Window_Closed(object sender, EventArgs e)
		{
			client.Dispose();
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}