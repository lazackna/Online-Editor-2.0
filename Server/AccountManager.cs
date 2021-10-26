using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public class AccountManager
	{
		public static string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Online_Editor");

		private const string PASSPATH = "Password.pass";

		public AccountManager()
		{

		}

		public static void InitializeServer()
		{
			if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
		}

		public bool CreateAccount(string name, string password)
		{
			string path = Path.Combine(dataPath, name);
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
				File.WriteAllText(Path.Combine(path, PASSPATH), password);
				return true;
			}
			return false;
		}

		public bool Login(string name, string password)
		{
			string root = Path.Combine(dataPath, name);
			if (Directory.Exists(root) && File.Exists(Path.Combine(root, PASSPATH)))
			{
				string savedpass = File.ReadAllText(Path.Combine(root, PASSPATH));
				if (password == savedpass) return true;
			}

			return false;
		}



	}
}
