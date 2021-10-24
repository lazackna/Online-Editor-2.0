using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace Online_Editor_2._0.Util
{
	class SymbolStorage
	{
		internal Regex Regex { get; }

		private string UndefinedPath { get; }

		internal IDictionary<string, string> SpecialConverter { get; }

		internal Bitmap Undefined { get; }
		internal IDictionary<string, Bitmap> Symbols { get; }

		public SymbolStorage()
		{
			Regex = new Regex("[\\\\|/<>\":*?]");
			SpecialConverter = new Dictionary<string, string>
			{
				{"\\", "bs"}, {"|", "pc"}, {"/", "fs"}, {"<", "la"}, {">", "ra"}, {"\"", "dq"}, {":", "cl"}, {"*", "as"}, {"?", "qm"}
			};
			UndefinedPath = Path.Combine("Resources", "undefined.png");
			Undefined = new Bitmap(Image.FromFile(UndefinedPath));

			Symbols = new Dictionary<string, Bitmap>();

			GetFiles("Resources");
		}

		~SymbolStorage()
		{
			Undefined.Dispose();
			foreach (var symbols in Symbols.Values) symbols.Dispose();
		}

		private void GetFiles(string path)
		{
			foreach (var d in Directory.GetDirectories(path)) GetFiles(d);

			var files = Directory.GetFiles(path);
			foreach (var file in files)
				if (file != UndefinedPath)
					Symbols.Add(Path.GetFileNameWithoutExtension(file), new Bitmap(Image.FromFile(file)));
		}

		public Bitmap GetImage(char symbol)
		{
			var s = $"{symbol}";
			if (Regex.IsMatch(s)) s = SpecialConverter[s];
			return Symbols.ContainsKey(s) ? Symbols[s] : Undefined;
		}
	}
}
