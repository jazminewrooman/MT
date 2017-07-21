using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using Xamarin.Forms;

namespace MasTicket
{
	public partial class Historicos : ContentPage
	{
        //List<Historico> ls = new List<Historico>();
        List<Recarga> lsr = new List<Recarga>();
        RecargasViewModel rvm;

        protected override void OnAppearing()
        {
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("es-MX");

            lvHistoricos.BeginRefresh();
            lvHistoricos.IsGroupingEnabled = false;
            lvHistoricos.ItemsSource = lsr;
            lvHistoricos.EndRefresh();

            base.OnAppearing();
        }

        public Historicos ()
		{
			try
			{
				InitializeComponent();
				rvm = new RecargasViewModel();
				Title = "Histórico de recargas";
				NavigationPage.SetBackButtonTitle(this, "");

				lsr = rvm.SelRecargas().Where(x => x.err == 0 && !String.IsNullOrEmpty(x.rsauthorization) && !String.IsNullOrEmpty(x.rstransactionid)).OrderByDescending(x => x.fecha).ToList();
				lvHistoricos.ItemSelected += async (sender, e) =>
				{
					if (e.SelectedItem == null)
						return;

					Recarga h = e.SelectedItem as Recarga;
					var action = await DisplayActionSheet("Opción", "Cerrar", null, "Recargar a este número");
					if (action == "Recargar a este número")
					{
						CargarSaldo p = new CargarSaldo(h);
						await App.Nav.PushAsync(p);
					}

					((ListView)sender).SelectedItem = null;
				};
			}
			catch (Exception ex)
			{
			}
        }

	}

	public class TelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string tel = value.ToString();
			string formatted = new StringBuilder(12).Append(tel, 0, 2).Append(" ").Append(tel, 2, 4).Append(' ').Append(tel, 6, 4).ToString();
			return (formatted);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

    public class CarrierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int idoperadora = (int)value;
            string ret = "";
            TarjetasViewModel tvm = new TarjetasViewModel();
            ret = tvm.LsOperadoras().Where(x => x.idoperadora == idoperadora).FirstOrDefault().operadora;
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MontoPaqueteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int idpaquete = (int)value;
            string ret = "";
            TarjetasViewModel tvm = new TarjetasViewModel();
			catPaquete cp = tvm.LsPaquetes().Where(x => x.idpaquete == idpaquete).FirstOrDefault();
			if (cp != null)
				ret = cp.monto.ToString("c", new System.Globalization.CultureInfo("es-MX"));
			else
				ret = 0.ToString("c", new System.Globalization.CultureInfo("es-MX"));
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusRecConverter : IValueConverter
    {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int err = (int)value;
			string ret = ""; catErrores e;
			if (err == 0)
				ret = "Éxito";
			else {
				//e = App.db.SelErrores().Where(x => x.iderror == err).FirstOrDefault();  //"Sin procesar"; break;
				//if (e == null)
					ret = "Error";
				//else
				//	ret = e.error;
			}
			return ret;
		}

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    
}
