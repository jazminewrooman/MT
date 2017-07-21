using System;
//using Android.App;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using MasTicket;
using MasTicket.iOS;
using System.Linq;
using Acr.UserDialogs;
using System.Collections.Generic;

[assembly: ExportRenderer(typeof(loginfacebook), typeof(loginfacebookRenderer))]

namespace MasTicket.iOS
{
	public class loginfacebookRenderer : PageRenderer
	{
		MasTicket.TipoSocial _ts;
		bool isShown;

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);
			_ts = (e.NewElement as loginfacebook)._ts;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			AccountStore store = AccountStore.Create();
			var acst = store.FindAccountsForService("MasTicket").ToList<Account>();
			if (acst.Count() > 0)
			{
				foreach (Account a in acst)
					store.Delete(a, "MasTicket");
			}
			acst.Clear();

			var accounts = AccountStore.Create().FindAccountsForService("MasTicket").ToList<Account>();

			//accounts.Clear();
			//AccountStore store = AccountStore.Create();
			//var accounts2 = store.FindAccountsForService("MasTicket").ToList<Account>();
			//if (accounts2.Count() > 0)
			//{
			//	foreach (Account a in accounts2)
			//		store.Delete(a, "MasTicket");
			//}
			//accounts2.Clear();


			var account = accounts.FirstOrDefault();

			OAuth2Authenticator auth = null;
			if (_ts == TipoSocial.facebook)
			{
				auth = new OAuth2Authenticator(
							clientId: Constantes.FBclientId,  // your OAuth2 client id
							scope: Constantes.FBscope,  // The scopes for the particular API you're accessing. The format for this will vary by API.
							authorizeUrl: new Uri(Constantes.FBauthorizeUrl),  // the auth URL for the service
							redirectUrl: new Uri(Constantes.FBredirectUrl)  // the redirect URL for the service
						);
			}
			else if (_ts == TipoSocial.google)
			{
				auth = new OAuth2Authenticator(clientId: Constantes.GGClientId,
					scope: Constantes.GGScope,
					authorizeUrl: new Uri(Constantes.GGAuthorizeUrl),
					redirectUrl: new Uri(Constantes.GGRedirectUrl)
					);
				
			}
			if (account == null)
			{
				if (!isShown)
				{
					isShown = true;
					auth.AllowCancel = true;

					//auth.Error += async (sender, e) =>
					//{
					//	Console.Write(e.Message);
					//};

					auth.Completed += async (sender, ea) =>
					{
						if (ea.IsAuthenticated)
						{
							var accessToken = ea.Account.Properties["access_token"].ToString();
							var expiresIn = Convert.ToDouble(ea.Account.Properties["expires_in"]);
							var expiryDate = DateTime.Now + TimeSpan.FromSeconds(expiresIn);

							// Now that we're logged in, make a OAuth2 request to get the user's id.
							OAuth2Request request = null;
							if (_ts == TipoSocial.facebook)
								request = new OAuth2Request("GET", new Uri(Constantes.FBgraph), null, ea.Account);
							else if (_ts == TipoSocial.google)
							{
								//Dictionary<string, string> dic = new Dictionary<string, string>();
								//dic.Add("grant_type", "authorization_code");
								request = new OAuth2Request("GET", new Uri(Constantes.GGUserInfoUrl), null, ea.Account);
							}

							var response = await request.GetResponseAsync();

							var obj = JObject.Parse(response.GetResponseText());
							var id = obj["id"].ToString().Replace("\"", ""); // Id has extraneous quotation marks
							Usuario u = new Usuario()
							{
								//id = id,
								email = (!String.IsNullOrEmpty(obj["email"].ToString()) ? obj["email"].ToString() : ""),
								first_name = (_ts == TipoSocial.facebook ? (!String.IsNullOrEmpty(obj["first_name"].ToString()) ? obj["first_name"].ToString() : "") : (!String.IsNullOrEmpty(obj["given_name"].ToString()) ? obj["given_name"].ToString() : "")),
								last_name = (_ts == TipoSocial.facebook ? (!String.IsNullOrEmpty(obj["last_name"].ToString()) ? obj["last_name"].ToString() : "") : (!String.IsNullOrEmpty(obj["family_name"].ToString()) ? obj["family_name"].ToString() : "")),
								name = (_ts == TipoSocial.google ? (!String.IsNullOrEmpty(obj["name"].ToString()) ? obj["name"].ToString() : "") : ""),
								gender = (!String.IsNullOrEmpty(obj["gender"].ToString()) ? obj["gender"].ToString() : ""),
								picture = (_ts == TipoSocial.facebook ? (!String.IsNullOrEmpty(obj["picture"]["data"]["url"].ToString()) ? obj["picture"]["data"]["url"].ToString() : "") : (!String.IsNullOrEmpty(obj["picture"].ToString()) ? obj["picture"].ToString() : ""))
							};
							if (_ts == TipoSocial.facebook)
								u.name = u.first_name + " " + u.last_name;
							u.registrado = false;
							App.usr = u;
							ea.Account.Username = u.email;
							App.Current.SuccessfulLoginAction.Invoke();
						}
						else
						{ //se cancelo
							App.Current.CancelAction.Invoke();
						}
						// var user = await ParseFacebookUtils.LogInAsync(id, accessToken, expiryDate);
					};
					//UserDialogs.Instance.HideLoading();
					PresentViewController(auth.GetUI(), true, null);
				}
			}
			else
			{
				if (!isShown)
				{
					App.usr = App.db.SelUsr(account.Username);
					//App.usr.id = account.Username;
					if (App.usr != null)
					{
						App.usr.registrado = false;
						App.Current.SuccessfulLoginAction.Invoke();
					}
				}
			}
		}


	}
}

