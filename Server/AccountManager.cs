using DataCommunication_ProjectData;
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
			if (Directory.Exists(root) && File.Exists(Path.Combine(root, PASSPATH)))
			{
				string savedpass = File.ReadAllText(Path.Combine(root, PASSPATH));
				CreateProject("tester", "test2");
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

		/*
		 * Use to create project. This is not complete. Change method and private "sub" method according to what is needed when a project is created.
		 */
		public bool CreateProject(string userName, string projectName)
		{
			string userPath = Path.Combine(dataPath, userName);
			if (Directory.Exists(userPath))
			{
				string projectPath = Path.Combine(userPath, projectName);
				if (!Directory.Exists(projectPath))
				{
					Directory.CreateDirectory(projectPath);

					
				} else
				{
					// Project already exists.
					return false;
				}
				return true;
			} else
			{
				// Project under that name already exists or no such user exists.
				return false;
			}
		}
		private const string mainFiller = @"{""Elements"":[{""Width"":10,""Height"":0,""Value"":""test"",""X"":0,""Y"":0}]}";
		private void CreateProjectFiles(string path, string username)
		{
			string permissionPath = Path.Combine(path, "Permissions");
			Directory.CreateDirectory(permissionPath);
			File.Create(Path.Combine(permissionPath, username + ".perm"));
			File.WriteAllText(Path.Combine(path, "main.mj"), mainFiller);
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

		public Page GetPage(string projectName)
		{
			string[] array = projectName.Split("|");
			string clientPath = Path.Combine(dataPath, array[0]);
			string projectPath = Path.Combine(clientPath, array[1]);
			string mainPath = Path.Combine(projectPath, "main.mj");
			if (Directory.Exists(clientPath))
			{
				if (Directory.Exists(projectPath))
				{
					if (File.Exists(mainPath))
					{
						string fileText = File.ReadAllText(mainPath);

						return JsonConvert.DeserializeObject<Page>(fileText);
					}
				}
			}
			return null;
		}



	}
}
