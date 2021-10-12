using System;
using System.Collections.Generic;
using System.Text;

namespace DataCommunication_ProjectData
{
	public class Page
	{
		public List<Element> Elements { get; set; }

		public Page()
		{
			Elements = new List<Element>();
		}
	}
}