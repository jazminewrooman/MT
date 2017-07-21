using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MasTicket
{
	public partial class ProcesarWallet : ContentPage
	{
        public ProcesarWallet ()
		{
			InitializeComponent ();

            Title = "Detalle de la orden";

            btnProcesar.Clicked += (sender, ea) =>
            {
                //await DisplayAlert("Aviso", "Su recarga ha sido realizada con exito", "OK");
                //await App.Nav.PopToRootAsync(Constantes.animated);
                
				//await App.Nav.PushAsync(new nip(typeof(Ticket), TipoRecarga.Monedero), Constantes.animated);
            };
        }
	}
}
