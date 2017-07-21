using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using CreditCardValidator;
using Rg.Plugins.Popup.Extensions;
using Acr.UserDialogs;

namespace MasTicket
{
	public enum TipoAnadidor
	{
		RegistroPago = 1,
		Monedero = 2
	};

	public partial class NuevaTarjeta : ContentPage
	{
		ObservableCollection<Opcion> ls = new ObservableCollection<Opcion>();
		TarjetasViewModel tvm;
		RecargasViewModel rvm;
		public event EventHandler<NuevaTarjetaEventArgs> NuevaTarj;

		protected override void OnAppearing()
		{
			if (tvm == null)
				btnGuardar.IsEnabled = false;
			else {
				if (tvm.EsIncompleta())
					btnGuardar.IsEnabled = false;
				else
					btnGuardar.IsEnabled = true;
			}
			base.OnAppearing();
		}

		protected override bool OnBackButtonPressed()
		{
			return base.OnBackButtonPressed();
		}

		private void Refresh()
		{
			txtNum.Completed += (s, e) => txtMes.Focus();
			txtMes.Completed += (s, e) => txtAno.Focus();
			txtAno.Completed += (s, e) => txtCvc.Focus();
			txtCvc.Completed += (s, e) => txtTitularFN.Focus();
			txtTitularFN.Completed += (s, e) => txtTitularLN.Focus();
			txtTitularLN.Completed += (s, e) => txtCalleNum.Focus();
			txtCalleNum.Completed += (s, e) => txtCP.Focus();

			tvm.idpais = 0;
			tvm.idemisor = 0;
			tvm.NumeroTarjeta = ""; //txtNum.Text = "";
			tvm.ExpiraMM = ""; //txtMes.Text = "";
			tvm.ExpiraYY = ""; //txtAno.Text = "";
			tvm.Cvc = ""; //txtCvc.Text = "";
			tvm.TitularFN = ""; //txtTitularFN.Text = ""; 
			tvm.TitularLN = ""; //txtTitularLN.Text = "";
			tvm.CalleyNum = ""; //txtCalleNum.Text = "";
			tvm.IdEstado = 0;
			tvm.IdCiudad = 0;
			tvm.CP = ""; //txtCP.Text = "";
			tvm.ExpirationMMYY = "";
		}

