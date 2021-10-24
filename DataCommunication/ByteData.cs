using System;
using System.Collections.Generic;
using System.Text;

namespace Communication
{
	public class ByteData
	{
		public readonly IList<Segment> Segments;
		public string Message;
		public int Id;
		public ByteData((byte, string) message)
		{
			Segments = new List<Segment>();

			var messageType = message.Item1;
			var messagePayload = message.Item2;
			if (messagePayload == null) throw new ArgumentNullException(nameof(messagePayload), "the payload was null");

			var maxSize = Segment.MaxMessageSize();
			var length = messagePayload.Length;

			if (length <= maxSize) Segments.Add(new Segment(messageType, messagePayload, 0));
			else
			{
				var segmentCount = decimal.ToInt32(Math.Ceiling((decimal)length / maxSize));
				for (var i = 0; i < segmentCount; i++)
				{
					var segmentMessage = messagePayload.Substring(maxSize * i, Math.Min(maxSize, length - maxSize * i));
					Segments.Add(new Segment(messageType, segmentMessage, (byte)(segmentCount - i - 1)));
				}
			}
		}

		public ByteData(byte[][] byteArrays) {
			this.Segments = new List<Segment>();
			StringBuilder builder = new StringBuilder();
			foreach (byte[] b in byteArrays) {
				Segment segment = new Segment(b);
				Segments.Add(segment);
				builder.Append(segment.GetMessage());
			}
			this.Message = builder.ToString();
			this.Id = this.Segments[0]._type;
		}

		public byte GetMessageType() {
			return this.Segments[0]._type;
		}
	}

	public class Segment
	{
		internal static readonly byte LengthSize = 2;
		internal static readonly byte TypeSize = 1;
		internal static readonly byte IdSize = 2;
		internal static readonly byte ChecksumSize = 1;
		internal static readonly ushort TotalLength = 1024;
		internal static ushort MaxMessageSize() => (ushort)(TotalLength - LengthSize - TypeSize - IdSize - ChecksumSize);

		private readonly ushort _length;
		public readonly byte _type;
		public readonly string Message;
		public readonly ushort _id;
		private byte _checksum;

		public Segment(byte[] bytes)
		{
			var lengthArr = new byte[LengthSize];
			Array.Copy(bytes, 0, lengthArr, 0, 2);
			Array.Reverse(lengthArr);
			_length = BitConverter.ToUInt16(lengthArr, 0);
			Console.WriteLine(_length);
			_type = bytes[2];
			var messageArr = new byte[_length - LengthSize - TypeSize - IdSize - ChecksumSize];
			Console.WriteLine(messageArr.Length);
			Array.Copy(bytes, 3, messageArr, 0, messageArr.Length);
			Message = Encoding.ASCII.GetString(messageArr);
			var idArr = new byte[IdSize];
			Array.Copy(bytes, bytes.Length - 3, idArr, 0, 2);
			Array.Reverse(idArr);
			_id = BitConverter.ToUInt16(idArr, 0);
			_checksum = CalculateChecksum();
			Console.WriteLine(_checksum);

			if (_checksum != bytes[^1]) throw new Exception("checksum was not correct");
		}

		internal Segment(byte messageType, string messagePayload, ushort id)
		{
			if (messagePayload.Length > MaxMessageSize()) throw new ArgumentOutOfRangeException(nameof(messagePayload.Length), "message was too long");
			_length = (ushort)(messagePayload.Length + LengthSize + TypeSize + IdSize + ChecksumSize);
			_type = messageType;
			Message = messagePayload;
			_id = id;
			_checksum = CalculateChecksum();
		}

		public string GetMessage () {

			byte[] array = ToByteArray();
			int size = array.Length - 6;
			byte[] messageArray = new byte[size];
			Array.Copy(array, 3, messageArray, 0, messageArray.Length);

			return Encoding.ASCII.GetString(messageArray);
		}

		public byte[] ToByteArray()
		{
			var lengthAsBytes = BitConverter.GetBytes(_length);
			var messageAsBytes = Encoding.ASCII.GetBytes(Message);
			var idAsBytes = BitConverter.GetBytes(_id);

			Array.Reverse(lengthAsBytes);
			Array.Reverse(idAsBytes);

			var bytes = new byte[LengthSize + TypeSize + IdSize + ChecksumSize + messageAsBytes.Length];
			lengthAsBytes.CopyTo(bytes, 0);
			bytes[2] = _type;
			messageAsBytes.CopyTo(bytes, 3);
			idAsBytes.CopyTo(bytes, bytes.Length - 3);
			bytes[^1] = _checksum;
			return bytes;
		}

		public byte CreateChecksum () {
			//byte[] array = new byte[]
			return 1;
		}

		public byte CalculateChecksum()
		{
			var bytes = ToByteArray();
			//byte checksum = 0;

			//for (var i = 0; i < bytes.Length - 1; i++) checksum ^= bytes[i];
			byte output = bytes[0];

			for (int i = 1; i < bytes.Length - 2; i++)
			{
				output = (byte)(output ^ bytes[i]);
			}
			return output;
		}

		//public static void Main(string[] args)
		//{
		//	var s1 = new Segment(0, "a", 256);
		//	s1.CalculateChecksum();
		//	Console.WriteLine(string.Join(", ", s1.ToByteArray()));
		//	Console.WriteLine();

		//	var s2 = new Segment(s1.ToByteArray());
		//	Console.WriteLine(string.Join(", ", s2.ToByteArray()));
		//	Console.WriteLine();
		//}
	}
}