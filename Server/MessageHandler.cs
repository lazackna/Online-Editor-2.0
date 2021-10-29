﻿using DataCommunication;
using DataCommunication_ProjectData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public class MessageHandler
	{
		private AccountManager manager;
		public NetworkClient client;
		private string name;
		public MessageHandler (NetworkClient client)
		{
			this.client = client;
			this.manager = new AccountManager();
			this.name = "";
		}

		public async Task Handle (ByteData data)
		{

			

			switch (data.GetMessageType())
			{
				case 0:
					await Login(data);
					break;
				case 1:
					await RequestAccount(data);
					break;
				case 2:
					await MakeAccount(data);
					break;
				case 11:
					await CreateProject(data);
					break;
				case 12:
					await CreatePage(data);
					break;
				case 20:
					await RequestPages(data);
					break;
				case 21:
					await RequestPage(data);
					break;
				case 22:
					UploadPage(data);
					break;
				case 23:
					RequestChangePage(data);
					break;
				case 24:
					UploadChangedPage(data);
					break;
				case 193:
					await Ping(data);
					break;
			}
		}

		public async Task Ping (ByteData array)
		{
			await this.client.Write(new ByteData(Messages.ServerPing()));
		}

		public void UploadChangedPage (ByteData array)
		{
			
		}

		public void RequestChangePage (ByteData array)
		{
			
		}

		public void UploadPage (ByteData array)
		{
			
		}

		public async Task RequestPages (ByteData array)
		{
			List<string> list = this.manager.GetPages(name);
			string pages = JsonConvert.SerializeObject(list);
			await this.client.Write(new ByteData(Messages.RequestPagesResponse(pages)));
		}
		
		public async Task RequestPage (ByteData array)
		{
			JObject root = Parse(array);
			string projectName = root.Value<string>("page");
			Page page = this.manager.GetPage(projectName);
			if (page != null)
			{
				await this.client.Write(new ByteData(Messages.RequestPageResponse(page)));
			} else
			{
				await this.client.WriteNotOkResponse();
			}
		}

		public async Task Login (ByteData array)
		{
			JObject root = Parse(array);
			this.name = (root.Value<string>("username"));
			if (this.name == null || this.name.Length == 0)
			{
				await this.client.WriteNotOkResponse();
				return;
			}
			if (this.manager.Login(this.name, root.Value<string>("password"))) {
				// log client into server.

				await this.client.WriteOkResponse();
			} else {
				await this.client.WriteNotOkResponse();
			}

		}

		public async Task RequestAccount (ByteData array) {
			await this.client.Write(new ByteData(Messages.ResponseOk()));
		}

		public async Task MakeAccount (ByteData array) {
			Console.WriteLine("making account");
			JObject root = JObject.Parse(array.Message);
			

			if (this.manager.CreateAccount(root.Value<string>("username"),root.Value<string>("password"))) {
				await this.client.WriteOkResponse();
			} else {
				await this.client.WriteNotOkResponse();
			}
		}

		public async Task CreateProject(ByteData array)
		{
			JObject root = JObject.Parse(array.Message);
			if (this.manager.CreateProject(this.name, root.Value<string>("projectName")))
			{
				await this.client.WriteOkResponse();
			} else
			{
				await this.client.WriteNotOkResponse();
			}
		}

		public async Task CreatePage(ByteData array)
		{

		}

		private JObject Parse (ByteData data) {
			return JObject.Parse(data.Message);
		}

	}
}
