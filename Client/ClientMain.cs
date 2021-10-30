using DataCommunication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
	public class ClientMain
	{
		public string name { get; set; }
		public TcpClient tcpClient;
		private NetworkStream stream;
		private Thread thread;

		private string DNS;
		private int port;
		public bool isConnected;

		public static void Main(string[] args)
		{
			//ClientMain program = new ClientMain();
			//program.Start();
		}

		public ClientMain(string DNS, int port)
		{
			this.DNS = DNS;
			this.port = port;
			this.tcpClient = new TcpClient();
			this.isConnected = false;
		}

		public bool ConnectToServer()
		{
			try
			{
				this.tcpClient.Connect(this.DNS, this.port);
				this.stream = this.tcpClient.GetStream();
				this.isConnected = true;
				return true;
			}
			catch
			{
				//this.tcpClient = null;
				this.isConnected = false;
				this.stream = null;
				return false;
			}
		}

		public async Task<bool> ConnectToServerAsync()
		{
			try
			{
				await this.tcpClient.ConnectAsync(this.DNS, this.port);
				this.stream = this.tcpClient.GetStream();
				this.isConnected = true;
				return true;
			}
			catch
			{
				//this.tcpClient = null;
				this.stream = null;
				this.isConnected = false;
				return false;
			}
		}

		public async void Start()
		{
			//this.tcpClient = new TcpClient("localhost", 8888);
			while (true)
			{
				byte[] buffer = Encoding.ASCII.GetBytes(Console.ReadLine());
				await this.stream.WriteAsync(WrapMessage(buffer));
				this.stream.Flush();
				byte[] received = await Read();
				Console.WriteLine(Encoding.ASCII.GetString(received));
			}
		}

		public async Task SendMessage(string message)
		{
			byte[] buffer = WrapMessage(Encoding.ASCII.GetBytes(message));
			await this.tcpClient.GetStream().WriteAsync(buffer, 0, buffer.Length);
			await this.tcpClient.GetStream().FlushAsync();

			byte[] received = await Read();
		}

		public async Task SendSegments(ByteData data)
		{
			foreach (Segment s in data.Segments)
			{
				byte[] array = s.ToByteArray();
				await this.tcpClient.GetStream().WriteAsync(array, 0, array.Length);
				await this.tcpClient.GetStream().FlushAsync();
				byte[] received = await Read();
				Debug.WriteLine("\nReceived: " + Encoding.ASCII.GetString(received) + "\n");

			}

		}

		public byte[] WrapMessage(byte[] message)
		{
			// Get the length prefix for the message
			byte[] lengthPrefix = BitConverter.GetBytes(message.Length);
			// Concatenate the length prefix and the message
			byte[] ret = new byte[lengthPrefix.Length + message.Length];
			lengthPrefix.CopyTo(ret, 0);
			message.CopyTo(ret, lengthPrefix.Length);
			return ret;
		}

		public async Task<byte[]> Read()
		{

			byte[] prefix = new byte[2];
			await stream.ReadAsync(prefix, 0, 2);
			Array.Reverse(prefix);
			int size = BitConverter.ToInt16(prefix);
			byte[] received = new byte[size];
			Array.Reverse(prefix);
			prefix.CopyTo(received, 0);
			int bytesRead = 2;

			while (bytesRead < size)
			{
				int read = await stream.ReadAsync(received, bytesRead, received.Length - bytesRead);
				bytesRead += read;
			}
			Debug.WriteLine("received message: " + Encoding.ASCII.GetString(received));
			return received;
		}

		public async Task<byte[][]> ReadSegments()
		{
			bool finalSegment = false;
			byte[][] segments = null;
			int currentSegment = 0;
			while (!finalSegment || currentSegment == 0)
			{
				byte[] received = await Read();
				Segment segment = new Segment(received);
				if (currentSegment == 0)
				segments = new byte[segment.Id + 1][];
				segments[currentSegment] = received;
				byte[] segmentID = { received[^2], received[^3] };
				int id = BitConverter.ToInt16(segmentID);

				if (id == 0) finalSegment = true;
				currentSegment++;
			}

			return segments;
		}
	}
}