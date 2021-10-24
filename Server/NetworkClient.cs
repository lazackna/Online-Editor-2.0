using Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public class NetworkClient
	{

		private TcpClient client;
		private NetworkStream stream;
		public NetworkClient(TcpClient client) {
			this.client = client;
			this.stream = client.GetStream();
		}

		public async Task<byte[]> Read () {
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
				await WriteOkResponse();
				Segment segment = new Segment(received);
				segments = new byte[segment._id + 1][];
				segments[currentSegment] = received;
				byte[] segmentID = { received[^2], received[^3] };
				int id = BitConverter.ToInt16(segmentID);

				if (id == 0) finalSegment = true;
				currentSegment++;
			}

			return segments;
		}

		public async Task Write (ByteData data) {
			foreach (Segment s in data.Segments)
			{
				byte[] array = s.ToByteArray();
				await this.stream.WriteAsync(array, 0, array.Length);
				await this.stream.FlushAsync();
			}
		}

		public async Task WriteOkResponse() {
			byte[] buffer = WrapMessage(Encoding.ASCII.GetBytes("200 OK"));
			await this.stream.WriteAsync(buffer);
			await this.stream.FlushAsync();
		}

		private byte[] WrapMessage(byte[] message)
		{
			// Get the length prefix for the message
			byte[] lengthPrefix = BitConverter.GetBytes(message.Length);
			// Concatenate the length prefix and the message
			byte[] ret = new byte[lengthPrefix.Length + message.Length];
			lengthPrefix.CopyTo(ret, 0);
			message.CopyTo(ret, lengthPrefix.Length);
			return ret;
		}
	}
}
