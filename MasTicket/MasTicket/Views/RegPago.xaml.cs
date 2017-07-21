using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Globalization;
using Newtonsoft.Json;

using Xamarin.Forms;
using Rg.Plugins.Popup.Extensions;
using Acr.UserDialogs;

namespace MasTicket
{
    public partial class 	RegPago : ContentPage
    {
        ObservableCollection<Grupo> grptipos;
        Grupo gMonedero; Grupo gTarjetas;
		TarjetasViewModel tvm;
		RecargasViewModel rvm;
		decimal montomax = 0M;

		public RegPago(RecargasViewModel _r = null)
		{
            try
            {
                InitializeComponent();
				App.db.IniciaMonedero();

				montomax = App.WSc.GetMontoMax();

                Title = "Medios de pago";
				rvm = _r;
                tvm = new TarjetasViewModel();
				tvm.idpais = rvm.idpais;
                this.BindingContext = tvm;
				NavigationPage.SetBackButtonTitle(this, "");
                ToolbarItems.Add(new ToolbarItem("Ayuda", "ayuda2.png", () =>
                {
                    var page = new InfoAyuda("mediosdepago.html");
                    Navigation.PushPopupAsync(page);
                }));

                grptipos = new ObservableCollection<Grupo>();
                gMonedero = new Grupo("Tu monedero", 1, tvm.lm);
                gTarjetas = new Grupo("Tus tarjetas", 2, tvm.lt);
                if (rvm.Tiporecarga == TipoRecarga.RecargaTA)
                {
                    slMonedero.IsVisible = false;
                    grptipos.Add(gMonedero);
                }
                if (rvm.Tiporecarga == TipoRecarga.Monedero)
                {
                    slMonedero.IsVisible = true;
                    lblSaldo.Text = "Saldo: " + tvm.lm[0].saldo.ToString("c", new System.Globalization.CultureInfo("es-MX"));
                }
                grptipos.Add(gTarjetas);

                lvRegPagos.ItemSelected += async (sender, e) =>
                {
                    if (e.SelectedItem == null)
                        return;
                    if (rvm.EdicionOnly) //no se recarga, son programadas solo se modifica
                    {
                        TipoPago tp = (e.SelectedItem as TipoPago);
                        rvm.idFormaPago = tp.idtipo;
						rvm.TipoTrans = TipoTransaccion.SegundaVez;
                        if (tp.idtipo == 1) //monedero
                            rvm.IdTarjeta = -1;
                        else
                            rvm.IdTarjeta = tp.extra;
                        await App.Nav.PushAsync(new Programar(rvm));
                    }
                    else
                    {
                        if (!rvm.ReadOnly)//mis tarjetas
                        {
                            if (rvm.Tiporecarga == TipoRecarga.Monedero)
                            {
								if (phvalNum.IsValid && phvalNum.Valor > 0M)
								{
									if (phvalNum.Valor > montomax)
									{
										await DisplayAlert("Error", "Solamente le es permitido agregar " + montomax.ToString("c", new System.Globalization.CultureInfo("es-MX")) + " por cada evento", "OK");
										((ListView)sender).SelectedItem = null;
										return;
									}
									else
										rvm.MontoRecargaMonedero = phvalNum.Valor;
								}
								else
								{
									await DisplayAlert("Error", "Para seleccionar una tarjeta tiene que capturar el monto a comprar", "OK");
									((ListView)sender).SelectedItem = null;
									return;
									//((ListView)sender).SelectedItem = null;
								}
                            }
                            TipoPago tp = (e.SelectedItem as TipoPago);
                            rvm.idFormaPago = tp.idtipo;
							rvm.TipoTrans = TipoTransaccion.SegundaVez;
							if (tp.idtipo == 1) //monedero
							{
								if (rvm.LsPaquetes(rvm.idoperadora).Where(x => x.idpaquete == rvm.idpaquete).FirstOrDefault().monto > App.db.SelSaldo().saldo)
								{
									await DisplayAlert("Error", "No tiene suficiente saldo", "Ok");
									((ListView)sender).SelectedItem = null;
									return;
								}
								else
									rvm.IdTarjeta = -1;
							}
							else
								rvm.IdTarjeta = tp.extra;
							Procesar pro = App.Nav.NavigationStack.OfType<Procesar>().FirstOrDefault();
							if (pro == null)
								await App.Nav.PushAsync(new Procesar(rvm, tvm, TipoTransaccion.SegundaVez), Constantes.animated);
							else {
								pro.Refresh(rvm, tvm, TipoTransaccion.SegundaVez);
								await App.Nav.PopAsync(Constantes.animated);
							}
                        }
                    }
                    ((ListView)sender).SelectedItem = null;
                };

				tvm.NuevaTarjeta += (s, e) =>
				{
					if (rvm.Tiporecarga == TipoRecarga.Monedero)
					{
						gTarjetas = grptipos[0];
						gTarjetas = new Grupo("Tus tarjetas", 2, e.lista);
						grptipos[0] = gTarjetas;
					}
					if (rvm.Tiporecarga == TipoRecarga.RecargaTA)
					{
						gTarjetas = grptipos[1];
						gTarjetas = new Grupo("Tus tarjetas", 2, e.lista);
						grptipos[1] = gTarjetas;
					}
					lvRegPagos.BeginRefresh();
					lvRegPagos.ItemsSource = null;
					lvRegPagos.ItemsSource = grptipos;
					lvRegPagos.EndRefresh();
					if (rvm.Tiporecarga == TipoRecarga.RecargaTA)
					{
						if ((grptipos[0].FirstOrDefault().saldo == 0M) && (grptipos[1].Count() == 0))
						{
							slMonedero.IsVisible = false;
							lvRegPagos.IsVisible = false;
							svMsgNoTarj.IsVisible = true;
						}
						else
						{
							slMonedero.IsVisible = false;
							lvRegPagos.IsVisible = true;
							svMsgNoTarj.IsVisible = false;
						}
					}
					if (rvm.Tiporecarga == TipoRecarga.Monedero)
					{
						//if (grptipos[0].Count() == 0)
						//{
						//	slMonedero.IsVisible = false;
						//	lvRegPagos.IsVisible = false;
						//	svMsgNoTarj.IsVisible = true;
						//}
						//else
						//{
							slMonedero.IsVisible = true;
							lvRegPagos.IsVisible = true;
							svMsgNoTarj.IsVisible = false;
						//}
					}
				};

                btnNuevo.Clicked += async (sender, ea) =>
                {
					if (rvm.Tiporecarga == TipoRecarga.Monedero)
					{
						if (phvalNum.IsValid && phvalNum.Valor > 0M)
						{
							if (phvalNum.Valor > montomax)
								await DisplayAlert("Error", "Solamente le es permitido agregar " + montomax.ToString("c", new System.Globalization.CultureInfo("es-MX")) + " por cada evento", "OK");
							else {
								rvm.MontoRecargaMonedero = phvalNum.Valor;
								await App.Nav.PushAsync(new NuevaTarjeta(TipoAnadidor.RegistroPago, tvm, rvm), Constantes.animated);
							}
						}
						else {
							await DisplayAlert("Error", "Para agregar una nueva tarjeta tiene que capturar el monto a comprar", "OK");
						}
					}
					if (rvm.Tiporecarga == TipoRecarga.RecargaTA)
						await App.Nav.PushAsync(new NuevaTarjeta(TipoAnadidor.RegistroPago, tvm, rvm), Constantes.animated);
                };

                if (rvm.Tiporecarga == TipoRecarga.RecargaTA) {
                    if ((grptipos[0].FirstOrDefault().saldo == 0M) && (grptipos[1].Count() == 0))
                    {
                        slMonedero.IsVisible = false;
                        lvRegPagos.IsVisible = false;
                        svMsgNoTarj.IsVisible = true;
                    }
                    else
                    {
                        slMonedero.IsVisible = false;
                        lvRegPagos.IsVisible = true;
                        svMsgNoTarj.IsVisible = false;
                    }
                }
                if (rvm.Tiporecarga == TipoRecarga.Monedero)
                {
                    //if (grptipos[0].Count() == 0)
                    //{
                    //    slMonedero.IsVisible = false;
                    //    lvRegPagos.IsVisible = false;
                    //    svMsgNoTarj.IsVisible = true;
                    //}
                    //else
                    //{
                        slMonedero.IsVisible = true;
                        lvRegPagos.IsVisible = true;
                        svMsgNoTarj.IsVisible = false;
                    //}
                }
            }
            catch(Exception e)
            {

            }
		}

