using Newtonsoft.Json;
using System;

namespace DataCommunication_ProjectData
{
	public class Element : IPositionProvider
	{
		public int _x;
		public int _y;

		public Element(int x, int y)
		{
			_x = x;
			_y = y;
		}

		public int GetX() => _x;
		public int GetY() => _y;
	}
}
