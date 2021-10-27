using Newtonsoft.Json;
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
			GetPages();
			if (Directory.Exists(root) && File.Exists(Path.Combine(root, PASSPATH)))
			{
				string savedpass = File.ReadAllText(Path.Combine(root, PASSPATH));
				if (password == savedpass) return true;
			}
			
			return false;
		}

		public List<string> GetPages()
		{
			string[] users = Directory.GetDirectories(dataPath);
			List<string> list = new List<string>();
			foreach (string s in users)
			{
				//list.AddRange()
				string[] projects = GetProjects(s);
				list.AddRange(projects);
			}

			return list;
		}

		private string[] GetProjects (string directory)
		{
			string[] projects = Directory.GetDirectories(directory);
			if (projects.Length == 0) return null;
			string[] projectNames = new string[projects.Length];
			for(int i = 0; i < projects.Length; i++)
			{
				string projectName = projects[i].Substring(projects[i].LastIndexOf("\\") + 1);
				string userName = directory.Substring(directory.LastIndexOf("\\") + 1);
				string fullName = userName + "|" + projectName;
				projectNames[i] = fullName;
			}
			return projectNames;
		}



	}
}
