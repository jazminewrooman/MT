using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

using Xamarin.Forms;
using Plugin.Connectivity;
using System.ServiceModel;
using Acr.UserDialogs;
using Newtonsoft.Json;
using System.Net;

namespace MasTicket
{
	public partial class App : Application, ILoginManager
    {
        public static App Current;
        public static MasterDetailPage master;
        public static INavigation intnav;
        public static Usuario usr;
        static BD internaldb;
        public EndpointAddress EndPoint = new EndpointAddress(Settings.WebService);
        static IsacClient ws;
        static wscatalogos.wscatalogos wsc;

        public static wscatalogos.wscatalogos WSc
        {
            get
            {
                return wsc;
            }
        }
        public static IsacClient WS
        {
            get
            {
                return ws;
            }
        }
		public static BD db
        {
            get
            {
                return internaldb;
            }
        }
        public static INavigation Nav
        {
            get
            {
                return intnav;
            }
            set
            {
                intnav = value;
            }
        }

        public static bool IsLoggedIn
        {
            get
            {
                if (usr != null)
                    return (usr.idusuario == 0 ? false : true);  //!string.IsNullOrWhiteSpace(usr.idusuario);
                else
                    return false;
            }
        }

        public void ShowMainPage()
        {
            master = new MainPage();
            MainPage = master;
        }

		public void IniciaWS()
		{
			if (ws.State == CommunicationState.Closed)
			{
				BasicHttpBinding binding = CreateBasicHttp();
				ws = new IsacClient(binding, EndPoint);
				//------	Credenciales y SSL	--------------------------
				ws.ClientCredentials.UserName.UserName = Settings.WebServiceUsr;
				ws.ClientCredentials.UserName.Password = Settings.WebServicePwd;
				//------	Credenciales y SSL	--------------------------
				//ws.Open();
			}
		}

		private void InitializeServiceClient()
		{
			BasicHttpBinding binding = CreateBasicHttp();

			ws = new IsacClient(binding, EndPoint);
			wsc = new wscatalogos.wscatalogos();
			wsc.Url = Settings.WebServiceCat;

			//------	Credenciales y SSL	--------------------------
			ws.ClientCredentials.UserName.UserName = Settings.WebServiceUsr;
			ws.ClientCredentials.UserName.Password = Settings.WebServicePwd;
			
			CredentialCache credentialCache = new CredentialCache();
			NetworkCredential credentials = new NetworkCredential(Settings.WebServiceUsr, Settings.WebServicePwd);
			credentialCache.Add(new Uri(wsc.Url), "Basic", credentials);
			wsc.Credentials = credentialCache;
			//------	Credenciales y SSL	--------------------------

		}

		private static BasicHttpBinding CreateBasicHttp()
		{
			//------	Credenciales y SSL	--------------------------
			BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport)
			//------	Credenciales y SSL	--------------------------
			//BasicHttpBinding binding = new BasicHttpBinding()
			{
				Name = "basicHttpBinding",
				MaxBufferSize = 2147483647,
				MaxReceivedMessageSize = 2147483647,

			};
			TimeSpan timeout = new TimeSpan(0, 3, 0);
			binding.SendTimeout = timeout;
			binding.OpenTimeout = timeout;
			binding.ReceiveTimeout = timeout;

			//------	Credenciales y SSL	--------------------------
			binding.Security = new BasicHttpSecurity
			{
				Mode = BasicHttpSecurityMode.Transport,
				Transport = new HttpTransportSecurity
				{
					ClientCredentialType = HttpClientCredentialType.Basic,
					ProxyCredentialType = HttpProxyCredentialType.None,
					Realm = "",
				},
			};
			//------	Credenciales y SSL	--------------------------

			return binding;
		}

