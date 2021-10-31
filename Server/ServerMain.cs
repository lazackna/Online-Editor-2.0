using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommunication;

namespace Server
{

	public class ServerMain
	{
		private TcpListener listener;
		private static List<ClientHandler> clients;

		public static async Task Main(string[] args)
		{
			new ServerMain();
			await Task.Delay(-1);
		}

		//34192
		public ServerMain()
		{
			clients = new List<ClientHandler>();
			AccountManager.InitializeServer();
			this.listener = new TcpListener(IPAddress.Loopback, 34192);
			this.listener.Start();
			this.listener.BeginAcceptTcpClient(OnConnect, null);
		}

		//public async void Start(TcpClient tcpClient)
		//{
		//    while (true)
		//    {
		//        byte[] received = await Read(tcpClient);
		//        Console.WriteLine($"Received: {Encoding.ASCII.GetString(received)}");

		//        byte[] buffer = WrapMessage(Encoding.ASCII.GetBytes("200 OK"));
		//        await tcpClient.GetStream().WriteAsync(buffer);
		//        tcpClient.GetStream().Flush();
		//    }
		//}

		private void OnConnect(IAsyncResult ar)
		{
			this.listener.BeginAcceptTcpClient(OnConnect, null);
			var tcpClient = this.listener.EndAcceptTcpClient(ar);
			clients.Add(new ClientHandler(tcpClient));
			Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");


		}


		public class ClientHandler
		{
			private TcpClient tcpClient;
			private Thread thread;
			private MessageHandler messageHandler;
			private NetworkClient client;

			public ClientHandler(TcpClient tcpClient)
			{
				this.tcpClient = tcpClient;
				this.client = new NetworkClient(this.tcpClient);
				this.messageHandler = new MessageHandler(this.client);

				this.thread = new Thread(async () =>
			   {
				   while (this.messageHandler.connected)
				   {
					   try
					   {
						   byte[][] segments = await this.client.ReadSegments();
						   ByteData data = new ByteData(segments);

						   Console.WriteLine($"Received from {this.tcpClient.Client.RemoteEndPoint}: {data.Message}");
						   await this.messageHandler.Handle(data);
					   }
					   catch (Exception e)
					   {
						   Debug.WriteLine(e.StackTrace);
						   this.client.Dispose();
						   clients.Remove(this);
						   break;
					   }

				   }
				   Console.WriteLine("Removing client");
				   this.client.Dispose();
				   clients.Remove(this);
			   });
				this.thread.Start();
			}
		}
	}
}