		public NuevaTarjeta(TipoAnadidor tipo, TarjetasViewModel _t, RecargasViewModel _r)
		{
			try
			{
				InitializeComponent();
				Title = "Nueva tarjeta";
				ToolbarItems.Add(new ToolbarItem("Ayuda", "ayuda2.png", () =>
				{
					var page = new InfoAyuda("nuevat.html");
					Navigation.PushPopupAsync(page);
				}));
#if __IOS__
				grdScanner.IsVisible = false;
#endif

				tvm = _t;
				rvm = _r;
				this.BindingContext = tvm;
				Refresh();
				tvm.Idusuario = App.usr.idusuario;
				if (App.usr.idpais != 0)
					tvm.idpais = App.usr.idpais;
				txtNum.Completed += (s, e) => txtMes.Focus();
				txtMes.Completed += (s, e) => txtAno.Focus();
				txtAno.Completed += (s, e) => txtCvc.Focus();
				txtCvc.Completed += (s, e) => txtTitularFN.Focus();
				txtTitularFN.Completed += (s, e) => txtTitularLN.Focus();
				txtTitularLN.Completed += (s, e) => txtCalleNum.Focus();
				txtCalleNum.Completed += (s, e) => txtCP.Focus();

				tvm.TarjetaIncompleta += (s, e) =>
				{
					if (e.estaincompleta)
					{
						if (tvm.idpais == 0 || tvm.IdEstado == 0 || tvm.IdCiudad == 0)
							lblvalGeo.IsVisible = true;
						else
							lblvalGeo.IsVisible = false;
						btnGuardar.IsEnabled = false;
					}
					else {
						lblvalGeo.IsVisible = false;
						btnGuardar.IsEnabled = true;
					}
				};

				txtNum.TextChanged += (sender, e) =>
				{
					if (!String.IsNullOrEmpty(txtNum.Text))
					{
						if (cardval.IsValid)
						{
							CreditCardDetector det = new CreditCardDetector(txtNum.Text);
							tvm.idemisor = SetEmisor(det.Brand);
							tvm.NumeroTarjeta = txtNum.Text;
							lblvalCard.IsVisible = false; //.Text = "";
						}
						else {
							CreditCardDetector det = new CreditCardDetector(txtNum.Text);
							tvm.idemisor = SetEmisor(det.Brand);
							tvm.NumeroTarjeta = "";
							lblvalCard.IsVisible = true; //.Text = "El numero de tarjeta es obligatorio, y tal cual aparece en la tarjeta";
						}
					}
				};
				txtMes.TextChanged += Valfecha;
				txtAno.TextChanged += Valfecha;
				txtCvc.TextChanged += Valfecha;
				txtTitularFN.TextChanged += (s, e) =>
				{
					if (valTitulFN.IsValid)
					{
						lblvalTitular.IsVisible = false; //.Text = "";
						tvm.TitularFN = CleanString.UseRegex(txtTitularFN.Text);
					}
					else
					{
						lblvalTitular.IsVisible = true; //.Text = "El nombre del titular es obligatorio y tal cual aparece en la tarjeta";
						tvm.TitularFN = "";
					}
				};
				txtTitularLN.TextChanged += (s, e) =>
				{
					if (valTitulLN.IsValid)
					{
						lblvalTitular.IsVisible = false; //.Text = "";
						tvm.TitularLN = CleanString.UseRegex(txtTitularLN.Text);
					}
					else
					{
						lblvalTitular.IsVisible = true; //.Text = "El nombre del titular es obligatorio y tal cual aparece en la tarjeta";
						tvm.TitularLN = "";
					}
				};
				txtCalleNum.TextChanged += (s, e) =>
				{
					if (valCalle.IsValid)
					{
						lblvalCalle.IsVisible = false; //.Text = "";
						tvm.CalleyNum = CleanString.UseRegex(txtCalleNum.Text);
					}
					else
					{
						lblvalCalle.IsVisible = true; //.Text = "El nombre del titular es obligatorio y tal cual aparece en la tarjeta";
						tvm.CalleyNum = "";
					}
				};
				txtCP.TextChanged += (s, e) =>
				{
					if (cpval.IsValid)
					{
						lblvalCP.IsVisible = false; //.Text = "";
						tvm.CP = check.SafeSqlLiteral(txtCP.Text);
					}
					else
					{
						lblvalCP.IsVisible = true; //.Text = "El nombre del titular es obligatorio y tal cual aparece en la tarjeta";
						tvm.CP = "";
					}
				};

				string title = ""; ListaOpciones lo;
				//grdPais.Tapped += async (object sender, EventArgs e) =>
				TapGestureRecognizer tapgrdPais = new TapGestureRecognizer();
				tapgrdPais.Tapped += async (s, e) =>
				{
					grdPais.BackgroundColor = Color.FromHex("#e5e5e5"); //Color.FromHex((App.Current.Resources["ButtonRojo"] as Style).Setters.Where(x => x.Property == BackgroundColorProperty).FirstOrDefault().Value.ToString()); //.Opacity = .5;
					await Task.Delay(100);
					grdPais.BackgroundColor = Color.Transparent; //grdEstado.Opacity = 1;
					ls.Clear();
					title = "Seleccione un país";
					ls = tvm.lsPais().Select(x => new Opcion { idopc = x.idpais, opc = x.pais, imgopc = x.img }).ToObservableCollection();
					lo = new ListaOpciones(ls, title, 2);
					lo.IdOpc = 1;
					await App.Nav.PushAsync(lo, Constantes.animated);
				};
				grdPais.GestureRecognizers.Add(tapgrdPais);

				TapGestureRecognizer tapgrdTipo = new TapGestureRecognizer();
				tapgrdTipo.Tapped += async (s, e) =>
				//grdTipo.Tapped += async (object sender, EventArgs e) =>
				{
					grdTipo.BackgroundColor = Color.FromHex("#e5e5e5"); //Color.FromHex((App.Current.Resources["ButtonRojo"] as Style).Setters.Where(x => x.Property == BackgroundColorProperty).FirstOrDefault().Value.ToString()); //.Opacity = .5;
					await Task.Delay(100);
					grdTipo.BackgroundColor = Color.Transparent; //grdEstado.Opacity = 1;
					ls.Clear();
					title = "Seleccione emisor";
					ls = tvm.LsEmisores().Select(x => new Opcion { idopc = x.idemisor, opc = x.emisor, imgopc = x.img }).ToObservableCollection();
					lo = new ListaOpciones(ls, title, 2);
					lo.IdOpc = 2;
					await App.Nav.PushAsync(lo, Constantes.animated);
				};
				grdTipo.GestureRecognizers.Add(tapgrdTipo);

				TapGestureRecognizer tapgrdScanner = new TapGestureRecognizer();
				tapgrdScanner.Tapped += async (s, e) =>
				//vclScanner.Tapped += (s, e) =>
				{
					grdScanner.BackgroundColor = Color.FromHex("#e5e5e5"); //Color.FromHex((App.Current.Resources["ButtonRojo"] as Style).Setters.Where(x => x.Property == BackgroundColorProperty).FirstOrDefault().Value.ToString()); //.Opacity = .5;
					await Task.Delay(100);
					grdScanner.BackgroundColor = Color.Transparent; //grdEstado.Opacity = 1;
#if __ANDROID__
					var plataforma = DependencyService.Get<ICardReader>();
					if (plataforma != null)
					{
						plataforma.CardFound = delegate (string FormattedCardNumber, int ExpiryMonth, int ExpiryYear, string CardholderName, Card.IO.CardType CardType)
						{
							txtNum.Text = FormattedCardNumber;
							//txtTitular.Text = CardholderName;
							txtMes.Text = (ExpiryMonth > 0 ? ExpiryMonth.ToString() : "");
							if (ExpiryYear > 0)
							{
								if (ExpiryYear.ToString().Length > 2)
									txtAno.Text = ExpiryYear.ToString().Substring(2, 2);
								else
									txtAno.Text = ExpiryYear.ToString();
							}
							else
								txtAno.Text = "";
						};
						plataforma.ReadCard();
					}
#endif
#if __IOS__
					//var plataforma = DependencyService.Get<ICardReader>();
					//if (plataforma != null)
					//{
					//	plataforma.CardFound = delegate (string FormattedCardNumber, int ExpiryMonth, int ExpiryYear, string CardholderName, Card.IO.CreditCardType CardType)
					//	{
					//		txtNum.Text = FormattedCardNumber;
					//		//txtTitular.Text = CardholderName;
					//		txtMes.Text = (ExpiryMonth > 0 ? ExpiryMonth.ToString() : "");
					//		if (ExpiryYear > 0)
					//		{
					//			if (ExpiryYear.ToString().Length > 2)
					//				txtAno.Text = ExpiryYear.ToString().Substring(2, 2);
					//			else
					//				txtAno.Text = ExpiryYear.ToString();
					//		}
					//		else
					//			txtAno.Text = "";
					//	};
					//	plataforma.ReadCard();
					//}
#endif
				};
				grdScanner.GestureRecognizers.Add(tapgrdScanner);

				TapGestureRecognizer tapgrdEstado = new TapGestureRecognizer();
				tapgrdEstado.Tapped += async (s, e) =>
				//grdEstado.Tapped += async (object sender, EventArgs e) =>
				{
					grdEstado.BackgroundColor = Color.FromHex("#e5e5e5"); //Color.FromHex((App.Current.Resources["ButtonRojo"] as Style).Setters.Where(x => x.Property == BackgroundColorProperty).FirstOrDefault().Value.ToString()); //.Opacity = .5;
					await Task.Delay(100);
					grdEstado.BackgroundColor = Color.Transparent; //grdEstado.Opacity = 1;
					ls.Clear();
					title = "Seleccione estado";
					ls = tvm.lsEstados(tvm.idpais).Select(x => new Opcion { idopc = x.idestado, opc = x.estado, }).ToObservableCollection();
					lo = new ListaOpciones(ls, title, 2);
					lo.IdOpc = 3;
					await App.Nav.PushAsync(lo, Constantes.animated);
				};
				grdEstado.GestureRecognizers.Add(tapgrdEstado);

				TapGestureRecognizer tapgrdCiudad = new TapGestureRecognizer();
				tapgrdCiudad.Tapped += async (s, e) =>
				//btnCiudad.Clicked += async (s,e) =>
				//vclCiudad.Tapped += async (s, e) =>
				{
					grdCiudad.BackgroundColor = Color.FromHex("#e5e5e5");
					await Task.Delay(100);
					grdCiudad.BackgroundColor = Color.Transparent; //grdEstado.Opacity = 1;
					ls.Clear();
					title = "Seleccione ciudad";
					ls = tvm.lsMunicipios(tvm.IdEstado).OrderBy(x => x.municipio).Select(x => new Opcion { idopc = x.idmunicipio, opc = x.municipio, }).ToObservableCollection();
					lo = new ListaOpciones(ls, title, 2);
					lo.IdOpc = 4;
					await App.Nav.PushAsync(lo, Constantes.animated);
				};
				grdCiudad.GestureRecognizers.Add(tapgrdCiudad);

				btnGuardar.Clicked += async (sender, e) =>
				{
					if (tvm.EsIncompleta())
						await DisplayAlert("Error", "Faltan capturar o seleccionar algunos campos", "OK");
					else {
						if (txtAno.Text.Trim().Length < 2 || txtMes.Text.Trim().Length < 2)
							await DisplayAlert("Error", "Revise mes y año de expiración", "OK");
						else {
							int mm = int.Parse(txtMes.Text.Trim());
							int yy = int.Parse(txtAno.Text.Trim());
							if (mm < 1 || mm > 12 || yy < DateTime.Now.Year - 2000)
							{
								await DisplayAlert("Error", "Revise mes y año de expiración", "OK");
							}
							else {
								if (App.usr == null || (App.usr != null && (App.usr.email == null || App.usr.idusuario == 0)))
								{
									var login = new Login(App.Current);
									await App.Nav.PushAsync(login, Constantes.animated);
								}
								else {
									btnGuardar.IsEnabled = false;
									//tvm.TarjetaAlta += async (s, ea) =>
									//{
									//	UserDialogs.Instance.HideLoading();
									//	if (ea.Error != null || ea.Cancelled)
									//		await DisplayAlert("Error", "Ocurrio un error, vuelva a intentar", "OK");
									//	else
									//		await App.Nav.PopAsync(Constantes.animated);
									//};
									//UserDialogs.Instance.ShowLoading("Guardando...");
									//tvm.Alta();
									rvm.idFormaPago = 2; //tarjeta
									rvm.TipoTrans = TipoTransaccion.PrimeraVez;
									rvm.IdTarjeta = 0;
									Procesar pro = App.Nav.NavigationStack.OfType<Procesar>().FirstOrDefault();
									if (pro == null)
										await App.Nav.PushAsync(new Procesar(rvm, tvm, TipoTransaccion.PrimeraVez), Constantes.animated);
									else {
										pro.Refresh(rvm, tvm, TipoTransaccion.PrimeraVez);
										await App.Nav.PopAsync(Constantes.animated); //boto nueva tarjeta (this)
										await App.Nav.PopAsync(Constantes.animated); //boto medios pago
									}
								}
							}
						}
					}
				};
				tvm.ChecaIncompleta();
			}
			catch (Exception e)
			{

			}
		}

