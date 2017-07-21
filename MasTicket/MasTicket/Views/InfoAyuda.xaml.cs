using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

using Xamarin.Forms;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using DeviceOrientation.Forms.Plugin.Abstractions;

namespace MasTicket
{
	public partial class InfoAyuda : PopupPage
    {
        public InfoAyuda (string htmlf)
		{
			InitializeComponent ();
            CambiaOrientacion();
            MessagingCenter.Subscribe<DeviceOrientationChangeMessage>(this, DeviceOrientationChangeMessage.MessageId, (message) =>
            {
                CambiaOrientacion();
            });

            this.IsBackgroundAnimating = true;
            this.IsCloseOnBackgroundClick = true;
            this.IsAnimating = true;

//            var assembly = typeof(InfoAyuda).GetTypeInfo().Assembly;
//#if __IOS__
//            Stream stream = assembly.GetManifestResourceStream("MasTicket.iOS." + htmlf);
//#endif
//#if __ANDROID__
//            Stream stream = assembly.GetManifestResourceStream("MasTicket.Droid.Resources.html.inforecarga.html");
//#endif
//#if WINDOWS_PHONE
//            Stream stream = assembly.GetManifestResourceStream("MasTicket.WinPhone.inforecarga.html");
//#endif
//            string text = "";
//            using (var reader = new System.IO.StreamReader(stream))
//            {
//                text = reader.ReadToEnd();
//            }

//            var htmlSource = new HtmlWebViewSource();
//            htmlSource.Html = text;
            wv.Source = "http://asicompras.com/ayudaapp/" + htmlf;
        }

        private void CambiaOrientacion()
        {
            IDeviceOrientation _deviceOrientationSvc = DependencyService.Get<IDeviceOrientation>();
            DeviceOrientations dvcor = _deviceOrientationSvc.GetOrientation();
            if (dvcor == DeviceOrientations.Landscape) //apaisado
            {
                wv.HeightRequest = 300;
                wv.WidthRequest = 300;
            }
            else //portrait
            {
                wv.HeightRequest = 400;
                wv.WidthRequest = 300;
            }
        }

        private void OnClose(object sender, EventArgs e)
        {
            PopupNavigation.PopAsync();
        }
    }
}
