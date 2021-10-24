using System;

namespace DataCommunication_ProjectData
{
	public abstract class Element
	{
		public int X { get; set; }
		public int Y { get; set; }

		protected Element(int x, int y)
		{
			X = x;
			Y = y;
		}

		public virtual bool HasText() => false;
		public virtual bool HasImage() => false;

		public virtual string GetText() => throw new NotImplementedException();
	}
}
