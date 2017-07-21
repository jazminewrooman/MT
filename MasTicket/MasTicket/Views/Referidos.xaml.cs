using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
//using Plugin.Share;
using Acr.UserDialogs;

namespace MasTicket
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Referidos : ContentPage
    {
        string msg;

        public Referidos()
        {
            InitializeComponent();

            Title = "Referidos";

            string codigo = "";

            UserDialogs.Instance.ShowLoading("Cargando codigo...");
            codigo = App.WSc.GetCodigoReferidoUsr(App.usr.idusuario);
            UserDialogs.Instance.HideLoading();

			var labelFormatted = new Label();
			var fs = new FormattedString();
            fs.Spans.Add(new Span { Text = "Comparte este codigo con tus amigos, cuando ellos recarguen tu ganas saldo: ", FontAttributes = FontAttributes.None });
            fs.Spans.Add(new Span { Text = codigo, FontAttributes = FontAttributes.Bold });
			lblCodigo.FormattedText = fs;

            btnComp.Clicked += (sender, ea) =>
            {
#if __IOS__
				//var title = "Así compras";
				//var message = "¡Baja esta App y gana credito al instante! http://asicompras.com";
				//var url = "http://asicompras.com";
				//await CrossShare.Current.ShareLink(url, message, title);

				MessagingCenter.Send<Page, string>(this, "Share", "Así Compras, ¡Te conviene!. Recarga tu celular, el de tu familia y amigos en cualquier momento y lugar. Conoce la forma inteligente de ahorrar tiempo y dinero de manera segura. http://asicompras.com");
#else
#if __ANDROID__
				MessagingCenter.Send<Page, string>(this, "Share", "Así Compras, ¡Te conviene!. Recarga tu celular, el de tu familia y amigos en cualquier momento y lugar. Conoce la forma inteligente de ahorrar tiempo y dinero de manera segura. http://asicompras.com");
#endif
#endif
			};
        }
    }
}

