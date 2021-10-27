using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace Storage
{
	class SymbolStorage
	{
		private static SymbolStorage _instance;
		public static SymbolStorage Instance => _instance ??= new SymbolStorage();

		private static readonly string ResourceRoot = Path.Combine("Resources", "symbols");

		private readonly Regex regex;
		private readonly IDictionary<string, string> specialConverter;

		private readonly string undefinedPath;
		public readonly Bitmap Undefined;

		private readonly IDictionary<string, Bitmap> symbols;

		private SymbolStorage()
		{
			regex = new Regex(@"[\\|/<>"":*? \.;]");
			specialConverter = new Dictionary<string, string>
			{
				{"\\", "bs"}, {@"|", "pc"}, {@"/", "fs"}, {@"<", "la"}, {@">", "ra"}, {"\"", "dq"},
				{@":", "cl"}, {@"*", "as"}, {@"?", "qm"}, {@" ", "sp"}, {@".", "dt"}, {@";", "sc"}
			};
			undefinedPath = Path.Combine(ResourceRoot, "undefined.png");
			Undefined = new Bitmap(Image.FromFile(undefinedPath));

			symbols = new Dictionary<string, Bitmap>();

			GetFiles(ResourceRoot);
		}

		~SymbolStorage()
		{
			Undefined.Dispose();
			foreach (var symbolBmp in symbols.Values) symbolBmp.Dispose();
		}

		private void GetFiles(string path)
		{
			foreach (var d in Directory.GetDirectories(path)) GetFiles(d);

			var files = Directory.GetFiles(path);
			foreach (var file in files)
				if (file != undefinedPath)
					symbols.Add(Path.GetFileNameWithoutExtension(file), new Bitmap(Image.FromFile(file)));
		}

		public Bitmap GetImage(char symbol)
		{
			var s = $"{symbol}";
			if (regex.IsMatch(s)) s = specialConverter[s];
			return symbols.ContainsKey(s) ? symbols[s] : Undefined;
		}
	}
}
