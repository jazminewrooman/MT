using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Globalization;

using Xamarin.Forms;
using Acr.UserDialogs;
using Splat;
using Plugin.Toasts;
using DeviceOrientation.Forms.Plugin.Abstractions;
using Rg.Plugins.Popup.Extensions;

namespace MasTicket
{
    public enum TipoListaOpciones
    {
        pais = 1,
        operadora = 2,
        monto = 4
    }
    public enum Estado
    {
        Recarga,
        NuevaTarjeta
    }
    public enum PasosRecarga
    {
        EnDesorden = 0,
        Numero = 1,
        Pais = 2,
        Operadora = 3,
        Monto = 4,
        MedioPago = 5,
        Recarga = 6
    }
    public enum Listas
    {
        contactos,
        opciones,
        pagos,
		listo,
        nuevacard
    }

    public partial class CargarSaldo2 : ContentPage
    {
		private double width;
        private double height;
        bool muestrainfo = true;
        PasosRecarga paso; Estado status;
        List<letragrupo> Groups = new List<letragrupo>();
        ObservableCollection<Opcion> ls = new ObservableCollection<Opcion>();
        List<opcMenu> opciones;
        List<Contacto> lsC = new List<Contacto>();
        IToastNotificator notif;
        ObservableCollection<Grupo> grptipos;
        List<TipoPago> lt = new List<TipoPago>();
        List<TipoPago> lm = new List<TipoPago>();
        Grupo gMonedero; Grupo gTarjetas;

		protected override void OnAppearing()
		{
			try
			{
				Iniciar();

				base.OnAppearing();
			}
			catch (Exception ex)
			{
			}
		}

        public void Recompensar()
        {
            status = Estado.Recarga;
            paso = PasosRecarga.MedioPago;

        }

		protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width != this.width || height != this.height)
            {
                this.width = width;
                this.height = height;
                IDeviceOrientation _deviceOrientationSvc = DependencyService.Get<IDeviceOrientation>();
                DeviceOrientations dvcor = _deviceOrientationSvc.GetOrientation();
                if (dvcor == DeviceOrientations.Landscape) 
                //if (width > height) //apaisado
                {
                    grdMain.ColumnDefinitions.Clear();
                    grdMain.RowDefinitions.Clear();
                    Enumerable.Range(1, 3).ToList().ForEach(x =>
                        grdMain.RowDefinitions.Add(
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                        ));
                    Enumerable.Range(1, 4).ToList().ForEach(x =>
                        grdMain.ColumnDefinitions.Add(
                            new ColumnDefinition { Width = new GridLength(25, GridUnitType.Star) }
                        ));
                    //grdMain.Children.Clear();
                    grdMain.Children.Add(srcBuscar, 0, 0+4, 0, 0+1);
                    grdMain.Children.Add(btnNombre, 0, 0+3, 1, 1+1);
                    grdMain.Children.Add(txtNum, 3, 3+1, 1, 1+1);
                    grdMain.Children.Add(btnPais, 0, 2);
                    grdMain.Children.Add(btnOper, 1, 2);
                    grdMain.Children.Add(btnMonto, 2, 2);
                    grdMain.Children.Add(btnPago, 3, 2);

                    grdSecondary.ColumnDefinitions.Clear();
                    grdSecondary.RowDefinitions.Clear();
                    grdSecondary.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(55, GridUnitType.Star) });
                    Enumerable.Range(1, 3).ToList().ForEach(x =>
                        grdSecondary.ColumnDefinitions.Add(
                            new ColumnDefinition { Width = new GridLength(55, GridUnitType.Star) }
                        ));
                    grdSecondary.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                    grdSecondary.Children.Add(btnNombreMin, 0, 0);
                    grdSecondary.Children.Add(btnPaisMin, 1, 0);
                    grdSecondary.Children.Add(btnOperMin, 2, 0);
                    grdSecondary.Children.Add(btnMontoMin, 3, 0);
                }
                else //portrait
                {
                    grdMain.ColumnDefinitions.Clear();
                    grdMain.RowDefinitions.Clear();
                    Enumerable.Range(1, 7).ToList().ForEach(x =>
                        grdMain.RowDefinitions.Add(
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                        ));
                    grdMain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    //grdMain.Children.Clear();
                    grdMain.Children.Add(srcBuscar, 0, 0);
                    grdMain.Children.Add(btnNombre, 0, 1);
                    grdMain.Children.Add(txtNum, 0, 2);
                    grdMain.Children.Add(btnPais, 0, 3);
                    grdMain.Children.Add(btnOper, 0, 4);
                    grdMain.Children.Add(btnMonto, 0, 5);
                    grdMain.Children.Add(btnPago, 0, 6);

                    grdSecondary.ColumnDefinitions.Clear();
                    grdSecondary.RowDefinitions.Clear();
                    Enumerable.Range(1, 3).ToList().ForEach(x =>
                        grdSecondary.ColumnDefinitions.Add(
                            new ColumnDefinition { Width = new GridLength(33, GridUnitType.Star) }
                        ));
                    Enumerable.Range(1, 2).ToList().ForEach(x =>
                        grdSecondary.RowDefinitions.Add(
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                        ));
                    grdSecondary.Children.Add(btnNombreMin, 0, 0+3, 0, 0+1);
                    grdSecondary.Children.Add(btnPaisMin, 0, 1);
                    grdSecondary.Children.Add(btnOperMin, 1, 1);
                    grdSecondary.Children.Add(btnMontoMin, 2, 1);
                }
            }
        }

        public CargarSaldo2()
        {
            InitializeComponent();
            //cvwNueva = new VwNuevaTarjeta();
            lm.Clear();
            lm.Add(new TipoPago() { idtipo = 1, tipo = "Saldo", saldo = 50.00M });
            lt.Clear();
            lt.Add(new TipoPago() { idtipo = 2, tipo = "VISA 2152", saldo = 0, imgtipo = "visa.png" });

            try
            {
                notif = DependencyService.Get<IToastNotificator>();
                Title = "Cargar Saldo";
                lsC.Clear();
                status = Estado.Recarga;

                string title = ""; ListaOpciones lo;

                lvOpciones.ItemSelected += LvOpciones_ItemSelected;

                srcBuscar.SearchButtonPressed += (s, e) =>
                {
                    BuscarContacto(srcBuscar.Text);
                };
                srcBuscar.TextChanged += (object sender, TextChangedEventArgs e) =>
                {
                    //string filtro = e.NewTextValue;
                    BuscarContacto(e.NewTextValue);
                };
                btnPais.Clicked += (s, e) =>
                {
                    paso = PasosRecarga.Pais;
                    MuestraOpciones(false);
                };
                btnOper.Clicked += (s, e) =>
                {
                    paso = PasosRecarga.Operadora;
                    MuestraOpciones(true);
                };
                btnMonto.Clicked += (s, e) =>
                {
                    paso = PasosRecarga.Monto;
                    MuestraOpciones(true);
                };
                btnPago.Clicked += (s, e) =>
                {
                    paso = PasosRecarga.MedioPago;
                    MuestraOpcionesPago(true);
                };
                btnAplicar.Clicked += async (sender, ea) =>
                {
                    var page = new InfoConfirmar();
                    await Navigation.PushPopupAsync(page);

                    //await App.Nav.PushAsync(new Procesar(), Constantes.animated);
                    //await App.Nav.PushAsync(new RegPago(), Constantes.animated);
                };
                btnNombre.Clicked += async (s, e) =>
                {
                    //notif.Notify(ToastNotificationType.Info, "", "Puedes seleccionar el numero de la lista", TimeSpan.FromSeconds(2));
                    if (!lvContactos.IsVisible)
                    {
                        await MuestraContactos(false);
                    }
                };
                txtNum.Focused += (s, e) =>
                {
                    //if (!lvContactos.IsVisible)
                    //{
                    //    await MuestraContactos(false);
                    //}
                    //notif.Notify(ToastNotificationType.Info, "", "Aqui puedes capturar el numero", TimeSpan.FromSeconds(2));
                };
                txtNum.TextChanged += (sender, ea) =>
                {
                    string num = ea.NewTextValue;
                    if (num.Length >= 5)
                    {
                        if (lsC.Where(x => x.Number == num).Count() == 1)
                            btnNombre.Text = lsC.Where(x => x.Number == num).FirstOrDefault().Name;
                        else
                            btnNombre.Text = "";
                    }
                };
                btnGuardarNuevaT.Clicked += (s, e) =>
                {
                    BackAgregarNuevaT();

                    status = Estado.Recarga;
                    paso = PasosRecarga.MedioPago;
                    MuestraOpcionesPago(false);
                    slMain.IsVisible = true;
                    slButtonMain.IsVisible = true;
                    slSecondary.IsVisible = false;
                    slButtonSecondary.IsVisible = false;
                    svNueva.IsVisible = false;
                };
                btnCancelar.Clicked += (s, e) =>
                {
                    status = Estado.Recarga;
                    paso = PasosRecarga.MedioPago;
                    MuestraOpcionesPago(false);
                    slMain.IsVisible = true;
                    slButtonMain.IsVisible = true;
                    slSecondary.IsVisible = false;
                    slButtonSecondary.IsVisible = false;
                    svNueva.IsVisible = false;
                };
			}catch(Exception ex)
            {

            }
        }

        private void DismissClick(object sender, EventArgs e)
        {
        }

        public void BackAgregarNuevaT()
        {
            lt.Add(new TipoPago() { idtipo = 2, tipo = "AMEX 0133", saldo = 0, imgtipo = "amex.png" });
            gTarjetas = new Grupo("Tus tarjetas", 2, lt);
            grptipos.Clear();
            grptipos.Add(gMonedero);
            grptipos.Add(gTarjetas);
            lvRegPagos.BeginRefresh();
            lvRegPagos.ItemsSource = null;
            lvRegPagos.ItemsSource = grptipos;
            lvRegPagos.EndRefresh();
  
        }

        void AgregarNuevaT(object sender, System.EventArgs e)
		{
            //App.Nav.PushAsync(new NuevaTarjeta(TipoAnadidor.RegistroPago), Constantes.animated);
            status = Estado.NuevaTarjeta;
            MuestraLV(Listas.nuevacard);
            slMain.IsVisible = false;
            slButtonMain.IsVisible = false;
            slSecondary.IsVisible = true;
            slButtonSecondary.IsVisible = true;
        }

		private void MuestraLV(Listas l)
        {
            switch (l)
            {
                case Listas.contactos:
                    lvContactos.IsVisible = true;
                    lvOpciones.IsVisible = false;
                    lvRegPagos.IsVisible = false;
                    svlistos.IsVisible = false;
                    svNueva.IsVisible = false;
                    break;
                case Listas.opciones:
                    lvContactos.IsVisible = false;
                    lvOpciones.IsVisible = true;
                    lvRegPagos.IsVisible = false;
                    svlistos.IsVisible = false;
                    svNueva.IsVisible = false;
                    break;
                case Listas.pagos:
                    lvContactos.IsVisible = false;
                    lvOpciones.IsVisible = false;
                    lvRegPagos.IsVisible = true;
                    svlistos.IsVisible = false;
                    svNueva.IsVisible = false;
                    break;
				case Listas.listo:
					lvContactos.IsVisible = false;
					lvOpciones.IsVisible = false;
					lvRegPagos.IsVisible = false;
					svlistos.IsVisible = true;
                    svNueva.IsVisible = false;
                    cvwlistos.Anim();
					break;
                case Listas.nuevacard:
                    lvContactos.IsVisible = false;
                    lvOpciones.IsVisible = false;
                    lvRegPagos.IsVisible = false;
                    svlistos.IsVisible = false;
                    svNueva.IsVisible = true;
                    break;
            }
        }

        private void BuscarContacto(string filtro)
        {
            if (!lvContactos.IsVisible)
            {
                MuestraLV(Listas.contactos);
            }
            lvContactos.BeginRefresh();
            if (filtro.Trim() == "")
            {
                lvContactos.ItemsSource = Groups;
            }
            else
            {
                if (filtro.Length == 1)
                {
                    List<letragrupo> llg = Groups.Where(x => x.letra.ToUpper() == filtro.ToUpper()).ToList();
                    lvContactos.ItemsSource = llg;
                }
                else
                {
                    List<letragrupo> lg = new List<letragrupo>() { new letragrupo("", lsC.Where(x => x.Name.ToUpper().Contains(filtro.ToUpper()))) };
                    lvContactos.ItemsSource = lg;
                }
            }
            lvContactos.EndRefresh();
        }

        public void Limpiar()
        {
            paso = PasosRecarga.Numero;
            status = Estado.Recarga;
            //btnNombre.Text = "";    btnNombre.Source = "smartphonew.png";
            //btnNombreMin.Style = DefaultStyles.CargarSaldoBtnNombre;
        }

        async void Iniciar()
        {
			try
			{
                if (App.usr != null)
                {
                    switch (App.usr.pais.ToLower())
                    {
                        case "argentina":
                            btnPais.Source = "ar.png"; btnPaisMin.Source = "ar.png"; break;
                        case "mexico":
                            btnPais.Source = "mx.png"; btnPaisMin.Source = "mx.png"; break;
                        case "us":
                            btnPais.Source = "us.png"; btnPaisMin.Source = "us.png"; break;
                    }
                    btnPais.Text = App.usr.pais;
                    btnPaisMin.Text = App.usr.pais;
                }
                paso = PasosRecarga.Numero;
                UserDialogs.Instance.ShowLoading("Cargando...");
				await Task.Run(() =>
				{
					if (lsC.Count() == 0)
						lsC = DependencyService.Get<IContactos>().GetLista().OrderBy(x => x.Name).ToList();
					Device.BeginInvokeOnMainThread(() =>
					{
						UserDialogs.Instance.HideLoading();
						MuestraContactos(true);
						//App.Nav.PushAsync(new CargaContactos(lsC), Constantes.animated);
					});
				});
			}
			catch (Exception ex)
			{
			}
        }

        private void MuestraOpciones(bool showinfo)
        {
            ls.Clear();
            lvOpciones.SelectedItem = null;
            PasosRecarga ult = Enum.GetValues(typeof(PasosRecarga)).Cast<PasosRecarga>().Last();
            if (paso != ult)
            {
                if (paso == PasosRecarga.Pais)
                {
                    ls.Add(new Opcion() { idopc = 1, opc = "Mexico", imgopc = "mx.png" });
                    ls.Add(new Opcion() { idopc = 2, opc = "Argentina", imgopc = "ar.png" });
                    ls.Add(new Opcion() { idopc = 3, opc = "US", imgopc = "us.png" });
                }
                if (paso == PasosRecarga.Operadora)
                {
                    ls.Add(new Opcion() { idopc = 1, opc = "ATT", imgopc = "att.png" });
                    ls.Add(new Opcion() { idopc = 2, opc = "Movistar", imgopc = "movi.png" });
                    ls.Add(new Opcion() { idopc = 3, opc = "Nextel", imgopc = "nextel.png" });
                    ls.Add(new Opcion() { idopc = 4, opc = "Telcel", imgopc = "telcel.png" });
                    ls.Add(new Opcion() { idopc = 5, opc = "Unefon", imgopc = "unefon.png" });
                    ls.Add(new Opcion() { idopc = 6, opc = "Virgin", imgopc = "virgin.png" });
                    if (showinfo)
                    {
#if __IOS__
                        Version ver = new Version(UIKit.UIDevice.CurrentDevice.SystemVersion);
                        if (ver.Major <= 8)
                            notif.Notify(ToastNotificationType.Info, "2- Operadora", "Selecciona el carrier al cual pertenece tu numero, de la lista de opciones", TimeSpan.FromSeconds(5));
                        else
                        {
                            var page = new InfoRecargar("2- Operadora", "Selecciona el carrier al cual pertenece tu numero, de la lista de opciones");
                            Navigation.PushPopupAsync(page);
                        }
#else
					var page = new InfoRecargar("2- Operadora", "Selecciona el carrier al cual pertenece tu numero, de la lista de opciones");
					Navigation.PushPopupAsync(page);
#endif
                    }
				}
                if (paso == PasosRecarga.Monto)
                {
                    ls.Add(new Opcion() { idopc = 1, opc = "$50.00", imgopc = "" });
                    ls.Add(new Opcion() { idopc = 2, opc = "$100.00", imgopc = "" });
                    ls.Add(new Opcion() { idopc = 3, opc = "$200.00", imgopc = "" });
                    ls.Add(new Opcion() { idopc = 3, opc = "$500.00", imgopc = "" });
                    if (showinfo)
                    {
#if __IOS__
                        Version ver = new Version(UIKit.UIDevice.CurrentDevice.SystemVersion);
                        if (ver.Major <= 8)
                            notif.Notify(ToastNotificationType.Info, "3- Monto", "Selecciona el monto que quieres recargar de la lista de opciones", TimeSpan.FromSeconds(5));
                        else
                        {
                            var page = new InfoRecargar("3- Monto", "Selecciona el monto que quieres recargar de la lista de opciones");
                            Navigation.PushPopupAsync(page);
                        }
#else
					var page = new InfoRecargar("3- Monto", "Selecciona el monto que quieres recargar de la lista de opciones");
					Navigation.PushPopupAsync(page);
#endif
                    }
				}
                lvOpciones.IsGroupingEnabled = false;
                lvOpciones.ItemsSource = null;
                lvOpciones.ItemsSource = ls;

                MuestraLV(Listas.opciones);
            } else
            {
                lvContactos.IsVisible = false;
                //lvOpciones.IsVisible = false;
            }
        }

        private void MuestraOpcionesPago(bool showinfo)
        {
            MuestraLV(Listas.pagos);
            if (showinfo)
            {
#if __IOS__
                Version ver = new Version(UIKit.UIDevice.CurrentDevice.SystemVersion);
                if (ver.Major <= 8)
                    notif.Notify(ToastNotificationType.Info, "4- Forma de pago", "Selecciona el tipo de pago que quieras usar, puedes usar el saldo de tu monedero o alguna tarjeta. Tambien puedes dar de alta nuevas tarjetas", TimeSpan.FromSeconds(5));
                else
                {
                    var page = new InfoRecargar("4- Forma de pago", "Selecciona el tipo de pago que quieras usar, puedes usar el saldo de tu monedero o alguna tarjeta. Tambien puedes dar de alta nuevas tarjetas");
                    Navigation.PushPopupAsync(page);
                }
#else
			var page = new InfoRecargar("4- Forma de pago", "Selecciona el tipo de pago que quieras usar, puedes usar el saldo de tu monedero o alguna tarjeta. Tambien puedes dar de alta nuevas tarjetas");
			Navigation.PushPopupAsync(page);
#endif
            }
			grptipos = new ObservableCollection<Grupo>();
            gMonedero = new Grupo("Tu monedero", 1, lm);
            gTarjetas = new Grupo("Tus tarjetas", 2, lt);
            grptipos.Add(gMonedero);
            grptipos.Add(gTarjetas);
            lvRegPagos.BeginRefresh();
            lvRegPagos.ItemsSource = null;
            lvRegPagos.ItemsSource = grptipos;
            lvRegPagos.EndRefresh();
            lvRegPagos.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;
                notif.HideAll();
                TipoPago tp = (e.SelectedItem as TipoPago);
                //btnPago.Text = (tp.idtipo == 1 ? "Monedero" : "Tarjeta") + " - ";
                btnPago.Text = (tp.idtipo == 1 ? "Saldo: " + tp.saldo.ToString("c") : tp.tipo);
				btnPago.Source = (tp.idtipo == 1 ? "wallet.png" : tp.imgtipo);
                paso = (PasosRecarga)(((int)paso) + 1);
				if (paso == PasosRecarga.Recarga)
					MuestraLV(Listas.listo);
                ((ListView)sender).SelectedItem = null;
            };
        }

        
        private void LvOpciones_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            notif.HideAll();
            Opcion opc = e.SelectedItem as Opcion;
            switch (paso)
            {
                case PasosRecarga.Pais:
                    btnPais.Text = opc.opc;
                    btnPais.Source = opc.imgopc;
					//btnPais.ImageHeightRequest = 30;
					//btnPais.ImageHeightRequest = 50;
                    break;
                case PasosRecarga.Operadora:
                    btnOper.Text = opc.opc;
                    btnOper.Source = opc.imgopc;
					//btnOper.ImageHeightRequest = 30;
					//btnOper.ImageHeightRequest = 50;
                    break;
                case PasosRecarga.Monto:
                    btnMonto.Text = opc.opc;
					if (!String.IsNullOrEmpty(opc.imgopc))
                    	btnMonto.Source = opc.imgopc;
					//btnMonto.ImageHeightRequest = 30;
					//btnMonto.ImageHeightRequest = 50;
                    break;
            }
            paso = (PasosRecarga)(((int)paso) + 1);
            if (paso == PasosRecarga.MedioPago)
                MuestraOpcionesPago(true);
            else
                MuestraOpciones(true);
        }

        private async Task MuestraContactos(bool primeravez)
        {
            List<char> letras = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList();
            try
            {
                MuestraLV(Listas.contactos);
                Groups.Clear();
                foreach (char c in letras)
                {
                    Groups.Add(new letragrupo(c.ToString(), lsC.Where(x => x.Name.StartsWith(c.ToString())).ToList()));
                }
                txtNum.Completed += (s, e) =>
                {
                    lvContactos.IsVisible = false;

                    notif.HideAll();
                    if (primeravez)
                    {
                        paso = (PasosRecarga)(((int)paso) + 2);
                        MuestraOpciones(true);
                    }
                };
                lvContactos.ItemSelected += (sender, e) =>
                {
                    if (e.SelectedItem == null)
                        return;
                    notif.HideAll();
                    txtNum.Text = (e.SelectedItem as Contacto).Number;
                    btnNombre.Text = (e.SelectedItem as Contacto).Name;
                    if (primeravez)
                    {
                        paso = (PasosRecarga)(((int)paso) + 2);
                        MuestraOpciones(true);
                    }
                    ((ListView)sender).SelectedItem = null;
                };
                lvContactos.BeginRefresh();
                lvContactos.IsGroupingEnabled = true;
                lvContactos.ItemsSource = Groups;
                lvContactos.EndRefresh();

                if (primeravez)
                {
					//UserDialogs.Instance.Toast(new ToastConfig(ToastEvent.Info, "1- Para empezar.", "Selecciona algun contacto para recargar de la lista o captura el numero. Tambien puedes buscar contactos en el cuadro correspondiente")
					//               {
					//                   Duration = TimeSpan.FromSeconds(5),
					//                   BackgroundColor = System.Drawing.Color.BlueViolet, //Color.FromHex("#543192"),
					//                   TextColor = System.Drawing.Color.White,
					//                   //Position = ToastPosition.Top,
					//                   Action = async () => {
					//                       await txtNum.AnimateWinAsync();
					//                       await srcBuscar.AnimateWinAsync();
					//                   },
					//                   Icon = BitmapLoader.Current.LoadFromResource("ayuda.png", null, null).Result
					//               });

#if __IOS__
					Version ver = new Version(UIKit.UIDevice.CurrentDevice.SystemVersion);
					if (ver.Major <= 8)
						await notif.Notify(ToastNotificationType.Info, "1- Para empezar", "Selecciona algun contacto para recargar de la lista o captura el numero. Tambien puedes buscar contactos en el cuadro correspondiente", TimeSpan.FromSeconds(5));
					else {
						var page = new InfoRecargar("1- Para empezar", "Selecciona algun contacto para recargar de la lista o captura el numero. Tambien puedes buscar contactos en el cuadro correspondiente");
						await Navigation.PushPopupAsync(page);
					}
#else
					var page = new InfoRecargar("1- Para empezar", "Selecciona algun contacto para recargar de la lista o captura el numero. Tambien puedes buscar contactos en el cuadro correspondiente");
					await Navigation.PushPopupAsync(page);
#endif
					//await txtNum.AnimateWinAsync();
					//await srcBuscar.AnimateWinAsync();
				}
            }
            catch (Exception e)
            {

            }
        }

        public void SetVal(int idpos, Opcion opc)
        {
            opciones.Where(x => x.Pos == idpos).First().Desc = opc.opc;

            
        }


    }


    
}
