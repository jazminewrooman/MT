using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Provider;
using Android.Graphics;

using Xamarin.Forms;

using MasTicket.Droid; //enables registration outside of namespace
using MasTicket;

[assembly: Dependency(typeof(BaseUrlImplementation))]
namespace MasTicket.Droid
{
    public class BaseUrlImplementation : IBaseUrl
    {
        public string Get()
        {
            return "file:///android_asset/";
        }
    }
}