		public async void AltaEnPrimeraVez()
		{
			tvm.Idusuario = App.usr.idusuario;
			rvm.IdUsuario = App.usr.idusuario;
			btnGuardar.IsEnabled = false;
			rvm.idFormaPago = 2; //tarjeta
			rvm.TipoTrans = TipoTransaccion.PrimeraVez;
			rvm.IdTarjeta = 0;
			Procesar pro = App.Nav.NavigationStack.OfType<Procesar>().FirstOrDefault();
			if (pro == null)
				await App.Nav.PushAsync(new Procesar(rvm, tvm, TipoTransaccion.PrimeraVez), Constantes.animated);
			else {
				pro.Refresh(rvm, tvm, TipoTransaccion.PrimeraVez);
				await App.Nav.PopAsync(Constantes.animated); //boto nueva tarjeta (this)
				await App.Nav.PopAsync(Constantes.animated); //boto medios pago
			}
			//tvm.TarjetaAlta += async (s, ea) =>
			//{
			//	UserDialogs.Instance.HideLoading();
			//	if (ea.Error != null || ea.Cancelled)
			//		await DisplayAlert("Error", "Ocurrio un error, vuelva a intentar", "OK");
			//	else {
			//		await App.Nav.PopAsync(Constantes.animated);
			//	}
			//};
			//UserDialogs.Instance.ShowLoading("Guardando...");
			//tvm.Alta();
		}

