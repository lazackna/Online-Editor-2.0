﻿using Client;
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
		private ClientMain client;
		public LoginViewModel(ClientMain client)
		{
			this.client = client;
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
			} else {
				// Could not log in.
			}
			
			//await this.client.Read();
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
