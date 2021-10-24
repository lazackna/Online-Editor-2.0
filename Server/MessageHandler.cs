using Communication;
using DataCommunication;
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
		public NetworkClient client;
		public MessageHandler (NetworkClient client)
		{
			this.client = client;
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
					MakeAccount(data);
					break;
				case 20:
					RequestPages(data);
					break;
				case 21:
					RequestPage(data);
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
					Ping(data);
					break;
			}
		}

		public void Ping (ByteData array)
		{
			
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

		public void RequestPages (ByteData array)
		{
			
		}

		public void RequestPage (ByteData array)
		{
			
		}

		public async Task Login (ByteData array)
		{
			await this.client.WriteOkResponse();

		}

		public async Task RequestAccount (ByteData array) {
			await this.client.Write(new ByteData(Messages.ResponseOk()));
		}

		public void MakeAccount (ByteData array) {
			Console.WriteLine("making account");
		}

	}
}
