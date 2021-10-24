using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

            public ClientHandler(TcpClient tcpClient)
            {
                this.tcpClient = tcpClient;

                this.thread = new Thread(async () =>
               {
                   while (true)
                   {
                       try
                       {
                           byte[] received = await Read(this.tcpClient);
                           Console.WriteLine($"Received from {this.tcpClient.Client.RemoteEndPoint}: {Encoding.ASCII.GetString(received)}");
                           byte[] buffer = WrapMessage(Encoding.ASCII.GetBytes("200 OK"));
                           await this.tcpClient.GetStream().WriteAsync(buffer);
                           await this.tcpClient.GetStream().FlushAsync();
                       } catch (Exception e)
                       {
                           Debug.WriteLine(e.Message);
                           CloseAndDispose();
                           break;
                       }
                   }
                   Console.WriteLine("Removing client");
                   clients.Remove(this);
               });
                this.thread.Start();
            }

            private void CloseAndDispose ()
            {
                this.tcpClient.GetStream().Close();
                this.tcpClient.GetStream().Dispose();
                this.tcpClient.Close();
                this.tcpClient.Dispose();
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

            public async Task<byte[]> Read(TcpClient tcpClient)
            {
                byte[] prefix = new byte[2];
                await tcpClient.GetStream().ReadAsync(prefix, 0, 2);
                Array.Reverse(prefix);
                int size = BitConverter.ToInt16(prefix);
                byte[] received = new byte[size];

                int bytesRead = 2;
                
                while (bytesRead < size)
                {
                    int read = await tcpClient.GetStream().ReadAsync(received, bytesRead, received.Length - bytesRead);
                    bytesRead += read;
                }

                return received;
            }


        }

    }

}