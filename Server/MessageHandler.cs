using DataCommunication;
using DataCommunication_ProjectData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		private CommandHandler[] commands;

		public bool connected = true;

		private string activePageString = "";


		private delegate Task CommandHandler(ByteData data);

		public MessageHandler(NetworkClient client)
		{
			this.client = client;
			this.manager = new AccountManager();
			this.name = "";

			commands = new CommandHandler[256];
			FillCommands();
		}

		private void FillCommands()
		{
			commands[Messages.Codes.Login] = Login;
			commands[Messages.Codes.RequestAccount] = RequestAccount;
			commands[Messages.Codes.MakeAccount] = MakeAccount;

			commands[Messages.Codes.CreateProject] = CreateProject;
			commands[Messages.Codes.CreatePage] = CreatePage;

			commands[Messages.Codes.RequestPages] = RequestPages;
			commands[Messages.Codes.RequestPage] = RequestPage;
			commands[Messages.Codes.UploadPage] = UploadPage;
			commands[Messages.Codes.RequestChangePage] = RequestChangePage;
			commands[Messages.Codes.UploadChangedPage] = UploadChangedPage;

			commands[Messages.Codes.ClientPing] = Ping;

			commands[Messages.Codes.Disconnect] = Disconnect;
		}

		public async Task Handle(ByteData data)
		{
			var command = commands[data.GetMessageType()];
			if (command != null) await command(data);
		}

		public async Task Ping(ByteData array)
		{
			await this.client.Write(new ByteData(Messages.ServerPing()));
		}

		public async Task UploadChangedPage(ByteData array)
		{

		}

		public async Task RequestChangePage(ByteData array)
		{

		}

		public async Task UploadPage(ByteData array)
		{
			try
			{
				Page page = JsonConvert.DeserializeObject<Page>(array.Message, new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Objects,
					SerializationBinder = new ElementsTypeBinder()
				});
				manager.UploadPage(page, manager.getMainPath(activePageString));
			}
			catch
			{
				Console.WriteLine("could not deserialize page");
			}

		}

		public async Task RequestPages(ByteData array)
		{
			List<string> list = this.manager.GetPages(name);
			string pages = JsonConvert.SerializeObject(list, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Objects,
				SerializationBinder = new ElementsTypeBinder()
			});
			await this.client.Write(new ByteData(Messages.RequestPagesResponse(pages)));
		}

		public async Task RequestPage(ByteData array)
		{
			JObject root = Parse(array);
			string projectName = root.Value<string>("page");
			activePageString = projectName;
			Page page = this.manager.GetPage(projectName);
			if (page != null)
			{
				await this.client.Write(new ByteData(Messages.RequestPageResponse(page)));
			}
			else
			{
				await this.client.WriteNotOkResponse();
			}
		}

		public async Task Login(ByteData array)
		{
			JObject root = Parse(array);
			this.name = root.Value<string>("username");
			if (string.IsNullOrEmpty(this.name))
			{
				await this.client.WriteNotOkResponse();
				return;
			}
			if (this.manager.Login(this.name, root.Value<string>("password")))
			{
				// log client into server.

				await this.client.WriteOkResponse();
			}
			else
			{
				await this.client.WriteNotOkResponse();
			}

		}

		public async Task RequestAccount(ByteData array)
		{
			await this.client.Write(new ByteData(Messages.ResponseOk()));
		}

		public async Task MakeAccount(ByteData array)
		{
			Console.WriteLine("making account");
			JObject root = JObject.Parse(array.Message);


			if (this.manager.CreateAccount(root.Value<string>("username"), root.Value<string>("password")))
			{
				await this.client.WriteOkResponse();
			}
			else
			{
				await this.client.WriteNotOkResponse();
			}
		}

		public async Task CreateProject(ByteData array)
		{
			JObject root = JObject.Parse(array.Message);
			if (this.manager.CreateProject(this.name, root.Value<string>("projectName")))
			{
				await this.client.WriteOkResponse();
			}
			else
			{
				await this.client.WriteNotOkResponse();
			}
		}

		public async Task CreatePage(ByteData array)
		{

		}

		public async Task Disconnect(ByteData array)
		{
			this.connected = false;
			await this.client.WriteOkResponse();
		}

		private JObject Parse(ByteData data)
		{
			return JObject.Parse(data.Message);
		}

	}
}