		public async void OnDelete(object sender, EventArgs e)
		{
			var mi = ((Xamarin.Forms.MenuItem)sender);
			TipoPago del = (mi.CommandParameter as TipoPago);
			if (del.idtipo == 2)
			{
				bool ret = await DisplayAlert("Confirme", "¿Desea eliminar esta tarjeta?", "Si", "No");
				if (ret)
				{
					tvm.TarjetaBaja += async (s, ea) =>
					{
						//Device.BeginInvokeOnMainThread(async () =>
						//{
							UserDialogs.Instance.HideLoading();
							if (ea.Error != null || ea.Cancelled)
								await DisplayAlert("Error", "Ocurrio un error, vuelva a intentar", "OK");
						//});
					};
					UserDialogs.Instance.ShowLoading("Eliminando...");
					tvm.Baja(del.extra);
				}
			}
		}

		public void NuevaT()
        {
        }

        protected override void OnAppearing()
        {
			string json = "";
			json = App.WSc.GetCatalogo(9, "where idusuario = " + App.usr.idusuario.ToString());
			List<Tarjeta> lt = JsonConvert.DeserializeObject<List<tempuri.org.Tarjeta>>(json).Select(x => new Tarjeta { idtarjeta = x.idtarjeta, idpais = x.idpais, idemisor = x.idemisor, idusuario = x.idusuario, Last4 = x.Last4 }).ToList();
			App.db.DescargaTarjetas(lt);
			json = "";
			json = App.WSc.GetCatalogo(12, "where idusuario = " + App.usr.idusuario.ToString());
			List<SaldoMonedero> sm = JsonConvert.DeserializeObject<List<SaldoMonedero>>(json);
			if (sm.Count() > 0)
				App.db.DescargaSaldo(sm.FirstOrDefault());
			
			tvm.RefreshMedios();
			if (rvm.Tiporecarga == TipoRecarga.Monedero)
			{
				gTarjetas = grptipos[0];
				gTarjetas = new Grupo("Tus tarjetas", 2, tvm.lt);
				grptipos[0] = gTarjetas;
			}
			if (rvm.Tiporecarga == TipoRecarga.RecargaTA)
			{
				gMonedero = grptipos[0];
				gMonedero = new Grupo("Tu monedero", 1, tvm.lm);
				grptipos[0] = gMonedero;
				gTarjetas = grptipos[1];
				gTarjetas = new Grupo("Tus tarjetas", 2, tvm.lt);
				grptipos[1] = gTarjetas;
			}
            lvRegPagos.BeginRefresh();
            lvRegPagos.ItemsSource = null;
            lvRegPagos.ItemsSource = grptipos;
            lvRegPagos.EndRefresh();
			if (rvm.Tiporecarga == TipoRecarga.RecargaTA)
			{
				if ((grptipos[0].FirstOrDefault().saldo == 0M) && (grptipos[1].Count() == 0))
				{
					slMonedero.IsVisible = false;
					lvRegPagos.IsVisible = false;
					svMsgNoTarj.IsVisible = true;
				}
				else
				{
					slMonedero.IsVisible = false;
					lvRegPagos.IsVisible = true;
					svMsgNoTarj.IsVisible = false;
				}
			}
			if (rvm.Tiporecarga == TipoRecarga.Monedero)
			{
				slMonedero.IsVisible = true;
				lvRegPagos.IsVisible = true;
				svMsgNoTarj.IsVisible = false;
			}
			if (rvm.Tiporecarga == TipoRecarga.Monedero)
			{
				//if (rvm == null || (rvm != null && rvm.MontoRecargaMonedero == 0M))
					lblSaldo.Text = "Saldo: " + tvm.lm[0].saldo.ToString("c", new System.Globalization.CultureInfo("es-MX"));
				//else
				//	lblSaldo.Text = "Saldo: " + rvm.MontoRecargaMonedero.ToString("c");
			}

			if (App.usr.idusuario != 0)
			{
				if (tvm != null)
					tvm.Idusuario = App.usr.idusuario;
				if (rvm != null)
					rvm.IdUsuario = App.usr.idusuario;
			}

			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("es-MX");

			base.OnAppearing();
        }
    }

    public class MuestraSaldoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool ret = false;
            var tipo = (int)value;
            if (tipo == 1)
                ret = true;
            if (tipo == 2)
                ret = false;
            return (ret);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MuestraTarjetaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool ret = false;
            var tipo = (int)value;
            if (tipo == 1)
                ret = false;
            if (tipo == 2)
                ret = true;
            return (ret);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Grupo : ObservableCollection<TipoPago>
    {
        public String Name { get; private set; }
        public int Tipo { get; private set; }

        public Grupo(String Name, int tipo, IEnumerable<TipoPago> col)
        {
            this.ClearItems();
            foreach (TipoPago tmp in this.Items.Union(col).ToList())
                this.Add(tmp);
            this.Name = Name;
			this.Tipo = tipo;
        }

        public Grupo(IEnumerable<TipoPago> col)
        {
            this.ClearItems();
            foreach (TipoPago tmp in this.Items.Union(col).ToList())
                this.Add(tmp);
        }
    }

    public class TipoPago
    {
        public int idtipo { get; set; }
        public string tipo { get; set; }
        public decimal saldo { get; set; }
        public string imgtipo { get; set; }
		public int extra { get; set; } //en tarjeta idtarjeta
    }
}
