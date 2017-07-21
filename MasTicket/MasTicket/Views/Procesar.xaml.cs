using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

using Xamarin.Forms;
using Acr.UserDialogs;

namespace MasTicket
{
	public enum TipoTransaccion
	{
		PrimeraVez,
		SegundaVez
	};

	public partial class Procesar : ContentPage
	{
		RecargasViewModel rvm;
		TarjetasViewModel tvm;
		EventHandler<CargaVesta1aVezCompletedEventArgs> av = null;
		EventHandler<CargaVesta2aVezCompletedEventArgs> av2 = null;
		EventHandler<AltaRecargaViaWalletCompletedEventArgs> aw = null;
		TipoTransaccion tipotran;

		public void Refresh(RecargasViewModel _r, TarjetasViewModel _t, TipoTransaccion tt)
		{
			rvm = _r;
			tvm = _t;
			tipotran = tt;
			CargaLabels();
		}

		public Procesar(RecargasViewModel _r, TarjetasViewModel _t, TipoTransaccion tt)
        {
			try
			{
				InitializeComponent();
				rvm = _r;
				tvm = _t;
				tipotran = tt;
				Title = "Detalle de la orden";
				CargaLabels();
				NavigationPage.SetBackButtonTitle(this, "");

				rvm.RecargaAltaErr += async (s, e) =>
				{
					UserDialogs.Instance.HideLoading();
					if (e != null)
					{
						//if (!String.IsNullOrEmpty(e.printdata))
						//	await DisplayAlert("Error", "Ocurrio un error, vuelva a intentar\n" + e.printdata, "OK");
						//else
						string msg = "";
						switch (e.err)
						{
							case 1: //RecargaSell
								msg = rvm.lsErrores().Where(x => x.iderror == rvm.Err.errRs).FirstOrDefault().error;
								break;
							case 2: //Vesta
								msg = rvm.lsErrores().Where(x => x.iderror == rvm.Err.errVs).FirstOrDefault().error;
								break;
							case 3: //Comunicaciones
								msg = rvm.lsErrores().Where(x => x.iderror == 9).FirstOrDefault().error;
								break;
						}
						if (!String.IsNullOrEmpty(e.msg))
							msg = e.msg;
						await DisplayAlert("Error", "Ocurrio un error, vuelva a intentar\n" + msg, "OK");
					}
					else
						await DisplayAlert("Error", "Ocurrio un error, vuelva a intentar", "OK");
				};
				rvm.CargaFinger += (s, e) =>
				{
					FingerP(e.url, e.orgid, e.webses);
				};

				av = async (s, ea) =>
				{
					rvm.ProcesadoEnVesta1aVez -= av;
					UserDialogs.Instance.HideLoading();
					if (ea.Error != null || ea.Cancelled)
						await DisplayAlert("Error", "Ocurrio un error, vuelva a intentar", "OK");
					else {
						string msg = "";
						if (rvm.Err.err > 0)
						{
							if (rvm.Tiporecarga == TipoRecarga.RecargaTA)
							{
								switch (rvm.Err.err)
								{
									case 1: //RecargaSell
										msg = rvm.lsErrores().Where(x => x.iderror == rvm.Err.errRs).FirstOrDefault().error;
										break;
									case 2: //Vesta
										msg = rvm.lsErrores().Where(x => x.iderror == rvm.Err.errVs).FirstOrDefault().error;
										break;
									case 3: //Comunicaciones
										msg = rvm.lsErrores().Where(x => x.iderror == 9).FirstOrDefault().error;
										break;
								}
								errRecarga err = JsonConvert.DeserializeObject<errRecarga>(ea.Result);
								msg += "\n" + err.tresp.rcode_description;
							}
							if (rvm.Tiporecarga == TipoRecarga.Monedero)
							{
								if (rvm.Err.err == 2)
									msg = rvm.lsErrores().Where(x => x.iderror == rvm.Err.errVs).FirstOrDefault().error;
								if (rvm.Err.err == 1) // err de RS, en este caso no es RS es monedero, se usa el mismo campo
									msg = rvm.lsErrores().Where(x => x.iderror == rvm.Err.errRs).FirstOrDefault().error;
								errRecarga err = JsonConvert.DeserializeObject<errRecarga>(ea.Result);
								msg += "\n" + err.tresp.rcode_description;
							}
							await DisplayAlert("Error", msg, "OK");
						}
						else
							await App.Nav.PushAsync(new Ticket(rvm), Constantes.animated);
					}
				};
				av2 = async (s, ea) =>
				{
					rvm.ProcesadoEnVesta2aVez -= av2;
					UserDialogs.Instance.HideLoading();
					if (ea.Error != null || ea.Cancelled)
						await DisplayAlert("Error", "Ocurrio un error, vuelva a intentar", "OK");
					else {
						string msg = "";
						if (rvm.Err.err > 0)
						{
							if (rvm.Tiporecarga == TipoRecarga.RecargaTA)
							{
								switch (rvm.Err.err)
								{
									case 1: //RecargaSell
										msg = rvm.lsErrores().Where(x => x.iderror == rvm.Err.errRs).FirstOrDefault().error;
										break;
									case 2: //Vesta
										msg = rvm.lsErrores().Where(x => x.iderror == rvm.Err.errVs).FirstOrDefault().error;
										break;
									case 3: //Comunicaciones
										msg = rvm.lsErrores().Where(x => x.iderror == 9).FirstOrDefault().error;
										break;
								}
								errRecarga err = JsonConvert.DeserializeObject<errRecarga>(ea.Result);
								msg += "\n" + err.tresp.rcode_description;
							}
							if (rvm.Tiporecarga == TipoRecarga.Monedero)
							{
								if (rvm.Err.err == 2)
									msg = rvm.lsErrores().Where(x => x.iderror == rvm.Err.errVs).FirstOrDefault().error;
								if (rvm.Err.err == 1)
									msg = rvm.lsErrores().Where(x => x.iderror == 10).FirstOrDefault().error;
							}
							await DisplayAlert("Error", msg, "OK");
						}
						else
							await App.Nav.PushAsync(new Ticket(rvm), Constantes.animated);
					}
				};

				aw = async (s, ea) =>
				{
					rvm.AltaViaWallet -= aw;
					UserDialogs.Instance.HideLoading();
					if (ea.Error != null || ea.Cancelled)
					{
						await DisplayAlert("Error", "Ocurrio un error, vuelva a intentar", "OK");
						//await App.Nav.PopAsync(Constantes.animated);
					}
					else {
						string msg = "";
						if (rvm.Err.err > 0)
						{
							switch (rvm.Err.err)
							{
								//  1 error no hay suficiente saldo
								//  2 error de sistema (BD)
								//  3 error de RS
								case 1: 
									msg = "No se cuenta con suficiente saldo";
									break;
								case 2: 
									msg = "Error de sistema";
									break;
								case 3: 
									msg = "Error en la plataforma, contacte al número de soporte para atención";
									break;
							}
							errRecarga err = JsonConvert.DeserializeObject<errRecarga>(ea.Result);
							msg += "\n" + err.tresp.rcode_description;
							await DisplayAlert("Error", msg, "OK");
							//await App.Nav.PopAsync(Constantes.animated);
						}
						else
						{
							//await App.Nav.PopAsync(Constantes.animated);
							await App.Nav.PushAsync(new Ticket(rvm), Constantes.animated);
						}
					}
				};

				btnProcesar.Clicked += async (sender, ea) =>
				{
					nip n = new nip();
					n.NipCorrecto += async (s, e) =>
					{
						if (rvm.Tiporecarga == TipoRecarga.Monedero)
						{
							UserDialogs.Instance.ShowLoading("Recargando...");
							await App.Nav.PopAsync(Constantes.animated);
							if (tt == TipoTransaccion.PrimeraVez)
							{
								rvm.TipoTrans = TipoTransaccion.PrimeraVez;
								rvm.ProcesadoEnVesta1aVez += av;
								rvm.AltaMonedero(tvm);
							}
							else {
								rvm.TipoTrans = TipoTransaccion.SegundaVez;
								rvm.ProcesadoEnVesta2aVez += av2;
								rvm.AltaMonedero(tvm);
							}
						}
						if (rvm.Tiporecarga == TipoRecarga.RecargaTA)
						{
							UserDialogs.Instance.ShowLoading("Recargando...");
							await App.Nav.PopAsync(Constantes.animated);
							if (tt == TipoTransaccion.PrimeraVez)
							{
								rvm.TipoTrans = TipoTransaccion.PrimeraVez;
								if (rvm.idFormaPago == 1)
									rvm.AltaViaWallet += aw;
								if (rvm.idFormaPago == 2)
									rvm.ProcesadoEnVesta1aVez += av;
								rvm.Alta(tvm);
							}
							else {
								rvm.TipoTrans = TipoTransaccion.SegundaVez;
								if (rvm.idFormaPago == 1)
									rvm.AltaViaWallet += aw;
								if (rvm.idFormaPago == 2)
									rvm.ProcesadoEnVesta2aVez += av2;
								rvm.Alta(tvm);
							}
						}
					};
					await App.Nav.PushAsync(n, Constantes.animated);
				};

				btnMedios.Clicked += async (sender, ea) =>
				{
					NuevaTarjeta nt = App.Nav.NavigationStack.OfType<NuevaTarjeta>().FirstOrDefault();
					if (nt != null)
						await App.Nav.PopAsync(Constantes.animated);
					else {
						RegPago rp = App.Nav.NavigationStack.OfType<RegPago>().FirstOrDefault();
						if (rp != null)
							await App.Nav.PopAsync(Constantes.animated);
						else
							await App.Nav.PushAsync(new RegPago(rvm), Constantes.animated);
					}
				};
			}
			catch (Exception ex){
				
			}
        }

