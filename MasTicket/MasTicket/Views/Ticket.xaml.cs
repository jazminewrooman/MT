using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
//using Acr.UserDialogs;
using Xamarin.Forms;

namespace MasTicket
{
    public enum TipoRecarga
    {
        RecargaTA,
        Monedero
    }

    public partial class Ticket : ContentPage
	{
        RecargasViewModel rvm;
		bool pregunto = false;

		protected override bool OnBackButtonPressed()
		{
			return false;
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();
			if (!pregunto)
			{
				pregunto = true;
				if (rvm.Tiporecarga == TipoRecarga.RecargaTA)
				{
					var res = await DisplayAlert("Aviso", "¿Desea programar esta recarga para que se realize de manera automatica?", "Si", "No");
					if (res)
						await App.Nav.PushAsync(new Programar(rvm));
				}
			}
		}

        public Ticket(RecargasViewModel r)
        {
            InitializeComponent();
            rvm = r;
            Title = "Ticket";

            if (rvm.Tiporecarga == TipoRecarga.RecargaTA)
            {
				lblTrans.Text = "TRANSACCION " + rvm.Err.tresp.transaction_id;
				lblFecha.Text = "FECHA " + DateTime.Now.ToString("dd/MMM/yyyy");
				lblCarrier.Text = rvm.LsOperadoras(rvm.idpais).Where(x => x.idoperadora == rvm.idoperadora).FirstOrDefault().operadora;
				lblMonto.Text = "MONTO: " + rvm.LsPaquetes().Where(x => x.idpaquete == rvm.idpaquete).FirstOrDefault().monto.ToString("c", new System.Globalization.CultureInfo("es-MX"));
				lblCel.Text = "celular: " + rvm.NumeroRecarga;
				lblAutorizacion.Text = "AUTORIZACION: " + rvm.Err.tresp.op_authorization;
				lblPrintdata.Text = rvm.Err.tresp.printDatam_data;

                lblCarrier.IsVisible = true;
                lblCel.IsVisible = true;
				lblTrans.IsVisible = true;
				lblCarrier.IsVisible = true;
				lblPrintdata.IsVisible = true;
            }
            if (rvm.Tiporecarga == TipoRecarga.Monedero)
            {
				lblFecha.Text = DateTime.Now.ToString("dd/MMM/yyyy");
				lblMonto.Text = rvm.MontoRecargaMonedero.ToString("c", new System.Globalization.CultureInfo("es-MX"));
				lblAutorizacion.Text = rvm.Err.PaymentID;

				lblCarrier.IsVisible = false;
                lblCel.IsVisible = false;
				lblTrans.IsVisible = false;
				lblCarrier.IsVisible = false;
				lblPrintdata.IsVisible = false;
			}
            btnCerrar.Clicked += async (sender, ea) =>
            {
				await App.Nav.PopToRootAsync(Constantes.animated);
            };


		}
	}
}
