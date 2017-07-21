using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;

namespace MasTicket
{
	public partial class Monedero : ContentPage
	{
        List<TipoPago> lt = new List<TipoPago>();
        ObservableCollection<Grupo> grptipos;
        Grupo gTarjetas;

        public Monedero ()
		{
			InitializeComponent ();

            Title = "Monedero";

            grptipos = new ObservableCollection<Grupo>();
            lt.Add(new TipoPago() { idtipo = 2, tipo = "VISA 2152", saldo = 0, imgtipo = "visa.png" });
            gTarjetas = new Grupo("Tus tarjetas", 2, lt);
            grptipos.Add(gTarjetas);

            lvRegPagos.ItemSelected += async (sender, e) => {
                if (e.SelectedItem == null)
                    return;
                await App.Nav.PushAsync(new ProcesarWallet(), Constantes.animated);
                ((ListView)sender).SelectedItem = null;
            };

            btnNuevo.Clicked += (sender, ea) =>
            {
                try
                {
                    //await App.Nav.PushAsync(new nip(typeof(NuevaTarjeta), TipoAnadidor.Monedero), Constantes.animated);
                }
                catch (Exception e)
                {

                }
            };
        }

        public void NuevaT()
        {
            lt.Add(new TipoPago() { idtipo = 2, tipo = "AMEX 0133", saldo = 0, imgtipo = "amex.png" });
            gTarjetas = new Grupo("Tus tarjetas", 2, lt);
            grptipos.Clear();
            grptipos.Add(gTarjetas);
        }

        protected override void OnAppearing()
        {
            lvRegPagos.BeginRefresh();
            lvRegPagos.ItemsSource = null;
            lvRegPagos.ItemsSource = grptipos;
            lvRegPagos.EndRefresh();

            base.OnAppearing();
        }
    }
}
