using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
using Card.IO;

[assembly: UsesPermission(Android.Manifest.Permission.AccessNetworkState)]
[assembly: UsesPermission(Android.Manifest.Permission.Internet)]
[assembly: UsesPermission(Android.Manifest.Permission.Camera)]
[assembly: UsesPermission(Android.Manifest.Permission.Vibrate)]

[assembly: UsesFeature("android.hardware.camera", Required = false)]
[assembly: UsesFeature("android.hardware.camera.autofocus", Required = false)]
[assembly: UsesFeature("android.hardware.camera.flash", Required = false)]

[assembly: Xamarin.Forms.Dependency(typeof(CardReaderImplementation))]

namespace MasTicket.Droid
{
    public class CardReaderImplementation : Activity, ICardReader
    {
        private const int reqcode = 101;

        public Action<string, int, int, string, Card.IO.CardType> CardFound { get; set; }
        
        public void ReadCard()
        {
            MainActivity ctx = (MainActivity)Forms.Context;
            var activity = Forms.Context as MainActivity;
            var intent = new Intent(activity, typeof(CardIOActivity));
            intent.PutExtra(CardIOActivity.ExtraRequireExpiry, true);
            intent.PutExtra(CardIOActivity.ExtraRequireCvv, false);
            intent.PutExtra(CardIOActivity.ExtraRequirePostalCode, false);
            intent.PutExtra(CardIOActivity.ExtraUseCardioLogo, false);
            intent.PutExtra(CardIOActivity.ExtraHideCardioLogo, true);
            intent.PutExtra(CardIOActivity.ExtraKeepApplicationTheme, false);
            intent.PutExtra(CardIOActivity.ExtraLanguageOrLocale, "es_MX");
            intent.PutExtra(CardIOActivity.ExtraScanExpiry, true);
            intent.PutExtra(CardIOActivity.ExtraSuppressConfirmation, true);
            intent.PutExtra(CardIOActivity.ExtraUsePaypalActionbarIcon, false);
            intent.PutExtra(CardIOActivity.ExtraSuppressManualEntry, true);
            intent.PutExtra(CardIOActivity.ExtraGuideColor, Android.Graphics.Color.Red);
            //intent.PutExtra(CardIOActivity.ExtraScanInstructions, "Alo");

            ctx.ConfigureActivityResultCallback(res);
            ctx.StartActivityForResult(intent, reqcode);
            //activity.StartActivity(intent, OnActivityResult);
        }

        private void res(int requestCode, Result resultCode, Intent data)
        {
            
            if (data != null)
            {
                var card = data.GetParcelableExtra(CardIOActivity.ExtraScanResult).JavaCast<CreditCard>();
                if (card != null)
                {
                    CardFound(card.FormattedCardNumber, card.ExpiryMonth, card.ExpiryYear, card.CardholderName, card.CardType);
                }
            }
        }
    }
}