		private void Valfecha(object sender, TextChangedEventArgs e)
        {
            if (cardmesval.IsValid && cardyyval.IsValid && cardcvcval.IsValid)
            {
				lblvalMes.IsVisible = false; //.Text = "";// = false;
				tvm.ExpiraMM = check.SafeSqlLiteral(txtMes.Text.Trim());
				tvm.ExpiraYY = check.SafeSqlLiteral(txtAno.Text.Trim());
				tvm.Cvc = check.SafeSqlLiteral(txtCvc.Text.Trim());
				tvm.ExpirationMMYY = check.SafeSqlLiteral(txtMes.Text.Trim()) + check.SafeSqlLiteral(txtAno.Text.Trim());
            }
            else
            {
				lblvalMes.IsVisible = true; //.Text = "Revise Mes y Año de validez, asi como el CVC"; //.IsVisible = true;
				if (!cardmesval.IsValid)
				{
					tvm.ExpiraMM = "";
					tvm.ExpirationMMYY = "";
				}
				if (!cardyyval.IsValid)
				{
					tvm.ExpiraYY = "";
					tvm.ExpirationMMYY = "";
				}
                if (!cardcvcval.IsValid)
                    tvm.Cvc = "";
            }
        }

        private int SetEmisor(CardIssuer tipo)
        {
            int localtipo = 0;
            switch (tipo)
            {
                case CardIssuer.AmericanExpress:
                    localtipo = 3; break;
                case CardIssuer.DinersClub:
                    localtipo = 7; break;
                case CardIssuer.Discover:
                    localtipo = 6; break;
                //case CardIssuer.JCB:
                //    Cambia(9); break;
                //case CardIssuer.Maestro:
                //    Cambia(5); break;
                case CardIssuer.MasterCard:
                    localtipo = 5; break;
                case CardIssuer.Unknown:
                    localtipo = 98; break;
                case CardIssuer.Visa:
                    localtipo = 4; break;
                default:
                    localtipo = 98; break;
            }
            return (localtipo);
        }
    
        public void SetVal(int idpos, Opcion opc)
        {
            if (idpos == 1) //pais
            {
                tvm.idpais = opc.idopc;
            }
            if (idpos == 2) //emisor
            {
                tvm.idemisor = opc.idopc;
            }
            if (idpos == 3) //estado
            {
                tvm.IdEstado = opc.idopc;
            }
            if (idpos == 4) //ciudad
            {
                tvm.IdCiudad = opc.idopc;
            }
        }
    }
}
