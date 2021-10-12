using Communication;
using DataCommunication;
using Online_Editor.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Online_Editor
{
	public class LoginViewModel
	{

		public LoginViewModel()
		{

		}

		public string UserName { get; set; }
		public string PassWord { get; set; }
		private ICommand login;
		public ICommand Login
		{
			get
			{
				if (login == null) login = new RelayCommand(async e => ClientLogin());
				return login;
			}
		}

		public async void ClientLogin()
		{
			Debug.WriteLine(UserName + ":" + PassWord);
		}

		public async void MakeAccount()
		{
			(byte, string) makeAccount = Messages.RequestAccount();

		}

	}
}