		async void Descarga()
		{
			MainPage = new Cargando();
			bool err = false;
			var canReach = await App.HayCnxSrv(Settings.WebServiceCat);
			if (!App.HayCnx() || !canReach)
			{
				//Page p = new Page();
				UserDialogs.Instance.HideLoading();
				await MainPage.DisplayAlert("Aviso", "No hay conexion a internet o el servicio no esta disponible. Consulte con soporte tecnico", "OK");
                MainPage = Reintentar();
			}
			else {
				CatalogosViewModel cvm = new CatalogosViewModel();
				//MainPage = new Cargando();
				err = await cvm.Descarga();
				if (err)
				{
					Page p = new Page();
					UserDialogs.Instance.HideLoading();
					await p.DisplayAlert("Aviso", "No hay conexion a internet o el servicio no esta disponible. Consulte con soporte tecnico", "OK");
                    MainPage = Reintentar();
				}
				else {
					usr = db.SelUsr();

					if (usr != null)
					{
						string json = "";
						json = App.WSc.GetCatalogo(9, "where idusuario = " + usr.idusuario.ToString());
						List<Tarjeta> lt = JsonConvert.DeserializeObject<List<tempuri.org.Tarjeta>>(json).Select(x => new Tarjeta { idtarjeta = x.idtarjeta, idpais = x.idpais, idemisor = x.idemisor, idusuario = x.idusuario, Last4 = x.Last4 }).ToList();
						db.DescargaTarjetas(lt);
						json = "";
						json = App.WSc.GetCatalogo(11, "where idusuario = " + usr.idusuario.ToString());
						List<Recarga> lr = JsonConvert.DeserializeObject<List<Recarga>>(json);
						db.DescargaRecargas(lr);
						json = "";
						json = App.WSc.GetCatalogo(12, "where idusuario = " + usr.idusuario.ToString());
						List<SaldoMonedero> sm = JsonConvert.DeserializeObject<List<SaldoMonedero>>(json);
						if (sm.Count() > 0)
							db.DescargaSaldo(sm.FirstOrDefault());
						json = "";
						json = App.WSc.GetCatalogo(13, "where idusuario = " + usr.idusuario.ToString());
						List<RecargaProg> lrp = JsonConvert.DeserializeObject<List<RecargaProg>>(json);
						db.DescargaRecargasProg(lrp);
					}

					UserDialogs.Instance.HideLoading();
					if (usr == null)
					{
						usr = new Usuario();
						if (Settings.TutoVisto1aVez)
						{
							catConfig cfg = App.db.SelcatConfig().Where(x => x.idconfig == 2).FirstOrDefault();
							if (cfg == null || (cfg != null && cfg.valor == "0")) //1a vez
							{
								//var det = new NavigationPage(new CargarSaldo(null))
								//{
								//	BarTextColor = Color.White,
								//	BarBackgroundColor = Color.FromHex("#e35102"),
								//	Title = "Así Compras",
								//};
								//intnav = det.Navigation;
								//MainPage = det;
								MainPage = new MainPage();
							}
							else { //cerre sesion y vuelvo a entrar
								var det = new NavigationPage(new Login(this))
								{
									BarTextColor = Color.White,
									BarBackgroundColor = Color.FromHex("#e35102"),
									Title = "Así compras",
								};
								intnav = det.Navigation;
								MainPage = det;
							}
						}
						else
							MainPage = new Tuto(this);
					}
					else
					{
						MainPage = new MainPage(); //ShowMainPage();
					}
				}
			}
		}

		ContentPage Reintentar()
		{
			StackLayout sl = new StackLayout() { VerticalOptions = LayoutOptions.Center };
			Button bt = new Button() { Text = "Reintentar" };
			bt.Style = App.Current.Resources["ButtonRojo"] as Style;
			bt.Clicked += (sender, e) => Descarga();
			sl.Children.Add(bt);
			ContentPage cp = new ContentPage() { Content = sl };
			return (cp);
		}

		public App()
		{
			InitializeComponent ();
            InitializeServiceClient();
            internaldb = new BD();
            Current = this;

			CancellationTokenSource ts = new CancellationTokenSource();
			CancellationToken ct = ts.Token;
			CrossConnectivity.Current.ConnectivityChanged += async (sender, args) =>
            {
				if (!args.IsConnected)
				{
					//Page p = new Page();
					//p.DisplayAlert("Aviso", "No hay conexion a internet", "OK");

					//UserDialogs.Instance.HideLoading();
					//await MainPage.DisplayAlert("Aviso", "No hay conexion a internet o el servicio no esta disponible. Consulte con soporte tecnico", "OK");
					//MainPage = Reintentar();

					try
					{
						await UserDialogs.Instance.AlertAsync("No hay conexión a internet, revise su configuración WiFi o 3G", "Aviso", "OK", ct);
					}
					catch (OperationCanceledException)
					{
						ts = new CancellationTokenSource();
						ct = ts.Token;
					}
				}
				else {
					ts.Cancel();
					ts = new CancellationTokenSource();
					ct = ts.Token;
				}
            };
			//MainPage = new Cargando();
			Descarga();
        }
		
		public static bool HayCnx()
		{
			return (CrossConnectivity.Current.IsConnected);
		}
		
		public async static Task<bool> HayCnxSrv(string srv)
		{
			//return(await CrossConnectivity.Current.IsRemoteReachable(srv));

			bool ret = false;
			try
			{
				using (HttpClient client = new HttpClient() { Timeout = System.TimeSpan.FromSeconds(5) })
				{
					//------	Credenciales y SSL	--------------------------
					var byteArray = Encoding.ASCII.GetBytes(Settings.WebServiceUsr + ":" + Settings.WebServicePwd);
					var header = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
					client.DefaultRequestHeaders.Authorization = header;
					//------	Credenciales y SSL	--------------------------

					using (HttpResponseMessage response = await client.GetAsync(srv))
					{
						ret = response.IsSuccessStatusCode; //200 = ok
					}
				}
			}
			catch
			{
				ret = false; //se cancela la Task
			}
			return (ret);
		}

