using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using Xamarin.Forms;

namespace MasTicket
{
	public partial class Automaticas : ContentPage
	{
        //List<Programacion> ls = new List<Programacion>();
        RecargasViewModel rvm;
        List<RecargaProg> lsr = new List<RecargaProg>();

        protected override void OnAppearing()
        {
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("es-MX");

			lsr = rvm.SelRecargasProg();
            lvAutomaticas.BeginRefresh();
            lvAutomaticas.ItemsSource = null;
            lvAutomaticas.ItemsSource = lsr;
            lvAutomaticas.EndRefresh();
            base.OnAppearing();
        }

        public Automaticas ()
		{
			InitializeComponent ();
            Title = "Recargas automaticas";
            rvm = new RecargasViewModel();
            NavigationPage.SetBackButtonTitle(this, "");

			lvAutomaticas.ItemSelected += async (sender, e) => {
                if (e.SelectedItem == null)
                    return;
                var action = await DisplayActionSheet("Opcion", "Cerrar", null, "Modificar");
                if (action == "Modificar")
                {
					CargarSaldo p = new CargarSaldo(e.SelectedItem as RecargaProg);
                    await App.Nav.PushAsync(p);
                }
                ((ListView)sender).SelectedItem = null;
            };
        }

        public async void OnDelete(object sender, EventArgs e)
        {
            var mi = ((Xamarin.Forms.MenuItem)sender);
            RecargaProg del = (mi.CommandParameter as RecargaProg);
            bool ret = await DisplayAlert("Confirme", "¿Desea eliminar esta recarga automatica?", "Si", "No");
            if (ret)
            {
                rvm.DelRecargaFrecuente(del);
                if (lsr.Remove(del))
                {
                    lvAutomaticas.BeginRefresh();
                    lvAutomaticas.ItemsSource = null;
                    lvAutomaticas.ItemsSource = lsr;
                    lvAutomaticas.EndRefresh();
                }
            }
        }
    }

    public class TipoTarjetaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int idtarjeta = (int)value;
            string ret = "";
            TarjetasViewModel tvm = new TarjetasViewModel();
            if (idtarjeta == -1)
                ret = "Monedero";
            else
            {
                Tarjeta t = App.db.SelTarjetas().Where(x => x.idtarjeta == idtarjeta).FirstOrDefault();
                if (t != null)
                {
                    catEmisorTC em = tvm.LsEmisores().Where(x => x.idemisor == t.idemisor).FirstOrDefault();
					ret = (em != null ? em.emisor + " " : "") + t.Last4;
                }
            }
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }

    public class DiasMesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {  
            string diasmes = value.ToString();
            string ret = "";
            ret = "dia(s) " + diasmes + " de cada mes";
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
