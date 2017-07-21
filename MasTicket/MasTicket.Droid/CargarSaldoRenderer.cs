using System;
using Android.App;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using MasTicket;
using MasTicket.Droid;
using System.Linq;

[assembly: ExportRenderer(typeof(CargarSaldo), typeof(CargarSaldoRenderer))]

namespace MasTicket.Droid
{
    public class CargarSaldoRenderer : PageRenderer
    {
        public override void OnWindowFocusChanged(bool hasWindowFocus)
        {
            base.OnWindowFocusChanged(hasWindowFocus);
            ActionBar actionBar = (Context as Activity).ActionBar;
            actionBar.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.ParseColor("#e35102")));
        }
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            ActionBar actionBar = (Context as Activity).ActionBar;
            actionBar.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.ParseColor("#e35102")));
            actionBar.SetTitle(Resource.String.titulo);
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            ActionBar actionBar = (Context as Activity).ActionBar;
            actionBar.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.ParseColor("#e35102")));
        }


    }
}