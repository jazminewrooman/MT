using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Provider;
using Android.Graphics;

using Xamarin.Forms;

using MasTicket.Droid; //enables registration outside of namespace
using Android.Hardware;
using MasTicket;

[assembly: Xamarin.Forms.Dependency(typeof(ContactosImplementation))]

namespace MasTicket.Droid
{
    class ContactosImplementation : IContactos
    {
        public ContactosImplementation() { }
        public static void Init() { }
        List<Contacto> lsc = new List<Contacto>();

		public string GetOS()
		{
			string s = "";
			Version ver = System.Environment.OSVersion.Version;
			s = "Android " + Build.VERSION.Release; //ver.Major.ToString() + ver.Minor.ToString();
			return (s);
		}

		public string GetIP()
		{
			//IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
			string ipAddress = string.Empty;
			//if (addresses != null && addresses[0] != null)
			//{
			//	ipAddress = addresses[0].ToString();
			//}
			//else
			//{
			//	ipAddress = null;
			//}

			//Java.Util.IEnumeration networkInterfaces = Java.Net.NetworkInterface.NetworkInterfaces;
			//while (networkInterfaces.HasMoreElements)
			//{
			//	Java.Net.NetworkInterface netInterface = (Java.Net.NetworkInterface)networkInterfaces.NextElement();
			//	Console.WriteLine(netInterface.ToString());
			//}

			//ipAddress = NetworkInterface.GetAllNetworkInterfaces().Where(x => x.Name.Equals("en0")).First().GetIPProperties().UnicastAddresses.Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork).First().Address.ToString();

			//using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
			//{
			//	socket.Connect("10.0.2.4", 65530);
			//	IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
			//	ipAddress = endPoint.Address.ToString();
			//}

			//string url = "http://checkip.dyndns.org";
			//System.Net.WebRequest req = System.Net.WebRequest.Create(url);
			//System.Net.WebResponse resp = req.GetResponse();
			//System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
			//string response = sr.ReadToEnd().Trim();
			//string[] a = response.Split(':');
			//string a2 = a[1].Substring(1);
			//string[] a3 = a2.Split('<');
			//ipAddress = a3[0];

			ipAddress = GetExternalIP();

			return (ipAddress);
		}

		protected string GetExternalIP()
		{
			try
			{
				using (WebClient client = new WebClient())
				{
					client.Headers["User-Agent"] =
					"Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) " +
					"(compatible; MSIE 6.0; Windows NT 5.1; " +
					".NET CLR 1.1.4322; .NET CLR 2.0.50727)";
					try
					{
						byte[] arr = client.DownloadData("http://checkip.amazonaws.com/");
						string response = System.Text.Encoding.UTF8.GetString(arr);
						return response.Trim();
					}
					catch (WebException ex)
					{
						try
						{
							byte[] arr = client.DownloadData("http://icanhazip.com/");
							string response = System.Text.Encoding.UTF8.GetString(arr);
							return response.Trim();
						}
						catch (WebException exc)
						{ return ""; }
					}
				}
			}
			catch (Exception ex)
			{
				return "";
			}
		}

		public string GetLocalIPv4(NetworkInterfaceType _type)
		{
			string output = "";
			foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
				{
					foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
					{
						if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
						{
							output = ip.Address.ToString();
						}
					}
				}
			}
			return output;
		}

		public List<string> GetPhoneNumber(string name, Context context)
        {
			List<string> ls = new List<string>();
            string ret = null;
            try
            {
                //string selection = ContactsContract.CommonDataKinds.Phone.SearchDisplayNameKey + " like'%" + name + "%'";
                string selection = ContactsContract.ContactsColumns.DisplayName + " like\"%" + name + "%\"";
                string[] projection = new string[] { ContactsContract.CommonDataKinds.Phone.Number };
                var c = context.ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, projection, selection, null, null);
				if (c.MoveToFirst())
                {
					do
					{
						ret = c.GetString(0);
						ls.Add(ret);
					} while (c.MoveToNext());
                }
                c.Close();
                if (ret == null)
                    ret = "Unsaved";
            } catch(Exception e)
            {

            }
            return ls;
        }

        private byte[] GetBitmap(string uri)
        {
            byte[] ret = null;
            Bitmap mBitmap = null;
            var url = global::Android.Net.Uri.Parse(uri);
            mBitmap = MediaStore.Images.Media.GetBitmap(Forms.Context.ContentResolver, url);
            if (mBitmap != null)
            {
                var ms = new MemoryStream();

                mBitmap.Compress(Bitmap.CompressFormat.Png, 0, ms);
                ret = ms.ToArray();
            }
            return (ret);
        }

        public List<Contacto> GetLista()
        {
            Contacto c;
            try
            {
                if (lsc.Count() == 0)
                {
                    var uri = ContactsContract.Contacts.ContentUri;
					List<string> tmptel = new List<string>();
					string tmpname = "";
                    string[] projection = { ContactsContract.Contacts.InterfaceConsts.Id, ContactsContract.Contacts.InterfaceConsts.DisplayName, ContactsContract.Contacts.InterfaceConsts.HasPhoneNumber, ContactsContract.Contacts.InterfaceConsts.PhotoThumbnailUri };
                    var cursor = ((Activity)Forms.Context).ManagedQuery(uri, projection, null, null, null);
                    if (cursor.MoveToFirst())
                    {
                        do
                        {
                            //tmptel = "";
                            if (cursor.GetString(cursor.GetColumnIndex(projection[2])) == "1")
                            {
								tmpname = cursor.GetString(cursor.GetColumnIndex(projection[1]));
                                tmptel = GetPhoneNumber(cursor.GetString(cursor.GetColumnIndex(projection[1])), (Activity)Forms.Context);
								if (tmptel.Count > 0)
								{
									foreach (string stel in tmptel)
									{
										c = new Contacto();
										c.Id = cursor.GetString(cursor.GetColumnIndex(projection[0]));
										c.Name = tmpname;
										c.Number = stel;
										c.PhotoUri = cursor.GetString(cursor.GetColumnIndex(projection[3]));
										if (c.PhotoUri != null)
										{
											//var contactUri = ContentUris.WithAppendedId(ContactsContract.Contacts.ContentUri, long.Parse(c.Id));
											//var contactPhotoUri = Android.Net.Uri.WithAppendedPath(contactUri, Contacts.Photos.ContentDirectory);
											c.Photo = GetBitmap(c.PhotoUri);
										}
										lsc.Add(c);
									}
								}
                            }
                        } while (cursor.MoveToNext());
                    }
                }
                //foreach (Contacto co in lsc)
                //    GetBitmap(co);
            }
            catch (Exception e)
            {

            }
            return (lsc);
        }
    }
}