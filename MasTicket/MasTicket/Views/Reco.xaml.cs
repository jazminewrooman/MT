using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MasTicket
{
	public partial class Reco : ContentPage
	{
        ObservableCollection<Recompensa> ls;

        protected override void OnAppearing()
        {
            lvRecompensas.BeginRefresh();
            lvRecompensas.IsGroupingEnabled = false;
            lvRecompensas.ItemsSource = ls;
            lvRecompensas.EndRefresh();

            base.OnAppearing();
        }

        public Reco ()
		{
			InitializeComponent ();
            Title = "Recompensas";
            ls = new ObservableCollection<Recompensa>();
            ls.Add(new Recompensa() { Id = "1", Titulo = "Gana $100 con Telcel!", Desc = "Por cada $1000 minimo de recarga en este mes, se te hara un reembolso en monedero de $100", Valido = "Valido hasta el 31/Julio/2016" });
            ls.Add(new Recompensa() { Id = "2", Titulo = "20% de descuento con Movistar!", Desc = "Obten cupones de descuento para tu siguiente recarga, a partir de recargas de $150", Valido = "Valido hasta el 15/Julio/2016" });
            lvRecompensas.IsGroupingEnabled = false;
            lvRecompensas.ItemsSource = ls;
            lvRecompensas.ItemSelected += async (sender, e) => {
                if (e.SelectedItem == null)
                    return;

                Recompensa r = e.SelectedItem as Recompensa;
                var action = await DisplayActionSheet("Opcion", "Cerrar", null, "Usar esta recompensa");
                if (action == "Usar esta recompensa")
                {
                    //CargarSaldo p = https);
                    //p.SetVal(1, new Opcion() { idopc = 1, opc = "Mexico", imgopc = "mx.png" }); //Pais - Mexico
                    //p.SetVal(2, new Opcion() { idopc = 4, opc = "Telcel", imgopc = "telcel.png" });
                    //p.SetVal(4, new Opcion() { idopc = 2, opc = "$100.00", imgopc = "" });
                    //p.NumeroRecarga(h.Celular);
                    //await App.Nav.PushAsync(p);
                }

                ((ListView)sender).SelectedItem = null;
            };
        }
	}

    public class Recompensa
    {
        public Recompensa() { }
        public string Id { get; set; }
        public string Titulo { get; set; }
        public string Desc { get; set; }
        public string Valido { get; set; }
    }
}

