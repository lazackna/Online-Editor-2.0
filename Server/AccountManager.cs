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

		public AccountManager() {

		}

		public static void InitializeServer() {
			if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
		}

		public bool CreateAccount (string name, string password) {
		string path = Path.Combine(dataPath, name);
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
				File.WriteAllText(Path.Combine(path, "Password.pass"), password);
				return true;
			}
			return false;
		}

	}
}
