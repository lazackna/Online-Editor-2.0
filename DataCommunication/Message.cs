using System;
using DataCommunication_ProjectData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace DataCommunication
{
	public static class Messages
	{
		public static (byte, string) Login(string username, string password) => (Codes.Login, JsonMessages.Login(username, password).ToString());
		public static (byte, string) RequestAccount() => (Codes.RequestAccount, StringMessages.RequestAccount);
		public static (byte, string) MakeAccount(string username, string password) => (Codes.MakeAccount, JsonMessages.MakeAccount(username, password).ToString());

		public static (byte, string) CreateProject(string name) => (Codes.CreateProject, JsonMessages.createProject(name).ToString());
		public static (byte, string) CreatePage(string name) => (Codes.CreateProject, JsonMessages.createPage(name).ToString());

		public static (byte, string) RequestPages() => (Codes.RequestPages, StringMessages.RequestPages);
		public static (byte, string) RequestPage(string pageID) => (Codes.RequestPage, JsonMessages.requestPage(pageID).ToString());
		public static (byte, string) UploadPage(Page page) => (Codes.UploadPage, JsonConvert.SerializeObject(page, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }));
		public static (byte, string) RequestChangePage() => (Codes.RequestChangePage, StringMessages.RequestChangePage);
		public static (byte, string) UploadChangedPage(string pageID, Page page) => (Codes.UploadChangedPage, JsonMessages.uploadChangedPage(pageID, page).ToString());

		public static (byte, string) ResponseOk() => (Codes.ResponseOK, StringMessages.ResponseOK);
		public static (byte, string) ResponseNotOk() => (Codes.ResponseNotOK, StringMessages.ResponseNotOK);
		public static (byte, string) RequestPagesResponse(string pages) => (Codes.RequestPagesResponse, pages);
		public static (byte, string) RequestPageResponse(Page page) => (Codes.RequestPageResponse, JsonMessages.requestPageResponse(page).ToString());

		public static (byte, string) ClientPing() => (Codes.ClientPing, JsonMessages.ping().ToString());
		public static (byte, string) ServerPing() => (Codes.ServerPing, JsonMessages.ping().ToString());

		private static class JsonMessages
		{
			internal static JObject Login(string username, string password) => JObject.FromObject(new { username, password });
			internal static JObject MakeAccount(string username, string password) => JObject.FromObject(new { username, password });

			internal static JObject requestPage(string pageID) => JObject.FromObject(new { page = pageID });
			internal static JObject uploadPage(Page page) => JObject.FromObject(new { elements = page.Elements });

			internal static JObject createProject(string name) => JObject.FromObject(new { projectName = name });
			internal static JObject createPage(string name) => JObject.FromObject(new { pageName = name });

			internal static JObject requestPageResponse(Page page) => JObject.FromObject(new { elements = page.Elements });
			internal static JObject uploadChangedPage(string pageID, Page page) => JObject.FromObject(new { page = pageID, elements = page.Elements });

			internal static JObject ping() => JObject.FromObject(new { sendtime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() });
		}

		private static class StringMessages
		{
			internal static readonly string RequestAccount = "request make account";

			internal static readonly string RequestPages = "request pages";
			internal static readonly string RequestChangePage = "request change page";

			internal static readonly string ResponseOK = "affirmative";
			internal static readonly string ResponseNotOK = "negative";
		}

		public static class Codes
		{
			public static readonly byte Login = 0;
			public static readonly byte RequestAccount = 1;
			public static readonly byte MakeAccount = 2;

			public static readonly byte CreateProject = 11;
			public static readonly byte CreatePage = 12;

			public static readonly byte RequestPages = 20;
			public static readonly byte RequestPage = 21;
			public static readonly byte UploadPage = 22;
			public static readonly byte RequestChangePage = 23;
			public static readonly byte UploadChangedPage = 24;

			public static readonly byte ResponseOK = 40;
			public static readonly byte ResponseNotOK = 41;
			public static readonly byte RequestPagesResponse = 42;
			public static readonly byte RequestPageResponse = 43;

			public static readonly byte ClientPing = 193;
			public static readonly byte ServerPing = 194;
		}
	}
}