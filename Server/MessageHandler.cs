using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public class MessageHandler
	{

		public MessageHandler ()
		{

		}

		public void Handle (byte[] array)
		{

			int type = array[2];

			switch (type)
			{
				case 0:
					Login(array);
					break;
				case 1:
					RequestAccount(array);
					break;
				case 2:
					MakeAccount(array);
					break;
				case 20:
					RequestPages(array);
					break;
				case 21:
					RequestPage(array);
					break;
				case 22:
					UploadPage(array);
					break;
				case 23:
					RequestChangePage(array);
					break;
				case 24:
					UploadChangedPage(array);
					break;
				case 193:
					Ping(array);
					break;
			}
		}

		public void Ping (byte[] array)
		{
			
		}

		public void UploadChangedPage (byte[] array)
		{
			
		}

		public void RequestChangePage (byte[] array)
		{
			
		}

		public void UploadPage (byte[] array)
		{
			
		}

		public void RequestPages (byte[] array)
		{
			
		}

		public void RequestPage (byte[] array)
		{
			
		}

		public void Login (byte[] array)
		{


		}

		public void RequestAccount (byte[] array) {

		}

		public void MakeAccount (byte[] array) {

		}

	}
}
