using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

using Xamarin.Forms;

namespace MasTicket
{
	public partial class webchat : ContentPage
	{
		private const string key = "3af0f0a79fc594c0b898e5b5992126a2a05d120f";
		private string message = "";
		private static readonly Encoding encoding = Encoding.UTF8;
		string hash = "";

		public webchat()
		{
			InitializeComponent();
			Title = "Chat de ayuda";

			//message = App.usr.email;
			//var keyByte = encoding.GetBytes(key);
			//using (var hmacsha256 = new HMACSHA256(keyByte))
			//{
			//	hmacsha256.ComputeHash(encoding.GetBytes(message));
			//	hash = ByteToString(hmacsha256.Hash);
			//}

			/*var sc = new UrlWebViewSource()
			{
				Url = System.IO.Path.Combine(DependencyService.Get<IBaseUrl>().Get(), "chat.htm"),
			};*/
			//wv.Source = sc;
			string url = @"https://asicompras.com/chatmobile.htm?name=" + System.Web.HttpUtility.UrlPathEncode(App.usr.name) + "&email=" + App.usr.email;
			wv.Source = url;
			//wv.Source = "http://192.168.15.152/~jazz/atropos/chatmobile.htm?hash=" + hash + "&email=" + App.usr.email;

			//Device.OpenUri(new System.Uri(url));

		}

		static string ByteToString(byte[] buff)
		{
			string sbinary = "";
			for (int i = 0; i < buff.Length; i++)
				sbinary += buff[i].ToString("X2"); /* hex format */
			return sbinary;
		}
	}
}

