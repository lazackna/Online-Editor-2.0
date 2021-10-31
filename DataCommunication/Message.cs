using System;
using DataCommunication_ProjectData;
using Newtonsoft.Json;


namespace DataCommunication
{
	public static class Messages
	{
		public static (byte, string) Login(string username, string password) => (Codes.Login, JsonMessages.Login(username, password));
		public static (byte, string) RequestAccount() => (Codes.RequestAccount, StringMessages.RequestAccount);
		public static (byte, string) MakeAccount(string username, string password) => (Codes.MakeAccount, JsonMessages.MakeAccount(username, password));

		public static (byte, string) CreateProject(string name) => (Codes.CreateProject, JsonMessages.createProject(name));
		public static (byte, string) CreatePage(string name) => (Codes.CreateProject, JsonMessages.createPage(name));

		public static (byte, string) RequestPages() => (Codes.RequestPages, StringMessages.RequestPages);
		public static (byte, string) RequestPage(string pageID) => (Codes.RequestPage, JsonMessages.requestPage(pageID));
		public static (byte, string) UploadPage(Page page) => (Codes.UploadPage, JsonMessages.uploadPage(page));
		public static (byte, string) RequestChangePage() => (Codes.RequestChangePage, StringMessages.RequestChangePage);
		public static (byte, string) UploadChangedPage(string pageID, Page page) => (Codes.UploadChangedPage, JsonMessages.uploadChangedPage(pageID, page));

		public static (byte, string) ResponseOk() => (Codes.ResponseOK, StringMessages.ResponseOK);
		public static (byte, string) ResponseNotOk() => (Codes.ResponseNotOK, StringMessages.ResponseNotOK);
		public static (byte, string) RequestPagesResponse(string pages) => (Codes.RequestPagesResponse, pages);
		public static (byte, string) RequestPageResponse(Page page) => (Codes.RequestPageResponse, JsonMessages.requestPageResponse(page));

		public static (byte, string) ClientPing() => (Codes.ClientPing, JsonMessages.ping());
		public static (byte, string) ServerPing() => (Codes.ServerPing, JsonMessages.ping());

		public static (byte, string) Disconnect() => (Codes.Disconnect, StringMessages.Disconnect);

		private static class JsonMessages
		{
			internal static string Login(string username, string password) => JsonConvert.SerializeObject(new { username, password });
			internal static string MakeAccount(string username, string password) => JsonConvert.SerializeObject(new { username, password });

			internal static string requestPage(string pageID) => JsonConvert.SerializeObject(new { page = pageID });
			internal static string uploadPage(Page page) => JsonConvert.SerializeObject(page,
				new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Objects,
					SerializationBinder = new ElementsTypeBinder()
				});

			internal static string createProject(string name) => JsonConvert.SerializeObject(new { projectName = name });
			internal static string createPage(string name) => JsonConvert.SerializeObject(new { pageName = name });

			internal static string requestPageResponse(Page page) => JsonConvert.SerializeObject(page,
				new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Objects,
					SerializationBinder = new ElementsTypeBinder()
				});
			internal static string uploadChangedPage(string pageID, Page page) => JsonConvert.SerializeObject(new { page = pageID, elements = page.Elements },
				new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Objects,
					SerializationBinder = new ElementsTypeBinder()
				});

			internal static string ping() => JsonConvert.SerializeObject(new { sendtime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() });
		}

		private static class StringMessages
		{
			internal static readonly string RequestAccount = "request make account";

			internal static readonly string RequestPages = "request pages";
			internal static readonly string RequestChangePage = "request change page";

			internal static readonly string ResponseOK = "affirmative";
			internal static readonly string ResponseNotOK = "negative";

			internal static readonly string Disconnect = "disconnect";
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

			public static readonly byte Disconnect = 255;
		}
	}
}