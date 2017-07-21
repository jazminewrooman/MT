using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

using Xamarin.Forms;
using Contacts;
using Foundation;
using UIKit;
using AddressBook;
using MasTicket.iOS; //enables registration outside of namespace
using MasTicket;

[assembly: Xamarin.Forms.Dependency(typeof(ContactosImplementation))]

namespace MasTicket.iOS
{
	class ContactosImplementation : IContactos
	{
		public ContactosImplementation() { }
		public static void Init() { }
		List<Contacto> lsc = new List<Contacto>();

		//public String GetPhoneNumber(string name, Context context)
		//{
		//    string ret = null;
		//    try
		//    {
		//        //string selection = ContactsContract.CommonDataKinds.Phone.SearchDisplayNameKey + " like'%" + name + "%'";
		//        string selection = ContactsContract.ContactsColumns.DisplayName + " like'%" + name + "%'";
		//        string[] projection = new string[] { ContactsContract.CommonDataKinds.Phone.Number };
		//        var c = context.ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, projection, selection, null, null);
		//        if (c.MoveToFirst())
		//        {
		//            ret = c.GetString(0);
		//        }
		//        c.Close();
		//        if (ret == null)
		//            ret = "Unsaved";
		//    } catch(Exception e)
		//    {

		//    }
		//    return ret;
		//}

		public string GetIP()
		{
			string ipAddress = string.Empty;
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

		private byte[] GetBitmap(NSData data)
		{
			byte[] ret = new byte[data.Length];
			System.Runtime.InteropServices.Marshal.Copy(data.Bytes, ret, 0, Convert.ToInt32(data.Length));
			return (ret);
		}

		public delegate void CNContactStoreEnumerateContactsHandler(CNContact contact, bool stop);

		public string GetOS()
		{
			string s = "";
			Version ver = new Version(UIDevice.CurrentDevice.SystemVersion);
			s = "iOS " + ver.Major.ToString() + ver.Minor.ToString();
			return (s);
		}

		public List<Contacto> GetLista()
		{
			Contacto c;
			lsc.Clear();
			try
			{
				Version ver = new Version(UIDevice.CurrentDevice.SystemVersion);
				if (ver.Major <= 8)
				{
					NSError err;
					var iPhoneAddressBook = ABAddressBook.Create(out err);
					var authStatus = ABAddressBook.GetAuthorizationStatus();
					if (authStatus != ABAuthorizationStatus.Authorized)
					{
						iPhoneAddressBook.RequestAccess(delegate (bool granted, NSError error)
						{
							if (granted)
							{
								ABPerson[] myContacts = iPhoneAddressBook.GetPeople();
								string phone = string.Empty;
								foreach (ABPerson contact in myContacts)
								{
									ABMultiValue<string> phs = contact.GetPhones();
									if (phs.Count() > 0)
									{
										foreach (var stel in phs)
										{
											c = new Contacto()
											{
												Name = contact.FirstName + " " + contact.LastName,
												Number = stel.Value, //phs.First().Value,
												Photo = (contact.HasImage ? GetBitmap(contact.Image) : null),
											};
											lsc.Add(c);
										}
									}
								}
							}
						});
					}
					else {
						ABPerson[] myContacts = iPhoneAddressBook.GetPeople();
						string phone = string.Empty;
						foreach (ABPerson contact in myContacts)
						{
							ABMultiValue<string> phs = contact.GetPhones();
							if (phs.Count() > 0)
							{
								foreach (var stel in phs)
								{
									c = new Contacto()
									{
										Name = contact.FirstName + " " + contact.LastName,
										Number = stel.Value, //phs.First().Value,
										Photo = (contact.HasImage ? GetBitmap(contact.Image) : null),
									};
									lsc.Add(c);
								}
							}
						}
					}
				}
				if (ver.Major >= 9) 
				{
					//ios 9
					var store = new CNContactStore();
					NSError error;
					var fetchKeys = new NSString[] { CNContactKey.Identifier, CNContactKey.GivenName, CNContactKey.FamilyName, CNContactKey.PhoneNumbers, CNContactKey.ThumbnailImageData };
					string contid = store.DefaultContainerIdentifier;
					var predicate = CNContact.GetPredicateForContactsInContainer(contid);
					var contacts = store.GetUnifiedContacts(predicate, fetchKeys, out error);
					foreach (CNContact cc in contacts)
					{
						for (int i = 0; i < cc.PhoneNumbers.Count(); i++)
						{
							Contacto ct = new Contacto()
							{
								Id = cc.Identifier,
								Name = (!string.IsNullOrEmpty(cc.GivenName) ? cc.GivenName : "") + (!string.IsNullOrEmpty(cc.FamilyName) ? " " + cc.FamilyName : ""),
								Number = (cc.PhoneNumbers[i] != null ? cc.PhoneNumbers[i].Value.StringValue : ""),
								Photo = (cc.ThumbnailImageData != null ? GetBitmap(cc.ThumbnailImageData) : null),
							};
							lsc.Add(ct);
						}
					}
					/*lsc = contacts.Select(x => new Contacto()
					{
						Id = x.Identifier,
						Name = (!string.IsNullOrEmpty(x.GivenName) ? x.GivenName : "") + (!string.IsNullOrEmpty(x.FamilyName) ? " " + x.FamilyName : ""),
						Number = (x.PhoneNumbers.FirstOrDefault() != null ? x.PhoneNumbers.FirstOrDefault().Value.StringValue : ""),
						Photo = (x.ThumbnailImageData != null ? GetBitmap(x.ThumbnailImageData) : null),
					}).ToList();*/
				}
			}
			catch (Exception e)
			{

			}
			return (lsc);
		}
	}
}