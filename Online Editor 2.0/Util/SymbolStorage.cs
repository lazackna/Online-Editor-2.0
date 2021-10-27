using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace Online_Editor_2._0.Util
{
	class SymbolStorage
	{
		public static readonly SymbolStorage Instance = new SymbolStorage();

		internal readonly Regex Regex;
		private readonly IDictionary<string, string> specialConverter;

		private readonly string undefinedPath;
		private readonly Bitmap undefined;

		private readonly IDictionary<string, Bitmap> symbols;

		private SymbolStorage()
		{
			Regex = new Regex(@"[\\|/<>"":*? \.;]");
			specialConverter = new Dictionary<string, string>
			{
				{"\\", "bs"}, {@"|", "pc"}, {@"/", "fs"}, {@"<", "la"}, {@">", "ra"}, {"\"", "dq"},
				{@":", "cl"}, {@"*", "as"}, {@"?", "qm"}, {@" ", "sp"}, {@".", "dt"}, {@";", "sc"}
			};
			undefinedPath = Path.Combine("Resources", "undefined.png");
			undefined = new Bitmap(Image.FromFile(undefinedPath));

			symbols = new Dictionary<string, Bitmap>();

			GetFiles("Resources");
		}

		~SymbolStorage()
		{
			undefined.Dispose();
			foreach (var symbols in symbols.Values) symbols.Dispose();
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
			if (Regex.IsMatch(s)) s = specialConverter[s];
			return symbols.ContainsKey(s) ? symbols[s] : undefined;
		}
	}
}
