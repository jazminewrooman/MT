using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Plugin.Connectivity;
using DeviceOrientation.Forms.Plugin.Abstractions;
using Acr.UserDialogs;
using Newtonsoft.Json;

namespace MasTicket
{
	public partial class Login : ContentPage
	{
        private void CambiaOrientacion()
        {
            IDeviceOrientation _deviceOrientationSvc = DependencyService.Get<IDeviceOrientation>();
            DeviceOrientations dvcor = _deviceOrientationSvc.GetOrientation();
            if (dvcor == DeviceOrientations.Landscape) //apaisado
            {
                //BackgroundImage = "backfoto2.jpg";
                grdMain.ColumnDefinitions.Clear();
                grdMain.RowDefinitions.Clear();
                Enumerable.Range(1, 3).ToList().ForEach(x =>
                    grdMain.RowDefinitions.Add(
                        new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) }
                    ));
                grdMain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
                grdMain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40, GridUnitType.Star) });
                grdMain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) });
                grdMain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40, GridUnitType.Star) });
                grdMain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
                grdMain.Children.Add(btnFace, 1, 0);
                grdMain.Children.Add(btnGoogle, 3, 0);
                grdMain.Children.Add(btnReg, 1, 1);
                grdMain.Children.Add(btnLogin, 3, 1);

				grdLogin.ColumnDefinitions.Clear();
				grdLogin.RowDefinitions.Clear();
				Enumerable.Range(1, 3).ToList().ForEach(x =>
					grdLogin.RowDefinitions.Add(
						new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) }
					));
				grdLogin.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
				grdLogin.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40, GridUnitType.Star) });
				grdLogin.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) });
				grdLogin.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40, GridUnitType.Star) });
				grdLogin.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
				grdLogin.Children.Add(edtCorreo, 1, 0);
				grdLogin.Children.Add(nipCapture, 3, 0);
				grdLogin.Children.Add(btnEntrar, 1, 1);
				grdLogin.Children.Add(btnCancelar, 3, 1);
            }
            else //portrait
            {
                //BackgroundImage = "backfoto1.jpg";
                grdMain.ColumnDefinitions.Clear();
                grdMain.RowDefinitions.Clear();
                Enumerable.Range(1, 3).ToList().ForEach(x =>
                    grdMain.RowDefinitions.Add(
                        new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) }
                    ));
                grdMain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10, GridUnitType.Absolute) });
                grdMain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });
                grdMain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10, GridUnitType.Absolute) });
                grdMain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
                grdMain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90, GridUnitType.Star) });
                grdMain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
                grdMain.Children.Add(btnFace, 1, 0);
                grdMain.Children.Add(btnGoogle, 1, 1);
                grdMain.Children.Add(btnReg, 1, 2);
                grdMain.Children.Add(btnLogin, 1, 4);

				grdLogin.ColumnDefinitions.Clear();
				grdLogin.RowDefinitions.Clear();
				Enumerable.Range(1, 4).ToList().ForEach(x =>
					grdLogin.RowDefinitions.Add(
						new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) }
					));
				grdLogin.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80, GridUnitType.Absolute) });
				grdLogin.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
				grdLogin.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90, GridUnitType.Star) });
				grdLogin.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
				grdLogin.Children.Add(edtCorreo, 1, 0);
				grdLogin.Children.Add(nipCapture, 1, 1);
				grdLogin.Children.Add(btnEntrar, 1, 2);
				grdLogin.Children.Add(btnCancelar, 1, 3);
            }
        }

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

		public async void ShowMsgBloq()
		{
			await DisplayAlert("Error", "El usuario se encuentra bloqueado. Revise el correo con el que se registro", "OK");
		}

		public async Task CancelFB()
		{
			try
			{
				if (App.Nav.ModalStack.Count > 0)
					await App.Nav.PopModalAsync(Constantes.animated);
			}
			catch (Exception ex)
			{
			}
		}

        public Login (ILoginManager ilm)
		{
			InitializeComponent ();
			try
			{
				CambiaOrientacion();
				MessagingCenter.Subscribe<DeviceOrientationChangeMessage>(this, DeviceOrientationChangeMessage.MessageId, (message) =>
				{
					CambiaOrientacion();
				});

				Title = "Asi Compras";
				NavigationPage.SetBackButtonTitle(this, "");

				grdMain.IsVisible = true;
				grdLogin.IsVisible = false;
				edtCorreo.Completed += (s, e) => nipCapture.SetFocus();

				TapGestureRecognizer tapolvide = new TapGestureRecognizer();
				tapolvide.Tapped += async (s, e) =>
				{
					await App.Nav.PushAsync(new OlvideNip());
				};
				lblOlvide.GestureRecognizers.Add(tapolvide);
				btnFace.Clicked += async (sender, ea) =>
				{
					if (!CrossConnectivity.Current.IsConnected)
					{
						await DisplayAlert("Aviso", "Necesita una conexion a internet", "OK");
						return;
					}
					await App.Nav.PushModalAsync(new loginfacebook(TipoSocial.facebook), Constantes.animated);
					//await App.Nav.PushAsync(new loginfacebook(TipoSocial.facebook), Constantes.animated);
				};
				btnGoogle.Clicked += async (sender, ea) =>
				{
					if (!CrossConnectivity.Current.IsConnected)
					{
						await DisplayAlert("Aviso", "Necesita una conexion a internet", "OK");
						return;
					}
					await App.Nav.PushModalAsync(new loginfacebook(TipoSocial.google), Constantes.animated);
					//await App.Nav.PushAsync(new loginfacebook(TipoSocial.google), Constantes.animated);
				};
				btnReg.Clicked += async (sender, ea) =>
				{
					App.usr = new Usuario();
					//await Navigation.PushAsync(new Registro(ilm));
					await App.Nav.PushAsync(new Registro(ilm), Constantes.animated);
				};
				btnLogin.Clicked += (s, e) =>
				{
					grdMain.IsVisible = false;
					grdLogin.IsVisible = true;
				};
				btnCancelar.Clicked += (s, e) =>
				{
					grdMain.IsVisible = true;
					grdLogin.IsVisible = false;
				};
				btnEntrar.Clicked += async (s, e) =>
				{
					string email = "";
					if (!String.IsNullOrEmpty(edtCorreo.Text))
						email = check.SafeSqlLiteral(edtCorreo.Text.Trim());
					string nip = check.SafeSqlLiteral(nipCapture.Nip);
					if (email == "" || nip == "")
						await DisplayAlert("Error", "No existe ese usuario y/o nip", "OK");
					else {
						string json = App.WSc.GetUser(0, email, nip);
						List<Usuario> lu = JsonConvert.DeserializeObject<List<Usuario>>(json);
						if (lu.Count > 0)
						{
							if (lu.FirstOrDefault().idusuario == -1)
								await DisplayAlert("Error", "El usuario se encuentra bloqueado. Revise el correo con el que se registro", "OK");
							else
							{
								UserDialogs.Instance.ShowLoading("Cargando...");
								App.usr = lu.FirstOrDefault();
								json = "";
								json = App.WSc.GetCatalogo(9, "where idusuario = " + App.usr.idusuario.ToString());
								List<Tarjeta> lt = JsonConvert.DeserializeObject<List<tempuri.org.Tarjeta>>(json).Select(x => new Tarjeta { idtarjeta = x.idtarjeta, idpais = x.idpais, idemisor = x.idemisor, idusuario = x.idusuario, Last4 = x.Last4 }).ToList();
								App.db.DescargaTarjetas(lt);
								json = "";
								json = App.WSc.GetCatalogo(11, "where idusuario = " + App.usr.idusuario.ToString());
								List<Recarga> lr = JsonConvert.DeserializeObject<List<Recarga>>(json);
								App.db.DescargaRecargas(lr);
								json = "";
								json = App.WSc.GetCatalogo(14, "where idusuario = " + App.usr.idusuario.ToString());
								List<RecargaMonedero> lrm = JsonConvert.DeserializeObject<List<RecargaMonedero>>(json);
								App.db.DescargaRecargasWallet(lrm);
								json = "";
								json = App.WSc.GetCatalogo(12, "where idusuario = " + App.usr.idusuario.ToString());
								List<SaldoMonedero> sm = JsonConvert.DeserializeObject<List<SaldoMonedero>>(json);
								if (sm.Count() > 0)
									App.db.DescargaSaldo(sm.FirstOrDefault());
								json = "";
								json = App.WSc.GetCatalogo(13, "where idusuario = " + App.usr.idusuario.ToString());
								List<RecargaProg> lrp = JsonConvert.DeserializeObject<List<RecargaProg>>(json);
								App.db.DescargaRecargasProg(lrp);

								//cerro sesion, y ahora entra, si no existe en db local debo dar de alta
								if (App.db.SelUsr(App.usr.email) == null)
									App.db.AltaUsr(lu.FirstOrDefault());

								var nva = App.Nav.NavigationStack.OfType<NuevaTarjeta>().FirstOrDefault();
								if (nva != null)
								{
									((App.Current.MainPage as MasterDetailPage).Master as menu).Refrescamenu();
									if (App.master == null)
										App.master = (App.Current.MainPage as MasterDetailPage);

									UserDialogs.Instance.HideLoading();
									await App.Nav.PopAsync(Constantes.animated);
									nva.AltaEnPrimeraVez();
								}
								else {
									UserDialogs.Instance.HideLoading();
									App.Current.MainPage = new MainPage();
								}
							}
						}
						else {
							UserDialogs.Instance.HideLoading();
							await DisplayAlert("Error", "No existe ese usuario y/o nip", "OK");
						}
					}
				};
			}
			catch (Exception ex)
			{
				
			}
        }
	}
}
