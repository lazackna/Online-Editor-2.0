﻿using Client;
using Communication;
using DataCommunication;
using Newtonsoft.Json;
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
		private ClientMain client;
		private MainWindowViewModel.CloseLogin close;
		public LoginViewModel(ClientMain client, MainWindowViewModel.CloseLogin close)
		{
			this.client = client;
			this.close = close;
		}

		public string UserName { get; set; }
		public string PassWord { get; set; }
		private ICommand login;
		public ICommand Login
		{
			get
			{
				if (login == null) login = new RelayCommand(async e => await ClientLogin());
				return login;
			}
		}

		private ICommand makeAccount;
		public ICommand MakeAccount
		{
			get 
			{
				if (makeAccount == null) makeAccount = new RelayCommand(async e => await ClientMakeAccount());
				return makeAccount;
			}
		}

		public async Task ClientLogin()
		{
			
			await this.client.SendSegments(new ByteData(Messages.Login(UserName, PassWord)));
			ByteData data = new ByteData(await this.client.Read());
			if (data.Id == Messages.Codes.ResponseOK) {
				// Tell client that they are logged in and change screen.

				this.close(await RequestPages());
			} else {
				// Could not log in.
			}
			
			//await this.client.Read();
		}

		public async Task<List<string>> RequestPages()
		{
			await this.client.SendSegments(new ByteData(Messages.RequestPages()));
			ByteData data = new ByteData(await this.client.Read());
			return JsonConvert.DeserializeObject<List<string>>(data.Message);
		}

		public async Task ClientMakeAccount()
		{
			//await this.client.SendTest();
			await this.client.SendSegments(new ByteData(Messages.RequestAccount()));
			byte[] received = await this.client.Read();
			ByteData data = new ByteData(received);
			if (data.Id == Messages.Codes.ResponseOK) {
				await this.client.SendSegments(new ByteData(Messages.MakeAccount(UserName, PassWord)));
				data = new ByteData(await this.client.Read());

				if (!(data.Id == Messages.Codes.ResponseOK)) {
					// could not create account. Could be due to already existing or an error occured on the server.
				}
				
			} else {
				// not allowed to make account
			}
		}

		public async Task Ping()
		{
			await this.client.SendSegments(new ByteData(Messages.ClientPing()));
			ByteData data = new ByteData(await this.client.Read());
			Debug.WriteLine(data.Message);
		}

	}
}
