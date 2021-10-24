using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using DataCommunication_ProjectData;
using Online_Editor.Util;
using Online_Editor_2._0.Util;

namespace Online_Editor
{
	/// <summary>
	/// Interaction logic for ProjectView.xaml
	/// </summary>
	public partial class ProjectView : Window, ICanvasFiller
	{
		public ProjectView()
		{
			InitializeComponent();
		}

		public void Add(Element element)
		{
			throw new NotImplementedException();
		}
	}
}