		private async void CargaVesta1aVez()
		{
			
		}


        private void FingerP(string fingerpurl, string orgid, string websess)
		{
			var html = new HtmlWebViewSource();
			string s = String.Format(@"<html>
    <head>
    </head>
    <body><form>
<p style=""background:url('{0}/ThreatMetrixUIRedirector/fp/clear.png?org_id={1}&session_id={2}&m=1')""></p>
<img src=""{0}/ThreatMetrixUIRedirector/fp/clear.png?org_id={1}&session_id={2}&m=2"" alt=""""/>
<script src=""{0}/ThreatMetrixUIRedirector/fp/check.js?org_id={1}&session_id={2}"" type=""text/javascript""></script>
<object data=""{0}/ThreatMetrixUIRedirector/fp/fp.swf?org_id={1}&session_id={2}"" type=""application/x-shockwave-flash"" width=""1"" height=""1"" id=""obj_id"">
<param value=""{0}/ThreatMetrixUIRedirector/fp/fp.swf?org_id={1}&session_id={2}"" name=""movie"" /> </object>
    </form></body>
    </html>", fingerpurl, orgid, websess);
			html.Html = s;
			wv.Source = html;
		}

		private void CargaLabels()
		{
			if (rvm.Tiporecarga == TipoRecarga.RecargaTA)
			{
				grdRecarga.IsVisible = true;
				grdMonedero.IsVisible = false;
				lblPais.Text = rvm.lsPais().Where(x => x.idpais == rvm.idpais).FirstOrDefault().pais;
				lblOperadora.Text = rvm.LsOperadoras(rvm.idpais).Where(x => x.idoperadora == rvm.idoperadora).FirstOrDefault().operadora;
				string formatted = new StringBuilder(12).Append(rvm.NumeroRecarga, 0, 2).Append(" ").Append(rvm.NumeroRecarga, 2, 4).Append(' ').Append(rvm.NumeroRecarga, 6, 4).ToString();
				lblNumero.Text = formatted + (!String.IsNullOrEmpty(rvm.ContactoRecarga) ? " (" + rvm.ContactoRecarga + ")" : "");
				lblMonto.Text = rvm.LsPaquetes(rvm.idoperadora).Where(x => x.idpaquete == rvm.idpaquete).FirstOrDefault().monto.ToString("c", new System.Globalization.CultureInfo("es-MX"));
				lblFormapago.Text = (rvm.idFormaPago == 1 ? "Monedero" : "Tarjeta");
			}
			if (rvm.Tiporecarga == TipoRecarga.Monedero)
			{
				grdRecarga.IsVisible = false;
				grdMonedero.IsVisible = true;
				lblMontoMon.Text = rvm.MontoRecargaMonedero.ToString("c", new System.Globalization.CultureInfo("es-MX"));
				lblRecargaMon.Text = "Recarga de monedero";
				lblFormapagoMon.Text = (rvm.idFormaPago == 1 ? "Monedero" : "Tarjeta");
			}
			if (rvm.idFormaPago == 2)
			{
				string tipo = "";
				if (rvm.TipoTrans == TipoTransaccion.PrimeraVez)
				{
					catEmisorTC em = rvm.LsEmisores().Where(x => x.idemisor == tvm.idemisor).FirstOrDefault();
					tipo = (em != null ? em.emisor + " " : "") + tvm.NumeroTarjeta.Substring(tvm.NumeroTarjeta.Length - 4, 4);
				}
				else {
					Tarjeta t = App.db.SelTarjetas().Where(x => x.idtarjeta == rvm.IdTarjeta).FirstOrDefault();
					catEmisorTC em = rvm.LsEmisores().Where(x => x.idemisor == t.idemisor).FirstOrDefault();
					tipo = (em != null ? em.emisor + " " : "") + t.Last4;
				}
				if (rvm.Tiporecarga == TipoRecarga.RecargaTA)
				{
					lblEmisor.Text = tipo;
					lblEmisor.IsVisible = true;
				}
				if (rvm.Tiporecarga == TipoRecarga.Monedero)
				{
					lblEmisorMon.Text = tipo;
					lblEmisorMon.IsVisible = true;
				}
			}
			else
				lblEmisor.IsVisible = false;
		}
        
    }
}
