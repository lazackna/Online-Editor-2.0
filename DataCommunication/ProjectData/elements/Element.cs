namespace DataCommunication_ProjectData
{
	public abstract class Element : IPositionProvider
	{
		private readonly int _x;
		private readonly int _y;

		protected Element(int x, int y)
		{
			_x = x;
			_y = y;
		}

		public int GetX() => _x;
		public int GetY() => _y;
	}
}
