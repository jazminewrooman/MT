using System;
using System.Linq;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MasTicket;
using Android.Content;

using Xamarin.Auth;
using Acr.UserDialogs;
using ImageCircle.Forms.Plugin.Droid;
using Xamarin.Forms;
using CaveBirdLabs.Forms.Platform.Android;
using CaveBirdLabs.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
//using Plugin.Toasts;
using DeviceOrientation.Forms.Plugin.Droid;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace MasTicket.Droid
{
    //[Activity(Theme = "@style/MyTheme.Base", Label = "MasTicket", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    //[Activity(Theme = "@style/MyTheme.Base", Label = "Asi Compras", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	[Activity(Name = "corit.asicompras.mainactivity", Theme = "@style/MyTheme.Base", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : XLabs.Forms.XFormsApplicationDroid // global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        //public override void OnBackPressed()
        //{
        //}
        private Action<int, Result, Intent> _activityResultCallback;

		public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			bool isOk = true;
			// If there are errors in the certificate chain, look at each error to determine the cause.
			if (sslPolicyErrors != SslPolicyErrors.None)
			{
				for (int i = 0; i < chain.ChainStatus.Length; i++)
				{
					if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
					{
						chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
						chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
						chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
						chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
						bool chainIsValid = chain.Build((X509Certificate2)certificate);
						if (!chainIsValid)
						{
							isOk = false;
						}
					}
				}
			}
			return isOk;
		}

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

			//ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

			System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

			//ImageCircleRenderer.Init();
			global::Xamarin.Forms.Forms.Init(this, bundle);

            XamForms.Controls.Droid.Calendar.Init();

            //DependencyService.Register<ToastNotificatorImplementation>(); // Register your dependency
            //ToastNotificatorImplementation.Init(this);

            DeviceOrientationImplementation.Init();

            ImageCircleRenderer.Init();
            UserDialogs.Init(this);
            
            if (!Resolver.IsSet) SetIoc();

            LoadApplication(new MasTicket.App());

            MessagingCenter.Subscribe<Page, string>(this, "Share", (sender, arg) =>
            {
                Share(arg);
            });
            MessagingCenter.Subscribe<Page>(this, "LogOutFace", (sender) =>
            {
                LogOutFace();
            });

			Xamarin.Facebook.FacebookSdk.SdkInitialize(Xamarin.Forms.Forms.Context); 			Xamarin.Facebook.AppEvents.AppEventsLogger.ActivateApp(Xamarin.Forms.Forms.Context);

        }

        public override void OnConfigurationChanged(global::Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            DeviceOrientationImplementation.NotifyOrientationChange(newConfig);
        }

        public void ConfigureActivityResultCallback(Action<int, Result, Intent> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            _activityResultCallback = callback;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (_activityResultCallback != null)
            {
                _activityResultCallback.Invoke(requestCode, resultCode, data);
                _activityResultCallback = null;
            }
        }

        private void SetIoc()
        {
            var resolverContainer = new SimpleContainer();
            Resolver.SetResolver(resolverContainer.GetResolver());
        }

        void LogOutFace()
        {
            AccountStore store = AccountStore.Create();
            var accounts = store.FindAccountsForService("MasTicket").ToList<Account>();
			if (accounts.Count() > 0)
			{
				foreach (Account a in accounts)
					store.Delete(a, "MasTicket");
			}
            accounts.Clear();
        }

        async void Share(string msg)
        {
            var intent = new Intent(Intent.ActionSend);
            intent.SetType("text/plain");

            intent.PutExtra(Intent.ExtraText, msg);

            var intentChooser = Intent.CreateChooser(intent, "Share via");

            StartActivityForResult(intentChooser, 0);
        }

        
    }
}

