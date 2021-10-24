using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace Online_Editor_2._0.Util
{
	static class SymbolStorage
	{
		internal static Regex Regex = new Regex("[\\|/<>\":*?]");

		internal static IDictionary<string, string> SpecialConverter = new Dictionary<string, string>
		{
			{"\\", "bs"}, {"|", "pc"}, {"/", "fs"}, {"<", "la"}, {">", "ra"}, {"\"", "dq"}, {":", "cl"}, {"*", "as"}, {"?", "qm"}
		};

		private readonly static string UndefinedPath = Path.Combine("Resources", "undefined.png");
		private static Image _undefined;
		internal static Image Undefined => _undefined ??= Image.FromFile(UndefinedPath);

		private static IDictionary<string, Image> _symbols;
		internal static IDictionary<string, Image> Symbols
		{
			get
			{
				if (_symbols == null || _symbols.Count == 0)
				{
					_symbols = new Dictionary<string, Image>();
					GetFiles("Resources");
				}

				return _symbols;
			}
		}

		private static void GetFiles(string path)
		{
			foreach (var d in Directory.GetDirectories(path)) GetFiles(d);

			var files = Directory.GetFiles(path);
			foreach (var file in files)
				if (file != UndefinedPath)
					_symbols.Add(Path.GetFileNameWithoutExtension(file), Image.FromFile(file));
		}

		public static Image GetImage(string symbol)
		{
			if (Regex.IsMatch(symbol)) symbol = SpecialConverter[symbol];
			if (Symbols.ContainsKey(symbol)) return Symbols[symbol];
			return Undefined;
		}
	}
}
