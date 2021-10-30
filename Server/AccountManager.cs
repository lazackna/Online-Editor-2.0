using DataCommunication_ProjectData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Server
{
	public class AccountManager
	{
		private static readonly string FileExtention = "jm";

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

		public List<string> GetPages(string username)
		{
			string[] users = Directory.GetDirectories(dataPath);
			List<string> list = new List<string>();
			foreach (string s in users)
			{
				list.AddRange(GetProjects(s, username));
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

					var page = new Page();
					page.Elements.Add(new Text(0, 0, "Click me!"));
					page.Elements.Add(new Button(0, 15, "No, Click Me!!"));
					var json = JsonConvert.SerializeObject(page, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

					File.WriteAllText(Path.Combine(projectPath, $"main.{FileExtention}"), json);

					string permissionsPath = Path.Combine(projectPath, "Permissions");
					Directory.CreateDirectory(permissionsPath);
					File.Create(Path.Combine(permissionsPath, $"{userName}.perm"));

					return true;
				} else
				{
					// Project already exists.
					return false;
				}

			} else
			{
				// Project under that name already exists or no such user exists.
				return false;
			}
		}

		internal void UploadPage(Page page, string pageID)
		{
			File.WriteAllText(pageID, JsonConvert.SerializeObject(page, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }));
		}

		private const string mainFiller = @"{""Elements"":[{""_value"":""Click me!"",""_x"":0,""_y"":0}]}";
		private void CreateProjectFiles(string path, string username)
		{
			string permissionPath = Path.Combine(path, "Permissions");
			Directory.CreateDirectory(permissionPath);
			File.Create(Path.Combine(permissionPath, username + ".perm"));
			var page = new Page();
			page.Elements.Add(new Text(0, 0, "Click me!"));
			page.Elements.Add(new Button(0, 15, "No, Click Me!!"));
			var json = JsonConvert.SerializeObject(page, new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All});
			File.WriteAllText(Path.Combine(path, $"main.{FileExtention}"), json);
		}

		private List<string> GetProjects (string directory, string username)
		{
			List<string> projectNames = new List<string>();
			string[] projects = Directory.GetDirectories(directory);
			foreach (string s in projects)
			{
				string[] permissions = Directory.GetFiles(Path.Combine(s, "Permissions"));
				bool contains = false;
				foreach(string p in permissions)
				{

					string permName = p.Substring(p.LastIndexOf("\\") + 1, (p.LastIndexOf(".perm") + 1) - (p.LastIndexOf("\\") + 2));

					if (username == permName)
					{
						contains = true;
						break;
					}
				}
				if (contains)
				{
					var path = s.Substring(directory.LastIndexOf("\\") + 1);
					projectNames.Add(path.Replace("\\", "|"));
				}
			}
			return projectNames;

			//if (projects.Length == 0) return null;
			//string[] projectNames = new string[projects.Length];
			//for(int i = 0; i < projects.Length; i++)
			//{
			//	string projectName = projects[i].Substring(projects[i].LastIndexOf("\\") + 1);
			//	string userName = directory.Substring(directory.LastIndexOf("\\") + 1);
			//	string fullName = userName + "|" + projectName;
			//	projectNames[i] = fullName;
			//}
			//return projectNames;
		}
		public string getMainPath(string projectName)
		{
			string[] array = projectName.Split("|");
			string clientPath = Path.Combine(dataPath, array[0]);
			string projectPath = Path.Combine(clientPath, array[1]);
			string mainPath = Path.Combine(projectPath, $"main.{FileExtention}");
			return mainPath;
		}

		public Page GetPage(string projectName)
		{
			string[] array = projectName.Split("|");
			string clientPath = Path.Combine(dataPath, array[0]);
			string projectPath = Path.Combine(clientPath, array[1]);
			string mainPath = Path.Combine(projectPath, $"main.{FileExtention}");
			if (Directory.Exists(clientPath))
			{
				if (Directory.Exists(projectPath))
				{
					if (File.Exists(mainPath))
					{
						string fileText = File.ReadAllText(mainPath);
						return JsonConvert.DeserializeObject<Page>(fileText, new JsonSerializerSettings{TypeNameHandling = TypeNameHandling.All});
					}
				}
			}
			return null;
		}
	}
}
