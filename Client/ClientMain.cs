using Communication;
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

        public async Task SendMessage (string message)
        {
            byte[] buffer = WrapMessage(Encoding.ASCII.GetBytes(message));
            await this.tcpClient.GetStream().WriteAsync(buffer, 0, buffer.Length);
            await this.tcpClient.GetStream().FlushAsync();

            byte[] received = await Read();
        }

        public async Task SendTest() {
        //byte[] buffer = { 6, 0, 0, 0, 104, 101, 108, 108, 111, 63 }
            byte[] buffer =  { 0, 26, 1, 0, 101, 113, 117, 101, 115, 116, 32, 109, 97, 107, 101, 32, 97, 99, 99, 111, 117, 110, 116, 0, 0, 0 };
            await this.stream.WriteAsync(buffer, 0, buffer.Length);
            await this.stream.FlushAsync();
            byte[] received = await Read();
		}

        public async Task SendSegments(ByteData data) {
            foreach (Segment s in data.Segments) {
                Debug.WriteLine("");
                
                byte[] array = s.ToByteArray();
                foreach (int i in array) {
                    Debug.Write(i + ", ");
				}
                await this.tcpClient.GetStream().WriteAsync(array, 0, array.Length);
                await this.tcpClient.GetStream().FlushAsync();
                byte[] received = await Read();
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
            byte[] prefix = new byte[4];
            await this.stream.ReadAsync(prefix, 0, 4);
            int size = BitConverter.ToInt32(prefix);
            byte[] received = new byte[size];

            int bytesRead = 0;

            while(bytesRead < size)
            {
                int read = await this.stream.ReadAsync(received, bytesRead, received.Length - bytesRead);
                bytesRead += read;
            }

            return received;
        }
    }
}