		public Action CancelAction
		{
			get
			{
				return new Action(async () =>
				{
					try
					{
						/*var log = App.Nav.NavigationStack.OfType<Login>().FirstOrDefault();
						if (log != null)
							await log.CancelFB();*/
						if (Nav.ModalStack.Count() > 0)
							await Nav.PopModalAsync(Constantes.animated);
						//await Nav.PopAsync(Constantes.animated);
					}
					catch (Exception e)
					{

					}
				});
			}
		}

		public Action SuccessfulLoginAction
        {
            get
            {
				return new Action(async () =>
				{
					try
					{
						//MainPage = new Cargando();
						string json = App.WSc.GetUser(0, usr.email, "");
						List<Usuario> lu = JsonConvert.DeserializeObject<List<Usuario>>(json);
						if (lu.Count > 0)
						{
							if (lu.FirstOrDefault().idusuario == -1)
							{
								if (Nav.ModalStack.Count() > 0)
									await Nav.PopModalAsync(Constantes.animated);
								var blq = App.Nav.NavigationStack.OfType<Login>().FirstOrDefault();
								if (blq != null)
									blq.ShowMsgBloq();
							}
							else {
								App.usr = lu.FirstOrDefault();
								json = "";
								json = App.WSc.GetCatalogo(9, "where idusuario = " + usr.idusuario.ToString());
								List<Tarjeta> lt = JsonConvert.DeserializeObject<List<tempuri.org.Tarjeta>>(json).Select(x => new Tarjeta { idtarjeta = x.idtarjeta, idpais = x.idpais, idemisor = x.idemisor, idusuario = x.idusuario, Last4 = x.Last4 }).ToList();
								//List<Tarjeta> lt = JsonConvert.DeserializeObject<List<Tarjeta>>(json);
								db.DescargaTarjetas(lt);
								json = "";
								json = App.WSc.GetCatalogo(11, "where idusuario = " + usr.idusuario.ToString());
								List<Recarga> lr = JsonConvert.DeserializeObject<List<Recarga>>(json);
								db.DescargaRecargas(lr);
								json = "";
								json = App.WSc.GetCatalogo(14, "where idusuario = " + usr.idusuario.ToString());
								List<RecargaMonedero> lrm = JsonConvert.DeserializeObject<List<RecargaMonedero>>(json);
								db.DescargaRecargasWallet(lrm);
								json = "";
								json = App.WSc.GetCatalogo(12, "where idusuario = " + usr.idusuario.ToString());
								List<SaldoMonedero> sm = JsonConvert.DeserializeObject<List<SaldoMonedero>>(json);
								if (sm.Count() > 0)
									db.DescargaSaldo(sm.FirstOrDefault());
								json = "";
								json = App.WSc.GetCatalogo(13, "where idusuario = " + usr.idusuario.ToString());
								List<RecargaProg> lrp = JsonConvert.DeserializeObject<List<RecargaProg>>(json);
								db.DescargaRecargasProg(lrp);

								var nva = App.Nav.NavigationStack.OfType<NuevaTarjeta>().FirstOrDefault();
								if (nva != null)
								{
									if (db.SelUsr(usr.email) == null)
										db.AltaUsr(lu.FirstOrDefault());
									((App.Current.MainPage as MasterDetailPage).Master as menu).Refrescamenu();
									if (App.master == null)
										App.master = (App.Current.MainPage as MasterDetailPage);

									if (Nav.ModalStack.Count() > 0)
										await Nav.PopModalAsync(Constantes.animated);
									//await Nav.PopAsync(Constantes.animated);
									await Nav.PopAsync(Constantes.animated);
									nva.AltaEnPrimeraVez();
								}
								else {
									//cerro sesion, y ahora entra, si no existe en db local debo dar de alta
									if (db.SelUsr(usr.email) == null)
										db.AltaUsr(lu.FirstOrDefault());
									usr = lu.FirstOrDefault();
									UserDialogs.Instance.HideLoading();
									var mp = new MainPage();
									mp.Title = "Loqsea";
									MainPage.Title = "Loqsea";
									MainPage = mp;
								}
							}
						}
						else {
							UserDialogs.Instance.HideLoading();
							var det = new Registro(this);
							if (Nav.ModalStack.Count() > 0)
								await Nav.PopModalAsync(Constantes.animated);
							//await Nav.PopAsync(Constantes.animated);
							if (usr != null)
							{
								//antes
								//Nav.InsertPageBefore(det, Nav.NavigationStack.First());
								//Nav.PopToRootAsync(Constantes.animated);
								//
								await Nav.PushAsync(det, Constantes.animated);

							}
						}
					}
					catch (Exception e)
					{

					}
				});
            }
        }
    }
}
