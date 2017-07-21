using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MasTicket
{
	public partial class Frecuentes : ContentPage
	{
        List<letragrupoHist> Groups = new List<letragrupoHist>();
        List<Historico> ls = new List<Historico>();

		public Frecuentes ()
		{
			InitializeComponent ();

            Title = "Historico de recargas";

            ls.Add(new Historico() { Id = "1", Monto = 50, Carrier = "Telcel", Celular = "55588235", Fecha = new DateTime(2016,6,3) });
            ls.Add(new Historico() { Id = "2", Monto = 100, Carrier = "ATT", Celular = "55588276", Fecha = new DateTime(2016, 6, 5) });
            ls.Add(new Historico() { Id = "3", Monto = 50, Carrier = "Telcel", Celular = "5558821212", Fecha = new DateTime(2016, 5, 6) });
            ls.Add(new Historico() { Id = "4", Monto = 150, Carrier = "Virgin", Celular = "5558821212", Fecha = new DateTime(2016, 5, 6) });
            ls.Add(new Historico() { Id = "5", Monto = 100, Carrier = "Telcel", Celular = "5558821212", Fecha = new DateTime(2016, 4, 3) });

            Groups.Clear();
            Groups.Add(new letragrupoHist("Semana pasada", ls.Where(x => x.Id == "1")));
            Groups.Add(new letragrupoHist("Mes pasado", ls.Where(x => x.Id == "2")));
            Groups.Add(new letragrupoHist("Dos meses", ls.Where(x => x.Id != "1" && x.Id != "2")));

            lvContactos.ItemSelected += async (sender, e) => {
                if (e.SelectedItem == null)
                    return;
                //var cs = App.Nav.NavigationStack.OfType<CargarSaldo>().First();
                //Historico h = e.SelectedItem as Historico;
                //(cs as CargarSaldo).SetVal(1, new Opcion() { idopc = 1, opc = "Mexico", imgopc = "mx.png" }); //Pais - Mexico
                //(cs as CargarSaldo).SetVal(2, new Opcion() { idopc = 4, opc = "Telcel", imgopc = "telcel.png" });
                //(cs as CargarSaldo).SetVal(4, new Opcion() { idopc = 2, opc = "$100.00", imgopc = "" });
                //(cs as CargarSaldo).NumeroRecarga(h.Celular);

                await App.Nav.PopAsync(Constantes.animated);
                ((ListView)sender).SelectedItem = null;
            };
        }

        protected override void OnAppearing()
        {
            lvContactos.BeginRefresh();
            lvContactos.IsGroupingEnabled = true;
            lvContactos.ItemsSource = Groups;
            lvContactos.EndRefresh();

            base.OnAppearing();
        }

    }
}
