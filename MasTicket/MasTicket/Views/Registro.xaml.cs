using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;

using Xamarin.Forms;
using Acr.UserDialogs;
using Splat;
using Rg.Plugins.Popup.Extensions;
using Newtonsoft.Json;

namespace MasTicket
{
    public partial class Registro : ContentPage
    {
        ILoginManager _ilm;
        bool nip = false;

		public Registro(ILoginManager ilm)
		{
			try
			{
				InitializeComponent();
				Title = "Registro";

				_ilm = ilm;
				if (App.usr == null)
				{
					Usuario u = new Usuario()
					{
						//id = "1",
						registrado = false,
					};
				}
				edtNombre.Completed += (s, e) => edtCorreo.Focus();
				edtCorreo.Completed += (s, e) => edtNumCto.Focus();
				edtNumCto.Completed += (s, e) => nipCapture.SetFocus();

				UserDialogs.Instance.HideLoading();
				//var fs = new FormattedString();
				//fs.Spans.Add(new Span { Text = "Al hacer click en ", ForegroundColor = Color.Black, Font = Font.SystemFontOfSize(12) });
				//fs.Spans.Add(new Span { Text = " Entrar ", ForegroundColor = Color.FromHex("#e35102"), FontAttributes = FontAttributes.Bold, Font = Font.SystemFontOfSize(14) });
				//fs.Spans.Add(new Span { Text = " usted acepta la ", ForegroundColor = Color.Black, Font = Font.SystemFontOfSize(12) });
				//fs.Spans.Add(new Span { Text = " politica de privacidad ", ForegroundColor = Color.FromHex("#553191"), FontAttributes = FontAttributes.Bold, Font = Font.SystemFontOfSize(14) });
				//fs.Spans.Add(new Span { Text = "y los ", ForegroundColor = Color.Black, Font = Font.SystemFontOfSize(12) });
				//fs.Spans.Add(new Span { Text = "Terminos y condiciones", ForegroundColor = Color.FromHex("#553191"), FontAttributes = FontAttributes.Bold, Font = Font.SystemFontOfSize(14) });
				////lblPolitica.FormattedText = fs;

				TapGestureRecognizer tappoliticas = new TapGestureRecognizer();
				tappoliticas.Tapped += (s, e) =>
				{
					var page = new InfoAyuda("politica.html");
					Navigation.PushPopupAsync(page);
				};
				lblPoliticas.GestureRecognizers.Add(tappoliticas);
				TapGestureRecognizer tapterminos = new TapGestureRecognizer();
				tapterminos.Tapped += (s, e) =>
				{
					var page = new InfoAyuda("terminos.html");
					Navigation.PushPopupAsync(page);
				};
				lblTerminos.GestureRecognizers.Add(tapterminos);

				if (App.usr != null)
				{
					if (!String.IsNullOrEmpty(App.usr.picture))
					{
						imgFoto.Source = ImageSource.FromUri(new Uri(App.usr.picture));
						imgFoto.IsVisible = true;
					}
					if (!String.IsNullOrEmpty(App.usr.email))
						edtCorreo.Text = App.usr.email;
					if (!String.IsNullOrEmpty(App.usr.first_name))
						edtNombre.Text = App.usr.first_name + (!String.IsNullOrEmpty(App.usr.last_name) ? " " + App.usr.last_name : "");
				}
				btnLogin.Clicked += async (sender, ea) =>
				{
					if (!check.ValidaNip(nipCapture.Nip))
						await DisplayAlert("Error", "Revise el NIP. (No se permiten consecutivos ni que todos los digitos sean iguales)", "OK");
					else {
						btnLogin.IsEnabled = false;

						UserDialogs.Instance.ShowLoading("Cargando...");
						string json = App.WSc.GetUser(0, check.SafeSqlLiteral(edtCorreo.Text.Trim()), "");
						List<Usuario> lu = JsonConvert.DeserializeObject<List<Usuario>>(json);
						if (lu.Count > 0)
						{
							btnLogin.IsEnabled = true;
							UserDialogs.Instance.HideLoading();
							await DisplayAlert("Error", "Ya existe un usuario con este correo. Utilice el boton \"Ya tengo cuenta\"", "OK");
							/*App.usr = lu.FirstOrDefault();
							json = "";
							json = App.WSc.GetCatalogo(9, "where idusuario = " + App.usr.idusuario.ToString());
							List<Tarjeta> lt = JsonConvert.DeserializeObject<List<tempuri.org.Tarjeta>>(json).Select(x => new Tarjeta { idtarjeta = x.idtarjeta, idpais = x.idpais, idemisor = x.idemisor, numero = x.numero, expiramm = x.expiramm, expirayy = x.expirayy, titularFN = x.titularFN, titularLN = x.titularLN, calleynumero = x.calleynumero, idciudad = x.idciudad, codigopostal = x.codigopostal, idestado = x.idestado, idusuario = x.idusuario, cvv = Crypto.EncryptAes(x.cvv, Settings.PwdCrypto, Crypto.String2ByteArray(Settings.SaltCrypto)) }).ToList();
							App.db.DescargaTarjetas(lt);
							json = "";
							json = App.WSc.GetCatalogo(11, "where idusuario = " + App.usr.idusuario.ToString());
							List<Recarga> lr = JsonConvert.DeserializeObject<List<Recarga>>(json);
							App.db.DescargaRecargas(lr);
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
							}*/
						}
						else {
							App.usr.email = check.SafeSqlLiteral(edtCorreo.Text);
							App.usr.name = check.SafeSqlLiteral(edtNombre.Text);
							App.usr.idpais = cvwPais.IdPais;
							App.usr.nip = nipCapture.Nip;
							App.usr.registrado = true;
							App.usr.numerocontacto = check.SafeSqlLiteral(edtNumCto.Text);
							App.usr.fechaalta = DateTime.Now;
							UserDialogs.Instance.ShowLoading("Registrando...");
							EventHandler<AltaUsrCompletedEventArgs> auc = null;
							auc = (s, e) =>
							{
								App.WS.AltaUsrCompleted -= auc;
								Device.BeginInvokeOnMainThread(async () =>
								{
									UserDialogs.Instance.HideLoading();
									if (e.Error != null || e.Cancelled)
									{
										await DisplayAlert("Error", "Ocurrio un error, vuelva a intentar", "OK");
										btnLogin.IsEnabled = true;
									}
									if (e.Error == null && !e.Cancelled)
									{
										if (e.Result > 0)
										{
											App.usr.idusuario = e.Result;
											App.db.AltaUsr(App.usr);
											var nva = App.Nav.NavigationStack.OfType<NuevaTarjeta>().FirstOrDefault();
											if (nva != null)
											{
												((App.Current.MainPage as MasterDetailPage).Master as menu).Refrescamenu();
												if (App.master == null)
													App.master = (App.Current.MainPage as MasterDetailPage);

												await App.Nav.PopAsync(Constantes.animated);
												await App.Nav.PopAsync(Constantes.animated);
												nva.AltaEnPrimeraVez();
											}
											else {
												UserDialogs.Instance.HideLoading();
												App.Current.MainPage = new MainPage();
											}
										} else
											await DisplayAlert("Error", "Ocurrio un error, vuelva a intentar", "OK");
									}
								});
							};
							App.WS.AltaUsrCompleted += auc;
							App.WS.AltaUsrAsync(ConvertUsuarioToTemp(App.usr));
						}
					}
				};
			}
			catch (Exception e)
			{

			}
		}

		private tempuri.org.Usuario ConvertUsuarioToTemp(Usuario u)
		{
			tempuri.org.Usuario tu = new tempuri.org.Usuario
			{
				email = u.email,
				name  = u.name, first_name = u.first_name, last_name = u.last_name,
				gender = u.gender, picture = u.picture,
				idpais = u.idpais,
				nip = u.nip,
				registrado = u.registrado,
				numerocontacto = u.numerocontacto,
				fechaalta = u.fechaalta,
			};
			return (tu);
		}

        private void ValidEntries(object sender, PropertyChangedEventArgs e)
        {
            lblvalNip.IsVisible = !((nipval.IsValid && nipconfval.IsValid) && (nipCapture.Nip == nipConfirme.Nip));
            btnLogin.IsEnabled = (phvalNum.IsValid && emvalNum.IsValid && nomvalNum.IsValid) && (nipval.IsValid && nipconfval.IsValid) && (nipCapture.Nip == nipConfirme.Nip);
        }


    }

    public class MuestraErroresConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool muestra = false;
            bool isvalid = (bool)value;
            if (isvalid)
                muestra = false;
            else
                muestra = true;
            return (muestra);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
