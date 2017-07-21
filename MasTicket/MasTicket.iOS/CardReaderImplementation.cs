using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Xamarin.Forms;

using MasTicket.iOS; //enables registration outside of namespace
using MasTicket;
using UIKit;
using Card.IO;
using Foundation;

[assembly: Xamarin.Forms.Dependency(typeof(CardReaderImplementation))]

namespace MasTicket.iOS
{
	public class CardReaderImplementation : NSObject, ICardReader, ICardIOPaymentViewControllerDelegate
	{
		public Action<string, int, int, string, CreditCardType> CardFound { get; set; }
		private CardIOPaymentViewControllerDelegate paymentDelegate;

		public void ReadCard()
		{
			var appcontroller = UIApplication.SharedApplication.KeyWindow.RootViewController;
			//var paymentDelegate = new payvcd(); //PaymentViewControllerDelegate();
			//paymentDelegate = new CardIOPaymentViewControllerDelegate();
			var paymentViewController = new Card.IO.CardIOPaymentViewController(); //PaymentViewController(paymentDelegate);

			paymentViewController.CollectCVV = false;
			paymentViewController.CollectExpiry = false;
			paymentViewController.MaskManualEntryDigits = true;
			//paymentViewController.AppToken = "e63c673c88c44b179dcbaa9f7a1f76af";

			//paymentDelegate.UserDidProvideCreditCardInfo += (CreditCardInfo arg1, CardIOPaymentViewController arg2) => //  .OnScanCompleted += (viewController, cardInfo) =>
			//{
			//	if (arg1 != null)
			//	{
			//		if (CardFound != null)
			//		{
			//			CardFound(arg1.RedactedCardNumber, arg1.ExpiryMonth, arg1.ExpiryYear, arg1.CardholderName, arg1.CardType);
			//		}
			//	}

			//	appcontroller.DismissViewController(true, null);
			//};

			appcontroller.PresentViewController(paymentViewController, true, null);
		}

		public void UserDidCancelPaymentViewController(CardIOPaymentViewController paymentViewController)
		{
			Console.WriteLine("Scanning Canceled!");
		}
		public void UserDidProvideCreditCardInfo(CreditCardInfo cardInfo, CardIOPaymentViewController paymentViewController)
		{
			if (cardInfo != null)
			{
				if (CardFound != null)
				{
					CardFound(cardInfo.RedactedCardNumber, (int)cardInfo.ExpiryMonth, (int)cardInfo.ExpiryYear, cardInfo.CardholderName, cardInfo.CardType);
				}
			}

			//if (cardInfo == null)
			//{
			//	Console.WriteLine("Scanning Canceled!");
			//}
			//else {
			//	Console.WriteLine("Card Scanned: " + cardInfo.CardNumber);
			//}

			paymentViewController.DismissViewController(true, null);
		}
	}

  //  public class CardReaderImplementation : ICardReader
  //  {
		//public void ReadCard()
		//{
		//	var appcontroller = UIApplication.SharedApplication.KeyWindow.RootViewController;

		//	var paymentDelegate = new payvcd(); //PaymentViewControllerDelegate();
		//	var paymentViewController = new Card.IO.CardIOPaymentViewController(paymentDelegate); //PaymentViewController(paymentDelegate);

		//	paymentViewController.CollectCVV = false;
		//	paymentViewController.CollectExpiry = false;
		//	paymentViewController.MaskManualEntryDigits = true;
		//	//paymentViewController.AppToken = "e63c673c88c44b179dcbaa9f7a1f76af";

		//	//paymentDelegate.UserDidProvideCreditCardInfo += (CreditCardInfo arg1, CardIOPaymentViewController arg2) => //  .OnScanCompleted += (viewController, cardInfo) =>
		//	//{
		//	//	if (arg1 != null)
		//	//	{
		//	//		if (CardFound != null)
		//	//		{
		//	//			CardFound(arg1.RedactedCardNumber, arg1.ExpiryMonth, arg1.ExpiryYear, arg1.CardholderName, arg1.CardType);
		//	//		}
		//	//	}

		//	//	appcontroller.DismissViewController(true, null);
		//	//};

		//	appcontroller.PresentViewController(paymentViewController, true, null);
		//}

  //  }
}
