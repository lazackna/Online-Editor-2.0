using System;
using System.Collections.Generic;
using System.Text;

namespace DataCommunication
{
	public class ByteData
	{
		public IList<Segment> Segments { private set; get; }
		public string Message;
		public int Id => Segments[0].Type;

		public ByteData((byte, string) message)
		{
			var messageType = message.Item1;
			Message = message.Item2 ?? throw new ArgumentNullException(nameof(message.Item2), "the payload was null");

			var maxSize = Segment.MaxMessageSize;
			var length = Message.Length;

			if (length <= maxSize)
			{
				var segment = new Segment(messageType, Message, 0);
				segment.CalculateChecksum();
				Segments = new List<Segment> { segment };
			}
			else
			{
				var segmentCount = decimal.ToInt32(Math.Ceiling((decimal) length / maxSize));
				Segments = new List<Segment>(segmentCount);

				for (var i = 0; i < segmentCount; i++)
				{
					var segmentMessage = Message.Substring(maxSize * i, Math.Min(maxSize, length - maxSize * i));
					var segment = new Segment(messageType, segmentMessage, (byte) (segmentCount - (i + 1)));
					segment.CalculateChecksum();
					Segments.Add(segment);
				}
			}
		}
		public ByteData(params byte[][] data)
		{
			Segments = new List<Segment>();
			if (data.Length == 0) return;
			var builder = new StringBuilder();

			foreach (var bytes in data)
			{
				var segment = new Segment(bytes);
				Segments.Add(segment);
				builder.Append(segment.Message);
			}
			Message = builder.ToString();
		}

		public byte GetMessageType()
		{
			return this.Segments[0].Type;
		}

		public static bool TryParse(out ByteData byteData, params byte[][] data)
		{
			var bd = new ByteData {Segments = new List<Segment>()};
			var builder = new StringBuilder();

			foreach (var bytes in data)
			{
				if (Segment.TryParse(out var segment, bytes))
				{
					bd.Segments.Add(segment);
					builder.Append(segment.Message);
				}
				else
				{
					byteData = null;
					return false;
				}
			}

			if (bd.Segments[^1].Id != 0)
			{
				byteData = null;
				return false;
			}

			bd.Message = builder.ToString();
			byteData = bd;
			return true;
		}
	}

	public class Segment
	{
		private const byte LengthSize = 2;
		private const byte TypeSize = 1;
		private const byte IdSize = 2;
		private const byte ChecksumSize = 1;
		internal static readonly ushort TotalLength = 1024;
		internal static readonly ushort MaxMessageSize = (ushort) (TotalLength - LengthSize - TypeSize - IdSize - ChecksumSize);

		private const int TypeIndex = LengthSize + TypeSize - 1;
		private ushort _length;
		internal byte Type { private set; get; }
		public string Message { private set; get; }
		public ushort Id { private set; get; }
		private byte _checksum;

		public Segment(byte[] bytes)
		{
			if (TryParse(out var segment, bytes))
			{
				_length = segment._length;
				Type = segment.Type;
				Message = segment.Message;
				Id = segment.Id;
				_checksum = segment._checksum;
			}
			else throw new Exception("Checksum was not correct");
		}

		internal Segment(byte type, string message, ushort id)
		{
			if (message.Length > MaxMessageSize) throw new ArgumentOutOfRangeException(nameof(message.Length), "message was too long");
			_length = (ushort) (LengthSize + TypeSize + message.Length + IdSize + ChecksumSize);
			Type = type;
			Message = message;
			Id = id;
			_checksum = 0;
		}

		private Segment()
		{
		}

		public static bool TryParse(out Segment segment, byte[] bytes)
		{
			var s = new Segment();

			var lengthArr = new byte[LengthSize];
			var messageArr = new byte[bytes.Length - LengthSize - TypeSize - IdSize - ChecksumSize];
			var idArr = new byte[IdSize];

			Array.Copy(bytes, 0, lengthArr, 0, 2);
			Array.Copy(bytes, 3, messageArr, 0, messageArr.Length);
			s.Message = Encoding.ASCII.GetString(messageArr);
			Array.Copy(bytes, bytes.Length - 3, idArr, 0, 2);

			Array.Reverse(lengthArr);
			Array.Reverse(idArr);
			s.Id = BitConverter.ToUInt16(idArr, 0);

			s._length = BitConverter.ToUInt16(lengthArr, 0);
			s.Type = bytes[TypeIndex];
			s.Message = Encoding.ASCII.GetString(messageArr);
			s.Id = BitConverter.ToUInt16(idArr, 0);

			s._checksum = 0;
			s.CalculateChecksum();

			var correctChecksum = s._checksum == bytes[^1];

			segment = correctChecksum ? s : null;
			return correctChecksum;
		}

		public string GetMessage()
		{

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
			var idAsBytes = BitConverter.GetBytes(Id);

			Array.Reverse(lengthAsBytes);
			Array.Reverse(idAsBytes);

			var bytes = new byte[LengthSize + TypeSize + IdSize + ChecksumSize + messageAsBytes.Length];
			lengthAsBytes.CopyTo(bytes, 0);
			bytes[TypeIndex] = Type;
			messageAsBytes.CopyTo(bytes, 3);
			idAsBytes.CopyTo(bytes, bytes.Length - 3);
			bytes[^1] = _checksum;

			return bytes;
		}

		public void CalculateChecksum()
		{
			var bytes = ToByteArray();
			//byte checksum = 0;

			//for (var i = 0; i < bytes.Length - 1; i++) checksum ^= bytes[i];
			byte output = bytes[0];

			for (int i = 1; i < bytes.Length - 2; i++)
			{
				output = (byte) (output ^ bytes[i]);
			}

			_checksum = output;
		}
	}
}