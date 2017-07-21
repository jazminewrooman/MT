using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Globalization;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using Acr.UserDialogs;
using Rg.Plugins.Popup.Extensions;

namespace MasTicket
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CargarSaldo : ContentPage
    {
        ObservableCollection<Opcion> ls = new ObservableCollection<Opcion>();
        List<Contacto> lsC = new List<Contacto>();
        RecargasViewModel rvm;
        Recarga mirec;
        double _imageHeight;
		Contactos vwContactos;

        private void Refresh()
        {
			rvm.idpais = 0;
            //txtCupon.Text = "";
            txtNum.Text = "";
        }

		protected override void OnAppearing()
		{
			if (rvm.EsIncompleta())
				btnAplicar.IsEnabled = false;
			else
				btnAplicar.IsEnabled = true;

			if (App.usr.idusuario != 0)
			{
				rvm.IdUsuario = App.usr.idusuario;
			}

			if (lsC.Count == 0 && vwContactos != null)
				lsC = vwContactos.GetLista();

			base.OnAppearing();
		}

		public void CargaDeAgenda(Recarga rec)
		{
			if (rec != null)
				mirec = rec;

			if (mirec.idpais != 0)
				rvm.idpais = mirec.idpais;
			if (mirec.idoperadora != 0)
				rvm.idoperadora = mirec.idoperadora;
			if (mirec.idpaquete != 0)
				rvm.idpaquete = mirec.idpaquete;
			rvm.NumeroRecarga = mirec.numerorecarga;
			rvm.ContactoRecarga = (!String.IsNullOrEmpty(mirec.contactorecarga) ? CleanString.UseRegex(mirec.contactorecarga) : "");
			rvm.idFormaPago = mirec.idformapago;
			rvm.IdTarjeta = mirec.idtarjeta;

			if (mirec != null)
			{
				txtNum.Text = mirec.numerorecarga;
				lblContacto.Text = (!String.IsNullOrEmpty(mirec.contactorecarga) ? CleanString.UseRegex(mirec.contactorecarga) : "");
			}
			if (rvm.idoperadora == 0 || rvm.idpaquete == 0)
				lblvalOpePaq.IsVisible = true;
			else
				lblvalOpePaq.IsVisible = false;
		}

		public CargarSaldo()
		{
			CargarInicia(null);
		}

		public CargarSaldo(Recarga rec)
		{
			CargarInicia(rec);
		}

		public void CargarInicia(Recarga rec)
        {
            InitializeComponent();
			Title = "Cargar Saldo";

			rvm = new RecargasViewModel();
            this.BindingContext = rvm;
			rvm.Tiporecarga = TipoRecarga.RecargaTA;
			NavigationPage.SetBackButtonTitle(this, "");

			if (rec != null)
			{
				if (rec is RecargaProg)
					rvm.EdicionOnly = true;

				if (App.usr.idpais != 0)
					rvm.idpais = App.usr.idpais;
				else
					rvm.idpais = 1; //default Mexico
				rvm.IdUsuario = App.usr.idusuario;

				mirec = new Recarga();
				mirec = rec;
			}
			else {
				Refresh();
				if (App.usr.idpais != 0)
					rvm.idpais = App.usr.idpais;
				else
					rvm.idpais = 1; //default Mexico
			}

            ToolbarItems.Add(new ToolbarItem("Ayuda", "ayuda2.png", () =>
            {
                var page = new InfoAyuda("cargarsaldo.html");
                Navigation.PushPopupAsync(page);
            }));

            lsC.Clear();

			rvm.RecargaIncompleta += (s, e) =>
			{
				if (e.estaincompleta)
				{
					if (rvm.idoperadora == 0 || rvm.idpaquete == 0)
						lblvalOpePaq.IsVisible = true;
					else
						lblvalOpePaq.IsVisible = false;
					btnAplicar.IsEnabled = false;
				}
				else {
					btnAplicar.IsEnabled = true;
				}
			};
			if (mirec != null)
			{
				rvm.idpais = mirec.idpais;
				rvm.idoperadora = mirec.idoperadora;
				rvm.idpaquete = mirec.idpaquete;
				rvm.NumeroRecarga = mirec.numerorecarga;
				rvm.ContactoRecarga = (!String.IsNullOrEmpty(mirec.contactorecarga) ? CleanString.UseRegex(mirec.contactorecarga) : "");
				rvm.idFormaPago = mirec.idformapago;
				rvm.IdTarjeta = mirec.idtarjeta;
				if (rvm.idFormaPago == 2 && rvm.IdTarjeta > 0)
					rvm.TipoTrans = TipoTransaccion.SegundaVez;
			}
			if (rvm.EdicionOnly)
				btnAplicar.Text = "Comprar";
			else {
				if (rvm.idFormaPago == 2 && rvm.IdTarjeta > 0 && App.db.SelTarjetas(rvm.IdTarjeta) != null)
					btnAplicar.Text = "Confirmar";
				else
					btnAplicar.Text = "Comprar";
			}

			string title = ""; ListaOpciones lo;
			TapGestureRecognizer tapgrdPais = new TapGestureRecognizer();
			tapgrdPais.Tapped += async (s, e) =>
			//vclPais.Tapped += async (object sender, EventArgs e) =>
            {
				grdPais.BackgroundColor = Color.FromHex("#e5e5e5"); //Color.FromHex((App.Current.Resources["ButtonRojo"] as Style).Setters.Where(x => x.Property == BackgroundColorProperty).FirstOrDefault().Value.ToString()); //.Opacity = .5;
				await Task.Delay(100);
				grdPais.BackgroundColor = Color.Transparent; //grdEstado.Opacity = 1;
				ls.Clear();
                title = "Selecciona un pais";
                ls = rvm.lsPais().Select(x => new Opcion { idopc = x.idpais, opc = x.pais, imgopc = x.img }).ToObservableCollection();
                lo = new ListaOpciones(ls, title, 1);
                lo.IdOpc = 1;
                await App.Nav.PushAsync(lo, Constantes.animated);
            };
			grdPais.GestureRecognizers.Add(tapgrdPais);

			TapGestureRecognizer tapgrdOperadora = new TapGestureRecognizer();
			tapgrdOperadora.Tapped += async (s, e) =>
			//vclOperadora.Tapped += async (object sender, EventArgs e) =>
            {
				grdOperadora.BackgroundColor = Color.FromHex("#e5e5e5"); //Color.FromHex((App.Current.Resources["ButtonRojo"] as Style).Setters.Where(x => x.Property == BackgroundColorProperty).FirstOrDefault().Value.ToString()); //.Opacity = .5;
				await Task.Delay(100);
				grdOperadora.BackgroundColor = Color.Transparent; //grdEstado.Opacity = 1;
				ls.Clear();
                title = "Selecciona una compañía";
				ls = rvm.LsOperadoras(rvm.idpais).Select(x => new Opcion { idopc = x.idoperadora, opc = x.operadora, imgopc = x.img }).ToObservableCollection();
                lo = new ListaOpciones(ls, title, 1);
                lo.IdOpc = 2;
                await App.Nav.PushAsync(lo, Constantes.animated);
            };
			grdOperadora.GestureRecognizers.Add(tapgrdOperadora);

			if (mirec != null)
			{
				txtNum.Text = mirec.numerorecarga;
				lblContacto.Text = (!String.IsNullOrEmpty(mirec.contactorecarga) ? CleanString.UseRegex(mirec.contactorecarga) : "");
			}
			TapGestureRecognizer tapgrdSaldo = new TapGestureRecognizer();
			tapgrdSaldo.Tapped += async (s, e) =>
			//vclSaldo.Tapped += async (object sender, EventArgs e) =>
            {
				grdSaldo.BackgroundColor = Color.FromHex("#e5e5e5"); //Color.FromHex((App.Current.Resources["ButtonRojo"] as Style).Setters.Where(x => x.Property == BackgroundColorProperty).FirstOrDefault().Value.ToString()); //.Opacity = .5;
				await Task.Delay(100);
				grdSaldo.BackgroundColor = Color.Transparent; //grdEstado.Opacity = 1;
				ls.Clear();
                title = "Selecciona monto";
				ObservableCollection<catPaquete> lpq = rvm.LsPaquetes(rvm.idoperadora);
				ls = lpq.OrderBy(x => x.tipo).Select(x => new Opcion { idopc = x.idpaquete, opc = x.monto.ToString("c", new System.Globalization.CultureInfo("es-MX")) + " " + x.paquete, idagrupador = x.tipo, agrupador = (x.tipo == 0 ? "Tiempo aire" : "Paquetes de datos")  }).ToObservableCollection();
				if (ls.Select(x => x.idagrupador).Distinct().Count() > 1)
					lo = new ListaOpciones(ls, title, 1, true); //hay TA y datos
				else
					lo = new ListaOpciones(ls, title, 1); //solo TA
                lo.IdOpc = 4;
                await App.Nav.PushAsync(lo, Constantes.animated);
            };
			grdSaldo.GestureRecognizers.Add(tapgrdSaldo);

			TapGestureRecognizer tapgrdLimpiar = new TapGestureRecognizer();
			tapgrdLimpiar.Tapped += async (s, e) =>
			{
				grdLimpiar.BackgroundColor = Color.FromHex("#e5e5e5");
				await Task.Delay(100);
				grdLimpiar.BackgroundColor = Color.Transparent;
				rvm.Refresh();
				txtNum.Text = "";
				lblContacto.Text = "";
			};
			grdLimpiar.GestureRecognizers.Add(tapgrdLimpiar);

			btnAplicar.Clicked += async (sender, ea) =>
			{
				if (rvm.EsIncompleta())
					await DisplayAlert("Error", "Faltan capturar o seleccionar algunos campos", "OK");
				else {
					btnAplicar.IsEnabled = false;
					rvm.NumeroRecarga = check.SafeSqlLiteral(txtNum.Text.Trim());
					if (rvm.EdicionOnly)
						await App.Nav.PushAsync(new RegPago(rvm), Constantes.animated);
					else {
						if (rvm.idFormaPago == 2 && rvm.IdTarjeta > 0 && App.db.SelTarjetas(rvm.IdTarjeta) != null)
						{
							//vamos a cargar el tvm desde una tarjeta (no se como?)
							TarjetasViewModel tvm = new TarjetasViewModel();
							await App.Nav.PushAsync(new Procesar(rvm, tvm, TipoTransaccion.SegundaVez), Constantes.animated);
						}
						else
							await App.Nav.PushAsync(new RegPago(rvm), Constantes.animated);
					}
				}
			};
			phvalNum.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
			{
				if (phvalNum.IsValid)
				{
					rvm.NumeroRecarga = check.SafeSqlLiteral(txtNum.Text);
				}
				else {
					rvm.NumeroRecarga = "";
				}
			};
			btnContactos.Clicked += async (sender, e) =>
			{
				vwContactos = new Contactos();
				await App.Nav.PushAsync(vwContactos);
				lsC = vwContactos.GetLista();
			};
			txtNum.TextChanged += (s, e) =>
			{
				if (lsC.Count > 0)
				{
					IEnumerable<Contacto> tmp = lsC.Where(x => Regex.Replace(x.Number, @"\s+", "").Contains(e.NewTextValue));
					if (tmp.Count() > 0)
					{
						lblContacto.Text = CleanString.UseRegex(tmp.FirstOrDefault().Name);
						rvm.ContactoRecarga = lblContacto.Text;
					}
					else {
						lblContacto.Text = "";
						rvm.ContactoRecarga = "";
					}
				}
			};
        }

        public void SetVal(int idpos, Opcion opc)
        {
            if (idpos == 1) //pais
            {
                rvm.idpais = opc.idopc;
                //mirec.idpais = opc.idopc;
                //mirec.idoperadora = 0;
                //mirec.idpaquete = 0;
            }
            if (idpos == 2) //operadora
            {
				rvm.idoperadora = opc.idopc;
    			//mirec.idoperadora = opc.idopc;
				//mirec.idpaquete = 0;
			}
            if (idpos == 4) //monto o paquete
            {
				rvm.idpaquete = opc.idopc;
				//mirec.idpaquete = opc.idopc;
			}
			if (rvm.idoperadora == 0 || rvm.idpaquete == 0)
				lblvalOpePaq.IsVisible = true;
			else
				lblvalOpePaq.IsVisible = false;
		}
    }

    public class TipoSelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int tipo = (int)value;
            bool ret = false;

            switch (tipo)
            {
                case 1:
                    ret = true;
                    break;
                case 2:
                    ret = false;
                    break;
                default:
                    ret = false;
                    break;
            }

            return ret;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }

    public class TipoTxtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int tipo = (int)value;
            bool ret = false;

            switch (tipo)
            {
                case 1:
                    ret = false;
                    break;
                case 2:
                    ret = true;
                    break;
                default:
                    ret = false;
                    break;
            }

            return ret;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
}
