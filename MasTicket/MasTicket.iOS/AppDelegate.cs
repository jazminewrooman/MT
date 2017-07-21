using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using CaveBirdLabs.Forms.PlatformiOS;
using CaveBirdLabs.Forms;
using Xamarin.Forms;
using Acr.UserDialogs;
using ImageCircle.Forms.Plugin.iOS;
using XLabs.Forms;
using XLabs.Ioc;
using Xamarin.Auth;
//using Plugin.Toasts;
using DeviceOrientation.Forms.Plugin.iOS;
using Rg.Plugins.Popup.IOS;
using Plugin.Share;

namespace MasTicket.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : XFormsApplicationDelegate
	//public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Popup.Init();

			global::Xamarin.Forms.Forms.Init ();
			Renderers.Init();
			DeviceOrientationImplementation.Init();

			//DependencyService.Register<ToastNotificatorImplementation>();
			//ToastNotificatorImplementation.Init();

			ImageCircleRenderer.Init();
			if (!Resolver.IsSet) SetIoc();
			LoadApplication (new MasTicket.App ());

			MessagingCenter.Subscribe<Page, string>(this, "Share", (sender, arg) =>
			{
				Share(arg);
			});
			MessagingCenter.Subscribe<Page>(this, "LogOutFace", (sender) =>
			{
				LogOutFace();
			});

			//ShareImplementation.ExcludedUIActivityTypes = new List<NSString> { UIActivityType.PostToFacebook };

			Facebook.CoreKit.AppEvents.ActivateApp();

			return base.FinishedLaunching(app, options);

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

		void Share(string msg)
		{
			//var item = NSObject.FromObject(msg);
			//var activityItems = new[] { item };
			//var activityController = new UIActivityViewController(activityItems, null);
			//var topController = UIApplication.SharedApplication.KeyWindow.RootViewController;
			//while (topController.PresentedViewController != null)
			//{
			//	topController = topController.PresentedViewController;
			//}
			//topController.PresentViewController(activityController, true, () => { });

			var activityController = new UIActivityViewController(new NSObject[] { UIActivity.FromObject(msg) }, null);
			UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(activityController, true, null);
		}
	}
}
