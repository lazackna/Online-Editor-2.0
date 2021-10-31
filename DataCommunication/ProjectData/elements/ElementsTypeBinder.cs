using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace DataCommunication_ProjectData
{
	public class ElementsTypeBinder : ISerializationBinder
	{
		public IList<Type> Types { get; set; }

		public ElementsTypeBinder()
		{
			Types = new List<Type>
			{
				typeof(Text),
				typeof(Image),
				typeof(Button),
				typeof(Bitmap),
				typeof(Page)
			};
		}

		public Type BindToType(string assemblyName, string typeName) =>
			Types.SingleOrDefault(t => t.UnderlyingSystemType.ToString() == typeName);

		public void BindToName(Type type, out string assemblyName, out string typeName)
		{
			assemblyName = null;
			typeName = type.UnderlyingSystemType.ToString();
		}
	}
}