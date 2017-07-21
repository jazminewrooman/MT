using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using ModernHttpClient;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Net.Http.Headers;

namespace MasTicket
{
    public class RecargasViewModel : CatalogosViewModel
    {
		private GetSessionTags tags;

        public bool ReadOnly { get; set; }
        public bool EdicionOnly { get; set; }

		private TipoTransaccion _tipotrans;
		public TipoTransaccion TipoTrans
		{
			get
			{
				return _tipotrans;
			}
			set
			{
				_tipotrans = value;
			}
		}

		private string errvestadetallado;
		public string errVestaDetallado
		{
			get
			{
				return errvestadetallado;
			}
		}

		private errRecarga _err;
		public errRecarga Err
		{
			get
			{
				return _err;
			}
			set
			{
				if (value != _err)
				{
					_err = value;
					NotifyPropertyChanged("Err");
				}
			}
		}
		private int _idusuario;
		public int IdUsuario
		{
			get
			{
				return _idusuario;
			}
			set
			{
				if (value != _idusuario)
				{
					_idusuario = value;
					NotifyPropertyChanged("IdUsuario");
				}
			}
		}
        private string _diasrecarga;
        public string DiasRecarga
        {
            get
            {
                return _diasrecarga;
            }
            set
            {
                if (value != _diasrecarga)
                {
                    _diasrecarga = value;
                    NotifyPropertyChanged("DiasRecarga");
                }
            }
        }
		private TipoRecarga _tiporecarga;
		public TipoRecarga Tiporecarga
		{
			get
			{
				return _tiporecarga;
			}
			set
			{
				if (value != _tiporecarga)
				{
					_tiporecarga = value;
					NotifyPropertyChanged("TipoRecarga");
				}
			}
		}
		private decimal _montorecargamonedero;
		public decimal MontoRecargaMonedero
		{
			get
			{
				return _montorecargamonedero;
			}
			set
			{
				if (value != _montorecargamonedero)
				{
					_montorecargamonedero = value;
					NotifyPropertyChanged("MontoRecargaMonedero");
				}
			}
		}
		private string _contactorecarga;
		public string ContactoRecarga
		{
			get
			{
				return _contactorecarga;
			}
			set
			{
				if (value != _contactorecarga)
				{
					_contactorecarga = value;
					NotifyPropertyChanged("ContactoRecarga");
				}
			}
		}
		private string _numerorecarga;
		public string NumeroRecarga
		{
			get
			{
				return _numerorecarga;
			}
			set
			{
				if (value != _numerorecarga)
				{
					_numerorecarga = value;
					NotifyPropertyChanged("NumeroRecarga");
				}
				OnRecargaIncompleta(null);
			}
		}
		private int _idformapago;
		public int idFormaPago
		{
			get
			{
				return _idformapago;
			}
			set
			{
				if (value != _idformapago)
				{
					_idformapago = value;
					NotifyPropertyChanged("idFormaPago");
				}
			}
		}
		private int _idtarjeta;
		public int IdTarjeta
		{
			get
			{
				return _idtarjeta;
			}
			set
			{
				if (value != _idtarjeta)
				{
					_idtarjeta = value;
					NotifyPropertyChanged("IdTarjeta");
				}
			}
		}
        private int _idpais;
        public int idpais
        {
            get
            {
                return _idpais;
            }
            set
            {
                if (value != _idpais)
				{
					if (value > 0)
					{
						_idpais = value;
						_idoperadora = 0;
						_idpaquete = 0;
						_opcpais.idOpcion = value;
						_opcpais.Desc = lsPais().Where(x => x.idpais == value).FirstOrDefault().pais;
						_opcpais.Image = lsPais().Where(x => x.idpais == value).FirstOrDefault().img;
					}
					else {
						_opcpais.idOpcion = 0;
						_opcpais.Desc = "Seleccione un país";
						if (App.usr.idpais != 0)
						{
							_opcpais.idOpcion = App.usr.idpais;
							_opcpais.Desc = lsPais().Where(x => x.idpais == App.usr.idpais).FirstOrDefault().pais;
						}
					}
                    _opcoperadora.idOpcion = 0;
                    _opcoperadora.Desc = "Seleccione una compañía";
					_opcpaquete.idOpcion = 0;
					_opcpaquete.Desc = "Seleccione un monto";
                    NotifyPropertyChanged("opcPais");
					NotifyPropertyChanged("opcOperadora");
					NotifyPropertyChanged("opcPaquete");
                }
				OnRecargaIncompleta(null);
			}
        }
        private int _idoperadora;
        public int idoperadora
        {
            get
            {
                return _idoperadora;
            }
            set
            {
                if (value != _idoperadora)
                {
                    _idpaquete = 0;
					catOperadora co = LsOperadoras(_idpais).Where(x => x.idoperadora == value).FirstOrDefault();
					if (co != null)
					{
						_idoperadora = value;
						_opcoperadora.idOpcion = value;
						_opcoperadora.Desc = co.operadora;
					}
					_opcpaquete.idOpcion = 0;
					_opcpaquete.Desc = "Seleccione un monto";
					NotifyPropertyChanged("opcOperadora");
					NotifyPropertyChanged("opcPaquete");
                }
				OnRecargaIncompleta(null);
			}
        }
        private int _idpaquete;
        public int idpaquete
        {
            get
            {
                return _idpaquete;
            }
            set
            {
                if (value != _idpaquete)
                {
                    catPaquete cp = LsPaquetes(_idoperadora).Where(x => x.idpaquete == value).FirstOrDefault();
					if (cp != null)
					{
						_idpaquete = value;
						_opcpaquete.idOpcion = value;
						_opcpaquete.Desc = cp.monto.ToString("c", new System.Globalization.CultureInfo("es-MX"));
					}
					NotifyPropertyChanged("opcPaquete");
                }
				OnRecargaIncompleta(null);
			}
        }
		private opcMenu _opclimpiar;
		public opcMenu opcLimpiar
		{
			get
			{
				return _opclimpiar;
			}
			set
			{
				if (value != _opclimpiar)
				{
					_opclimpiar = value;
					NotifyPropertyChanged();
				}
			}
		}
		private opcMenu _opcpais;
        public opcMenu opcPais
        {
            get
            {
                return _opcpais;
            }
            set
            {
                if (value != _opcpais)
                {
                    _opcpais = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private opcMenu _opcoperadora;
        public opcMenu opcOperadora
        {
            get
            {
                return _opcoperadora;
            }
            set
            {
                if (value != _opcoperadora)
                {
                    _opcoperadora = value;
                    NotifyPropertyChanged();
                }
            }
        }
		private opcMenu _opcpaquete;
		public opcMenu opcPaquete
		{
			get
			{
				return _opcpaquete;
			}
			set
			{
				if (value != _opcpaquete)
				{
					_opcpaquete = value;
					NotifyPropertyChanged();
				}
			}
		}
		private opcMenu _opcnumero;
		public opcMenu opcNumero
		{
			get
			{
				return _opcnumero;
			}
			set
			{
				if (value != _opcnumero)
				{
					_opcnumero = value;
					NotifyPropertyChanged();
				}
			}
		}
		private opcMenu _opccupon;
		public opcMenu opcCupon
		{
			get
			{
				return _opccupon;
			}
			set
			{
				if (value != _opccupon)
				{
					_opccupon = value;
					NotifyPropertyChanged();
				}
			}
		}

		public event EventHandler<RecargaIncompletaEventArgs> RecargaIncompleta;
		protected virtual void OnRecargaIncompleta(RecargaIncompletaEventArgs e)
		{
			var handler = RecargaIncompleta;
			if (handler != null)
			{
				if (EsIncompleta())
					handler(this, new RecargaIncompletaEventArgs { estaincompleta = true });
				else
					handler(this, new RecargaIncompletaEventArgs { estaincompleta = false });
			}
		}

		public bool EsIncompleta()
		{
			bool ret;
			if (_idpais == 0 || _idoperadora == 0 || _idpaquete == 0 || String.IsNullOrEmpty(_numerorecarga))
				ret = true;
			else
				ret = false;
			return (ret);
		}

		public event EventHandler<EventArgs> RecargaAltaProg;
		protected virtual void OnRecargaAltaProg(EventArgs ea)
		{
			var handler = RecargaAltaProg;
			if (handler != null)
			{
				handler(this, ea);
			}
		}
		public event EventHandler<errRecargaEventArgs> RecargaAltaErr;
		protected virtual void OnRecargaAltaErr(errRecargaEventArgs ea)
		{
			var handler = RecargaAltaErr;
			if (handler != null)
			{
				handler(this, ea);
			}
		}
		public event EventHandler<AltaRecargaCompletedEventArgs> RecargaAlta;
		protected virtual void OnRecargaAlta(AltaRecargaCompletedEventArgs ea)
		{
			var handler = RecargaAlta;
			if (handler != null)
			{
				handler(this, ea);
			}
		}
		public event EventHandler<CargaVesta1aVezCompletedEventArgs> ProcesadoEnVesta1aVez;
		protected virtual void OnProcesadoEnVesta(CargaVesta1aVezCompletedEventArgs ea)
		{
			var handler = ProcesadoEnVesta1aVez;
			if (handler != null)
			{
				handler(this, ea);
			}
		}
		public event EventHandler<CargaVesta2aVezCompletedEventArgs> ProcesadoEnVesta2aVez;
		protected virtual void OnProcesadoEnVesta(CargaVesta2aVezCompletedEventArgs ea)
		{
			var handler = ProcesadoEnVesta2aVez;
			if (handler != null)
			{
				handler(this, ea);
			}
		}
		public event EventHandler<AltaRecargaViaWalletCompletedEventArgs> AltaViaWallet;
		protected virtual void OnAltaViaWallet(AltaRecargaViaWalletCompletedEventArgs ea)
		{
			var handler = AltaViaWallet;
			if (handler != null)
			{
				handler(this, ea);
			}
		}
		public event EventHandler<FingerPrintEventArgs> CargaFinger;
		protected virtual void OnCargaFinger(FingerPrintEventArgs ea)
		{
			var handler = CargaFinger;
			if (handler != null)
			{
				handler(this, ea);
			}
		}

		/*
		public async Task Alta(TarjetasViewModel tvm)
		{
			Recarga r = new Recarga()
			{
				idusuario = _idusuario,
				idpais = _idpais,
				idoperadora = _idoperadora,
				idpaquete = _idpaquete,
				idformapago = _idformapago,
				idtarjeta = (_idformapago == 2) ? _idtarjeta : -1,
				numerorecarga = _numerorecarga,
				contactorecarga = _contactorecarga,
				fecha = DateTime.Now,
				TransactionID = Guid.NewGuid().ToString(),
				err = -1, // -1 es una recarga recien creada q aun no ha sido procesada (pagada)
				os = DependencyService.Get<IContactos>().GetOS()
			};

			if (_idformapago == 1) //monedero
			{
				EventHandler<AltaRecargaViaWalletCompletedEventArgs> atc = null;
				atc = (s, ea) =>
				{
					App.WS.AltaRecargaViaWalletCompleted -= atc;
					if (ea.Error == null && !ea.Cancelled)
					{
						errRecarga err = JsonConvert.DeserializeObject<errRecarga>(ea.Result);
						r.idrecarga = err.idrecarga;
						r.err = err.err;
						r.errVs = err.errVs;
						r.errRs = err.errRs;
						r.rsauthorization = err.tresp.op_authorization;
						r.rstransactionid = err.tresp.transaction_id;
						r.rsrcode = err.tresp.rcode_description;
						r.printdata = err.tresp.printDatam_data;
						App.db.AltaRecarga(r);
						this._err = err;
						if (err.err == 0)
						{
							string json = "";
							json = App.WSc.GetCatalogo(12, "where idusuario = " + App.usr.idusuario.ToString());
							List<SaldoMonedero> sm = JsonConvert.DeserializeObject<List<SaldoMonedero>>(json);
							if (sm.Count() > 0)
								App.db.DescargaSaldo(sm.FirstOrDefault());
						}
						Device.BeginInvokeOnMainThread(() => OnAltaViaWallet(ea));
					}
					else
						Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(null));
					App.WS.Close();
				};
				App.Current.IniciaWS();
				App.WS.AltaRecargaViaWalletCompleted += atc;
				r.ip = DependencyService.Get<IContactos>().GetIP();
				App.WS.AltaRecargaViaWalletAsync(ConvertRecargaToTemp(r));
			}
			else { //tarjeta
				if (_tipotrans == TipoTransaccion.PrimeraVez)
				{
					GetSessionTags ts = await LeeTags(r, null);
					Device.BeginInvokeOnMainThread(() => OnCargaFinger(new FingerPrintEventArgs { url = Constantes.FingerprintAPI, orgid = ts.OrgID, webses = ts.WebSessionID }));
					await Task.Delay(TimeSpan.FromSeconds(10));
					ChargeAccountToTemporaryToken tk = await TempToken(r, null, tvm);
					if (tk.ResponseCode == 0)
					{
						ChargeSale sale = await PermTokenYCarga(r, null, tvm, ts, tk);
						string paymentid = sale.PaymentID;
						if (sale.ResponseCode == 0 && sale.PaymentStatus == 10)
						{
							r.errVs = 0;
							r.err = 0;
							r.PaymentID = sale.PaymentID;
							//------------------------------------------------------
							tvm.NumeroTarjeta = sale.ChargePermanentToken;
							//------------------------------------------------------
							Tarjeta t = new Tarjeta()
							{
								idusuario = _idusuario,
								idpais = tvm.idpais,
								idemisor = tvm.idemisor,
								numero = tvm.NumeroTarjeta,
								titularFN = tvm.TitularFN,
								titularLN = tvm.TitularLN,
								calleynumero = tvm.CalleyNum,
								idestado = tvm.IdEstado,
								idciudad = tvm.IdCiudad,
								codigopostal = tvm.CP
							};
							App.WS.CargaVesta1aVezCompleted += (sender, ea) =>
							{
								if (ea.Error == null && !ea.Cancelled)
								{
									errRecarga err = JsonConvert.DeserializeObject<errRecarga>(ea.Result);
								}
								//	r.idrecarga = e.Result;
								//App.db.AltaRecarga(r);
								//Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(eea));
							};
							App.WS.CargaVesta1aVezAsync(ConvertRecargaToTemp(r), null, ConvertTarjetaToTemp(t));
						}
						else {
							errRecargaEventArgs eea = new errRecargaEventArgs();
							if (sale.AuthResultCode == 1)
								eea.errVs = 3;
							if (sale.AVSResultCode != 4096 && sale.AVSResultCode != 0)
								eea.errVs = 4;
							if (sale.CVNResultCode != 17 && sale.CVNResultCode != 0)
								eea.errVs = 5;
							if (sale.PaymentStatus != 6 && sale.PaymentStatus != 10)
								eea.errVs = 6;
							if (sale.PaymentStatus == 6)
								eea.errVs = 1;
							List<int> salerrs5 = new List<int> { 510, 511, 512, 513, 514 };
							if (sale.ResponseCode == 1 || salerrs5.Contains(sale.ResponseCode) || sale.ResponseCode == 1005)
								eea.errVs = 1;
							if (sale.ResponseCode == 521 || sale.ResponseCode == 1002 || sale.ResponseCode == 1003)
								eea.errVs = 2;
							if (sale.ResponseCode == 1014)
								eea.errVs = 7;
							if (sale.ResponseCode == 1016)
								eea.errVs = 4;
							eea.err = 2;
							r.errVs = eea.errVs;
							r.err = eea.err;
							r.PaymentID = sale.PaymentID;
							errvestadetallado = "Charge= AuthResultCode: " + sale.AuthResultCode + " AVSResultCode: " + sale.AVSResultCode + " CVNResultCode: " + sale.CVNResultCode + " PaymentStatus: " + sale.PaymentStatus + " ResponseCode: " + sale.ResponseCode;
							r.errvestadetallado = errvestadetallado;
							App.WS.AltaRecargaCompleted += (sender, e) =>
							{
								r.idrecarga = e.Result;
								App.db.AltaRecarga(r);
								Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(eea));
							};
							App.WS.AltaRecargaAsync(ConvertRecargaToTemp(r));
						}
					}
					else {
						errRecargaEventArgs eea = new errRecargaEventArgs();
						List<int> errs5 = new List<int> { 510, 511, 512, 513, 514 };
						if (tk.ResponseCode == 1)
							eea.errVs = 1;
						if (errs5.Contains(tk.ResponseCode))
							eea.errVs = 2;
						if (tk.ResponseCode == 521 || tk.ResponseCode == 1002 || tk.ResponseCode == 1003)
							eea.errVs = 2;
						eea.err = 2;
						errvestadetallado = "Token tmp= " + tk.ResponseCode;
						App.WS.AltaRecargaCompleted += (sender, e) =>
						{
							r.idrecarga = e.Result;
							App.db.AltaRecarga(r);
							Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(eea));
						};
						App.WS.AltaRecargaAsync(ConvertRecargaToTemp(r));
					}
				}
			}
		}
		*/

		public void Alta(TarjetasViewModel tvm)
		{
			Recarga r = new Recarga()
			{
				idusuario = _idusuario,
				idpais = _idpais,
				idoperadora = _idoperadora,
				idpaquete = _idpaquete,
				idformapago = _idformapago,
				idtarjeta = (_idformapago == 2) ? _idtarjeta : -1,
				numerorecarga = _numerorecarga,
				contactorecarga = _contactorecarga,
				fecha = DateTime.Now,
				TransactionID = Guid.NewGuid().ToString(),
				err = -1, // -1 es una recarga recien creada q aun no ha sido procesada (pagada)
				os = DependencyService.Get<IContactos>().GetOS()
			};
//--------------------------------------------------------------------------
			if (_idformapago == 1) //monedero
			{
				EventHandler<AltaRecargaViaWalletCompletedEventArgs> atc = null;
				atc = (s, ea) =>
				{
					App.WS.AltaRecargaViaWalletCompleted -= atc;
					//errRecargaEventArgs errea = new errRecargaEventArgs();
					if (ea.Error == null && !ea.Cancelled)
					{
						errRecarga err = JsonConvert.DeserializeObject<errRecarga>(ea.Result);
						r.idrecarga = err.idrecarga;
						r.err = err.err;
						r.errVs = err.errVs;
						r.errRs = err.errRs;
						//r.PaymentID = err.PaymentID;
						r.rsauthorization = err.tresp.op_authorization;
						r.rstransactionid = err.tresp.transaction_id;
						r.rsrcode = err.tresp.rcode_description;
						r.printdata = err.tresp.printDatam_data;
						App.db.AltaRecarga(r);
						this._err = err;
						if (err.err == 0)
						{
							//App.db.CargaMonedero(this.LsPaquetes().Where(x => x.idpaquete == this._idpaquete).FirstOrDefault().monto);
							string json = "";
							json = App.WSc.GetCatalogo(12, "where idusuario = " + App.usr.idusuario.ToString());
							List<SaldoMonedero> sm = JsonConvert.DeserializeObject<List<SaldoMonedero>>(json);
							if (sm.Count() > 0)
								App.db.DescargaSaldo(sm.FirstOrDefault());
						}
						//else
						//	errea.printdata = err.tresp.rcode_description;
						Device.BeginInvokeOnMainThread(() => OnAltaViaWallet(ea));
					}
					else
						Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(null));
					App.WS.Close();
				};
				App.Current.IniciaWS();
				App.WS.AltaRecargaViaWalletCompleted += atc;
				r.ip = DependencyService.Get<IContactos>().GetIP();
				App.WS.AltaRecargaViaWalletAsync(ConvertRecargaToTemp(r));
			}
			else { //2 tarjeta
//--------------------------------------------------------------------------				
				if (_tipotrans == TipoTransaccion.PrimeraVez)
				{
					Tarjeta t = new Tarjeta()
					{
						idusuario = _idusuario,
						idpais = tvm.idpais,
						idemisor = tvm.idemisor,
						permtoken = tvm.NumeroTarjeta,
						Last4 = tvm.Last4,
						titularFN = tvm.TitularFN,
						titularLN = tvm.TitularLN,
						calleynumero = tvm.CalleyNum,
						idestado = tvm.IdEstado,
						idciudad = tvm.IdCiudad,
						codigopostal = tvm.CP,
						expirationMMYY = tvm.ExpirationMMYY,
					};
					EventHandler<CargaVesta1aVezCompletedEventArgs> av = null;
					av = (s, ea) =>
					{
						App.WS.CargaVesta1aVezCompleted -= av;
						if (ea.Error == null && !ea.Cancelled)
						{
							errRecarga err = JsonConvert.DeserializeObject<errRecarga>(ea.Result);
							r.err = err.err;
							r.errVs = err.errVs;
							r.errRs = err.errRs;
							r.PaymentID = err.PaymentID;
							r.rsauthorization = err.tresp.op_authorization;
							r.rstransactionid = err.tresp.transaction_id;
							r.rsrcode = err.tresp.rcode_description;
							r.idtarjeta = err.idtarjeta;
							App.db.AltaRecarga(r);
							if (err.idtarjeta > 0)
							{
								t.idtarjeta = err.idtarjeta;
								App.db.AltaTarjeta(t);
							}
							this._err = err;
							Device.BeginInvokeOnMainThread(() => OnProcesadoEnVesta(ea));
						}
						else
							Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(null));
						App.WS.Close();
					};
					EventHandler<AltaRecargaCompletedEventArgs> atc = null;
					atc = async (s, ea) =>
					{
						App.WS.AltaRecargaCompleted -= atc;
						if (ea.Error == null && !ea.Cancelled)
						{
							tags = JsonConvert.DeserializeObject<GetSessionTags>(ea.Result);
							Device.BeginInvokeOnMainThread(() => OnCargaFinger(new FingerPrintEventArgs { url = Constantes.FingerprintAPI, orgid = tags.OrgID, webses = tags.WebSessionID }));
							await Task.Delay(TimeSpan.FromSeconds(10));
							ChargeAccountToTemporaryToken tk = await TempToken(r, null, tvm);
							if (tk.ResponseCode == 0)
							{
								r.idrecarga = tags.idrecarga;
								App.db.AltaRecarga(r);
								t.permtoken = tk.ChargeAccountNumberToken;
								t.Last4 = tk.PaymentDeviceLast4;
								App.WS.CargaVesta1aVezCompleted += av;
								App.WS.CargaVesta1aVezAsync(ConvertRecargaToTemp(r), null, tags.WebSessionID, ConvertTarjetaToTemp(t), tvm.Cvc);
							}
							else
								Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(null));
						}
						else
							Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(null));
					};
					App.Current.IniciaWS();
					App.WS.AltaRecargaCompleted += atc;
					r.ip = DependencyService.Get<IContactos>().GetIP();
					App.WS.AltaRecargaAsync(ConvertRecargaToTemp(r));
				} // si primera vez
//--------------------------------------------------------------------------
				if (_tipotrans == TipoTransaccion.SegundaVez)
				{
					EventHandler<CargaVesta2aVezCompletedEventArgs> av2 = null;
					av2 = (s, ea) =>
					{
						App.WS.CargaVesta2aVezCompleted -= av2;
						if (ea.Error == null && !ea.Cancelled)
						{
							errRecarga err = JsonConvert.DeserializeObject<errRecarga>(ea.Result);
							r.err = err.err;
							r.errVs = err.errVs;
							r.errRs = err.errRs;
							r.PaymentID = err.PaymentID;
							r.rsauthorization = err.tresp.op_authorization;
							r.rstransactionid = err.tresp.transaction_id;
							r.rsrcode = err.tresp.rcode_description;
							r.idrecarga = err.idrecarga;
							r.idtarjeta = err.idtarjeta;
							App.db.AltaRecarga(r);
							this._err = err;
							Device.BeginInvokeOnMainThread(() => OnProcesadoEnVesta(ea));
						}
						else
							Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(null));
						App.WS.Close();
					};
					EventHandler<AltaRecargaCompletedEventArgs> atc = null;
					atc = async (s, ea) =>
					{
						App.WS.AltaRecargaCompleted -= atc;
						if (ea.Error == null && !ea.Cancelled)
						{
							tags = JsonConvert.DeserializeObject<GetSessionTags>(ea.Result);
							r.idrecarga = tags.idrecarga;
							Device.BeginInvokeOnMainThread(() => OnCargaFinger(new FingerPrintEventArgs { url = Constantes.FingerprintAPI, orgid = tags.OrgID, webses = tags.WebSessionID }));
							await Task.Delay(TimeSpan.FromSeconds(10));

							App.WS.CargaVesta2aVezCompleted += av2;
							App.WS.CargaVesta2aVezAsync(ConvertRecargaToTemp(r), null, tags.WebSessionID);
						}
						else
							Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(null));
					};
					App.Current.IniciaWS();
					App.WS.AltaRecargaCompleted += atc;
					r.ip = DependencyService.Get<IContactos>().GetIP();
					App.WS.AltaRecargaAsync(ConvertRecargaToTemp(r));
				} // segunda vez
			} // si tarjeta
		}

		public void AltaMonedero(TarjetasViewModel tvm)
		{
			RecargaMonedero rm = new RecargaMonedero()
			{
				idusuario = _idusuario,
				idtarjeta = (_idformapago == 2) ? _idtarjeta : -1,
				monto = _montorecargamonedero,
				fecha = DateTime.Now,
				TransactionID = Guid.NewGuid().ToString(),
				os = DependencyService.Get<IContactos>().GetOS(),
				err = -1 // -1 es una recarga recien creada q aun no ha sido procesada (pagada)
			};
			if (_tipotrans == TipoTransaccion.PrimeraVez)
			{
				Tarjeta t = new Tarjeta()
				{
					idusuario = _idusuario,
					idpais = tvm.idpais,
					idemisor = tvm.idemisor,
					permtoken = tvm.NumeroTarjeta,
					Last4 = tvm.Last4,
					titularFN = tvm.TitularFN,
					titularLN = tvm.TitularLN,
					calleynumero = tvm.CalleyNum,
					idestado = tvm.IdEstado,
					idciudad = tvm.IdCiudad,
					codigopostal = tvm.CP,
					expirationMMYY = tvm.ExpirationMMYY,
				};
				EventHandler<CargaVesta1aVezCompletedEventArgs> av = null;
				av = (s, ea) =>
				{
					App.WS.CargaVesta1aVezCompleted -= av;
					if (ea.Error == null && !ea.Cancelled)
					{
						errRecarga err = JsonConvert.DeserializeObject<errRecarga>(ea.Result);
						rm.err = err.err;
						rm.errVs = err.errVs;
						rm.PaymentID = err.PaymentID;
						rm.idtarjeta = err.idtarjeta;
						if (err.idtarjeta > 0)
						{
							t.idtarjeta = err.idtarjeta;
							App.db.AltaTarjeta(t);
						}
						if (err.err == 0)
						{
							App.db.AltaRecargaMonedero(rm, true);
							string json = "";
							json = App.WSc.GetCatalogo(12, "where idusuario = " + App.usr.idusuario.ToString());
							List<SaldoMonedero> sm = JsonConvert.DeserializeObject<List<SaldoMonedero>>(json);
							if (sm.Count() > 0)
								App.db.DescargaSaldo(sm.FirstOrDefault());
							//_montorecargamonedero = App.db.SelSaldo().saldo;
						}
						else
							App.db.AltaRecargaMonedero(rm, false);
						this._err = err;
						Device.BeginInvokeOnMainThread(() => OnProcesadoEnVesta(ea));
					}
					else
						Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(null));
					App.WS.Close();
				};
				EventHandler<AltaRecargaMonederoCompletedEventArgs> atc = null;
				atc = async (s, ea) =>
				{
					App.WS.AltaRecargaMonederoCompleted -= atc;
					if (ea.Error == null && !ea.Cancelled)
					{
						tags = JsonConvert.DeserializeObject<GetSessionTags>(ea.Result);
						Device.BeginInvokeOnMainThread(() => OnCargaFinger(new FingerPrintEventArgs { url = Constantes.FingerprintAPI, orgid = tags.OrgID, webses = tags.WebSessionID }));
						await Task.Delay(TimeSpan.FromSeconds(10));
						ChargeAccountToTemporaryToken tk = await TempToken(null, rm, tvm);
						if (tk.ResponseCode == 0)
						{
							rm.idrecargamonedero = tags.idrecarga; //tags regresa idr o idr monedero segun sea el caso, en el mismo campo "idrecarga"
							App.db.AltaRecargaMonedero(rm, false);
							t.permtoken = tk.ChargeAccountNumberToken;
							t.Last4 = tk.PaymentDeviceLast4;
							App.WS.CargaVesta1aVezCompleted += av;
							App.WS.CargaVesta1aVezAsync(null, ConvertRecargaMonToTemp(rm), tags.WebSessionID, ConvertTarjetaToTemp(t), tvm.Cvc);
						}
						else
							Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(null));
					}
					else {
						if (!String.IsNullOrEmpty(ea.Error.Message))
							Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(new errRecargaEventArgs() { msg = ea.Error.Message }));
						else
							Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(null));
					}
				};
				App.Current.IniciaWS();
				App.WS.AltaRecargaMonederoCompleted += atc;
				rm.ip = DependencyService.Get<IContactos>().GetIP();
				App.WS.AltaRecargaMonederoAsync(ConvertRecargaMonToTemp(rm));
			} // si primera vez
			if (_tipotrans == TipoTransaccion.SegundaVez)
			{
				EventHandler<CargaVesta2aVezCompletedEventArgs> av2 = null;
				av2 = (s, ea) =>
				{
					App.WS.CargaVesta2aVezCompleted -= av2;
					if (ea.Error == null && !ea.Cancelled)
					{
						errRecarga err = JsonConvert.DeserializeObject<errRecarga>(ea.Result);
						rm.err = err.err;
						rm.errVs = err.errVs;
						rm.PaymentID = err.PaymentID;
						rm.idrecargamonedero = err.idrecarga;
						rm.idtarjeta = err.idtarjeta;
						if (err.err == 0)
						{
							App.db.AltaRecargaMonedero(rm, true);
							string json = "";
							json = App.WSc.GetCatalogo(12, "where idusuario = " + App.usr.idusuario.ToString());
							List<SaldoMonedero> sm = JsonConvert.DeserializeObject<List<SaldoMonedero>>(json);
							if (sm.Count() > 0)
								App.db.DescargaSaldo(sm.FirstOrDefault());
							//_montorecargamonedero = App.db.SelSaldo().saldo;
						}
						else
							App.db.AltaRecargaMonedero(rm, false);
						this._err = err;
						Device.BeginInvokeOnMainThread(() => OnProcesadoEnVesta(ea));
					}
					else {
						if (!String.IsNullOrEmpty(ea.Error.Message))
							Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(new errRecargaEventArgs() { msg = ea.Error.Message }));
						else
							Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(null));
					}
					App.WS.Close();
				};
				App.Current.IniciaWS();
				rm.ip = DependencyService.Get<IContactos>().GetIP();
				App.WS.CargaVesta2aVezCompleted += av2;
				App.WS.CargaVesta2aVezAsync(null, ConvertRecargaMonToTemp(rm), "");
			} // si segunda vez
		}

		private string GeneraRisk(Recarga r, RecargaMonedero rm, TarjetasViewModel tvm, catPaquete p, catPais ps, catEstado ed, catMunicipio m, Usuario u)
		{
			string xml = "";
			TransactionTypeChannelChannelCode ch = TransactionTypeChannelChannelCode.Web;
			ch = TransactionTypeChannelChannelCode.Mobile;
			var data = new RiskInformation
			{
				version = RiskInformationVersion.Item15,
				Transaction = new TransactionType
				{
					IsB2BTransaction = false,
					IsOneTimePayment = true,
					Account = new TransactionTypeAccount
					{
						AccountID = App.usr.idusuario.ToString(), //ids de usuario en AC
						Email = u.email,
						FirstName = tvm.TitularFN,
						LastName = tvm.TitularLN,
						Address = new AddressType[] { new AddressType
						{
							AddressLine1 = (tvm.CalleyNum.Length > 30 ? tvm.CalleyNum.Substring(0, 30) : tvm.CalleyNum),
							City = m.municipio,
							State = ed.estado,
							PostalCode = tvm.CP,
							CountryCode = ps.codigopais
						}
						},
						MobilePhoneNumber = (ps.codigotel + u.numerocontacto).PadLeft(15, '0'),
						CreatedDTM = u.fechaalta.ToString("yyyy-MM-ddTHH:mm:ssZ"), //fecha de alta de usuario
						HasNegativeHistory = false,
					},
					Authentication = new TransactionTypeAuthentication
					{
						IsAuthenticated = true,
						AuthenticationMethod = TransactionTypeAuthenticationAuthenticationMethod.PIN,
						AuthenticationMethodSpecified = true,
						LoginID = u.email
					},
					Channel = new TransactionTypeChannel
					{
						ChannelCode = ch,
						IPAddress = (r == null ? rm.ip : r.ip)
					},
					TimeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
				},
				Billing = new BillingType
				{
					Email = u.email,
					ContactPhoneNumber = (ps.codigotel + u.numerocontacto).PadLeft(15, '0'),
					PaymentDeviceCreatedDtm = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
					PaymentDeviceCreatedDtmSpecified = true,
					HasNegativeHistory = false,
				},
				ShoppingCartItems = new ShoppingCartItemsType
				{
					count = "1",
					ShoppingCartItem = new ShoppingCartItemsTypeShoppingCartItem[]
					{
						new ShoppingCartItemsTypeShoppingCartItem
						{
							DeliveryMethod = ShoppingCartItemsTypeShoppingCartItemDeliveryMethod.Reload,
							DeliveryRecipient = new ShoppingCartItemsTypeShoppingCartItemDeliveryRecipient
							{
								FirstName = (r == null ? u.name : r.contactorecarga),
								PhoneNumber = (ps.codigotel + (r == null ? u.numerocontacto : r.numerorecarga)).PadLeft(15, '0'),
								Address = new AddressType
								{
									AddressLine1 = (tvm.CalleyNum.Length > 30 ? tvm.CalleyNum.Substring(0, 30) : tvm.CalleyNum),
									City = m.municipio,
									State = ed.estado,
									PostalCode = tvm.CP,
									CountryCode = ps.codigopais
								},
								StoredValueAccountID = (ps.codigotel + (r == null ? u.numerocontacto : r.numerorecarga)).PadLeft(15, '0'),
							},
							LineItems = new LineItemsType
							{
								count = "1",
								LineItem = new LineItemsTypeLineItem[]
								{
									new LineItemsTypeLineItem
									{
										ProductCode = (r == null ? "CORRS001" : p.sku),
										ProductDescription = (r == null ? "Recarga de monedero" : p.paquete),
										UnitPrice = (r == null ? rm.monto : p.monto),
										Quantity = "1"
									}
								}
							},
						}
					},
				},
			};
			var serializer = new XmlSerializer(typeof(RiskInformation));
			using (StringWriterWithEncoding textWriter = new StringWriterWithEncoding(System.Text.Encoding.UTF8))
			{
				XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
				ns.Add("", "");
				serializer.Serialize(textWriter, data, ns);
				xml = textWriter.ToString();
			}
			return (xml);
		}

		private async Task<string> GeneraDevolucion(Recarga r, TarjetasViewModel tvm)
		{
			string errorvestadetallado = "";
			catPaquete cp = LsPaquetes(_idoperadora).Where(x => x.idpaquete == _idpaquete).FirstOrDefault();
			using (var client = new HttpClient(new HttpClientHandler()))
			{
				ReversePayment rev = new ReversePayment();
				string json = "";
				var values2 = new Dictionary<string, string>
					{
						{ "AccountName", Settings.APIUsername },
                        //{ "ChargeAccountNumber", t.numero },
                        //{ "ChargeAccountNumberIndicator", "1" },
						{ "ChargeExpirationMMYY", tvm.ExpiraMM + tvm.ExpiraYY },
						{ "MerchantRoutingID", Settings.MerchantRoutingID },
						{ "PartnerCustomerKey", App.usr.idusuario.ToString() },
						{ "Password", Settings.APIPassword },
						{ "PaymentID", r.PaymentID },
						{ "RefundAmount", cp.monto.ToString() },
                        //{ "ReportingInformation", "" },
                        { "TransactionID", r.TransactionID },
					};
				json = JsonConvert.SerializeObject(values2);
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				var response = await client.PostAsync(Constantes.PaymentAPI + "/ReversePayment", new StringContent(json, Encoding.UTF8, "application/json"));
				var responseString = await response.Content.ReadAsStringAsync();
				rev = JsonConvert.DeserializeObject<ReversePayment>(responseString);
				errorvestadetallado = "ReversePayment= PaymentStatus: " + rev.PaymentStatus.ToString() + " ResponseCode: " + rev.ResponseCode.ToString() + " ResponseText: " + rev.ResponseText + " ReversalAction:" + rev.ReversalAction.ToString();
			}
			return (errorvestadetallado);
		}

		private async Task<ChargeSale> PermTokenYCarga(Recarga r, RecargaMonedero rm, TarjetasViewModel tvm, GetSessionTags tags, ChargeAccountToTemporaryToken token)
		{
			ChargeSale sale = new ChargeSale();
			string riskxml = "";
			catPaquete p = null;
			catPais ps = lsPais().Where(x => x.idpais == App.usr.idpais).FirstOrDefault();
			catEstado ed = lsEstados(App.usr.idpais).Where(x => x.idestado == tvm.IdEstado).FirstOrDefault();
			catMunicipio m = lsMunicipios(tvm.IdEstado).Where(x => x.idmunicipio == tvm.IdCiudad).FirstOrDefault();
			if (rm == null && r != null)
				p = LsPaquetes(_idoperadora).Where(x => x.idpaquete == _idpaquete).FirstOrDefault();
			try
			{
				using (var client = new HttpClient(new HttpClientHandler()))
				{
					string json = "";
					riskxml = GeneraRisk(r, rm, tvm, p, ps, ed, m, App.usr);
					var values3 = new Dictionary<string, string>
					{
						{ "AccountName", Settings.APIUsername },
						{ "CardHolderAddressLine1", (tvm.CalleyNum.Length > 30 ? tvm.CalleyNum.Substring(0, 30) : tvm.CalleyNum) },
						{ "CardHolderCity", m.municipio },
						{ "CardHolderCountryCode", ps.codigopais },
						{ "CardHolderFirstName", tvm.TitularFN },
						{ "CardHolderLastName", tvm.TitularLN },
						{ "CardHolderPostalCode", tvm.CP },
						{ "CardHolderRegion", ed.estado },
						{ "ChargeAccountNumber", token.ChargeAccountNumberToken },
						{ "ChargeAccountNumberIndicator", "2" }, //2 token temporal 1 tarjeta
						{ "ChargeAmount", (rm == null && r != null ? p.monto.ToString() : rm.monto.ToString()) },
						{ "ChargeExpirationMMYY", tvm.ExpiraMM.ToString() + tvm.ExpiraYY.ToString() },
						{ "ChargeSource", "WEB" },
						{ "MerchantRoutingID", Settings.MerchantRoutingID },
						{ "Password", Settings.APIPassword },
						{ "TransactionID", (r == null ? rm.TransactionID : r.TransactionID) },
						{ "RiskInformation", riskxml },
						{ "WebSessionID", tags.WebSessionID },
						{ "ChargeCVN", tvm.Cvc },
						{ "StoreCard", "true" },
					};
					json = JsonConvert.SerializeObject(values3);
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					var response2 = await client.PostAsync(Constantes.PaymentAPI + "/ChargeSale", new StringContent(json, Encoding.UTF8, "application/json"));
					var responseString2 = await response2.Content.ReadAsStringAsync();
					sale = JsonConvert.DeserializeObject<ChargeSale>(responseString2);
				}
			}
			catch (Exception ex)
			{
			}
			return (sale);
		}

		private async Task<ChargeAccountToTemporaryToken> TempToken(Recarga r, RecargaMonedero rm, TarjetasViewModel tvm)
		{
			HttpClient client = null;
			ChargeAccountToTemporaryToken token = new ChargeAccountToTemporaryToken();
			try
			{
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
#if __IOS__
				client = new HttpClient(new System.Net.Http.NSUrlSessionHandler());
#endif
#if __ANDROID__
				if (((int)Android.OS.Build.VERSION.SdkInt) >= 21) //android 5.0
					client = new HttpClient(new Xamarin.Android.Net.AndroidClientHandler());
				else //android 4.4
					client = new HttpClient(new ModernHttpClient.NativeMessageHandler());
				//using (var client = new HttpClient(new HttpClientHandler()))
#endif
				string json = "";
				var values2 = new Dictionary<string, string>
					{
						{ "AccountName", Settings.APIUsername },
						{ "ChargeAccountNumber", tvm.NumeroTarjeta }
					};
				json = JsonConvert.SerializeObject(values2);
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				var response = await client.PostAsync(Constantes.TokenizationAPI + "/ChargeAccountToTemporaryToken", new StringContent(json, Encoding.UTF8, "application/json"));
				var responseString = await response.Content.ReadAsStringAsync();
				token = JsonConvert.DeserializeObject<ChargeAccountToTemporaryToken>(responseString);
			}
			catch (Exception ex)
			{

			}
			return (token);
		}

		private async Task<GetSessionTags> LeeTags(Recarga r, RecargaMonedero rm)
		{
			GetSessionTags ts = new GetSessionTags();
			try
			{
				//using (var client = new HttpClient(new System.Net.Http.NSUrlSessionHandler()))
				//{
				//	ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
				//	var values1 = new Dictionary<string, string>{
				//		{ "AccountName", Settings.APIUsername },
				//		{ "Password", Settings.APIPassword },
				//		{ "TransactionID", (r == null ? rm.TransactionID : r.TransactionID) } //UUID?
				//	};
				//	string json = JsonConvert.SerializeObject(values1);
				//	client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				//	var response = await client.PostAsync(Constantes.PaymentAPI + "/GetSessionTags", new StringContent(json, Encoding.UTF8, "application/json"));
				//	var responseString = await response.Content.ReadAsStringAsync();
				//	ts = JsonConvert.DeserializeObject<GetSessionTags>(responseString);
				//}


			}
			catch (Exception ex)
			{

			}
			return (ts);
		}

		private static bool CertificateValidationCallBack(
		 object sender,
		 System.Security.Cryptography.X509Certificates.X509Certificate certificate,
		 System.Security.Cryptography.X509Certificates.X509Chain chain,
		 System.Net.Security.SslPolicyErrors sslPolicyErrors)
		{
			// If the certificate is a valid, signed certificate, return true.
			if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
			{
				return true;
			}

			// If there are errors in the certificate chain, look at each error to determine the cause.
			if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
			{
				if (chain != null && chain.ChainStatus != null)
				{
					foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
					{
						if ((certificate.Subject == certificate.Issuer) &&
						   (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
						{
							// Self-signed certificates with an untrusted root are valid. 
							continue;
						}
						else
						{
							if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
							{
								// If there are any other errors in the certificate chain, the certificate is invalid,
								// so the method returns false.
								return false;
							}
						}
					}
				}

				// When processing reaches this line, the only errors in the certificate chain are 
				// untrusted root errors for self-signed certificates. These certificates are valid
				// for default Exchange server installations, so return true.
				return true;
			}
			else
			{
				// In all other cases, return false.
				return false;
			}
		}

		private async Task LlamaFingerpripri(GetSessionTags tags)
		{
			try
			{
				string resultado = "";
				GetSessionTags t = tags;
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
			| SecurityProtocolType.Tls11
			| SecurityProtocolType.Tls12
			| SecurityProtocolType.Ssl3;
				var uri = new Uri("http://192.168.15.113:8888");
				//using (var hc = new HttpClient(new HttpClientHandler() { Proxy = new Proxy(uri), UseProxy = true }))
#if __IOS__
				using (var hc = new HttpClient(new HttpClientHandler()))
#endif
#if __ANDROID__
				using (var hc = new HttpClient(new Xamarin.Android.Net.AndroidClientHandler()))
#endif
				{
					hc.BaseAddress = new Uri(Constantes.FingerprintAPI);
					//var content = new FormUrlEncodedContent(new[]
					//{
					//	new KeyValuePair<string, string>("org_id", t.OrgID),
					//	new KeyValuePair<string, string>("session_id", t.WebSessionID),
					//	new KeyValuePair<string, string>("m", "1"),
					//});
					var url = "ThreatMetrixUIRedirector/fp/clear.png?org_id=" + t.OrgID + "&session_id=" + t.WebSessionID + "&m=1";
					var request = new HttpRequestMessage(HttpMethod.Post, url);
					var response = await hc.SendAsync(request).ConfigureAwait(true);
					var res = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(true);
					//await Task.Delay(TimeSpan.FromSeconds(2));

					url = "ThreatMetrixUIRedirector/fp/clear.png?org_id=" + t.OrgID + "&session_id=" + t.WebSessionID + "&m=2";
					request = new HttpRequestMessage(HttpMethod.Post, url);
					response = await hc.SendAsync(request).ConfigureAwait(true);
					var res2 = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(true);
					//await Task.Delay(TimeSpan.FromSeconds(2));

					url = "ThreatMetrixUIRedirector/fp/check.js?org_id=" + t.OrgID + "&session_id=" + t.WebSessionID;
					request = new HttpRequestMessage(HttpMethod.Post, url);
					response = await hc.SendAsync(request).ConfigureAwait(true);
					var str = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
					//await Task.Delay(TimeSpan.FromSeconds(2));

					url = "ThreatMetrixUIRedirector/fp/fp.swf?org_id=" + t.OrgID + "&session_id=" + t.WebSessionID;
					request = new HttpRequestMessage(HttpMethod.Post, url);
					response = await hc.SendAsync(request).ConfigureAwait(true);
					var res3 = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(true);
					//await Task.Delay(TimeSpan.FromSeconds(2));

					//resultado = String.Format("Lon 1= {0} Lon 2= {1} lon3= {3} response={4} uri={5} JS= {2} ", res.Length, res2.Length, str, res3.Length, response.StatusCode.ToString(), response.RequestMessage.RequestUri);
					//Page p = new Page();
					//Device.BeginInvokeOnMainThread(() => p.DisplayAlert("info", resultado, "OK"));

					/*content = null;
					content = new FormUrlEncodedContent(new[]
					{
						new KeyValuePair<string, string>("org_id", t.OrgID),
						new KeyValuePair<string, string>("session_id", t.WebSessionID),
						new KeyValuePair<string, string>("m", "2"),
					});
					var result = hc.PostAsync("/fp/clear.png", content).Result;

					content = null;
					content = new FormUrlEncodedContent(new[]
					{
						new KeyValuePair<string, string>("org_id", t.OrgID),
						new KeyValuePair<string, string>("session_id", t.WebSessionID),
					});
					result = hc.PostAsync("/fp/check.js", content).Result;

					content = null;
					content = new FormUrlEncodedContent(new[]
					{
						new KeyValuePair<string, string>("org_id", t.OrgID),
						new KeyValuePair<string, string>("session_id", t.WebSessionID),
					});
					result = hc.PostAsync("/fp/fp.swf", content).Result;*/
				}


				/*ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
				using (WebClient wclient = new WebClient())
				{
					wclient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
					var reqparm = new System.Collections.Specialized.NameValueCollection();
					reqparm.Add("org_id", tags.OrgID);
					reqparm.Add("session_id", tags.WebSessionID);
					reqparm.Add("m", "1");
					byte[] responsebytes = wclient.UploadValues(Constantes.FingerprintAPI + "/fp/clear.png", "POST", reqparm);
					//string responsebody = Encoding.UTF8.GetString(responsebytes);

					reqparm.Remove("m");
					reqparm.Add("m", "2");
					responsebytes = wclient.UploadValues(Constantes.FingerprintAPI + "/fp/clear.png", "POST", reqparm);

					reqparm.Remove("m");

					responsebytes = wclient.DownloadData(Constantes.FingerprintAPI + "/fp/check.js?org_id=" + tags.OrgID + "&session_id=" + tags.WebSessionID);

					responsebytes = wclient.UploadValues(Constantes.FingerprintAPI + "/fp/fp.swf", "POST", reqparm);
				}*/
			}
			catch (Exception ex)
			{
				Page p = new Page();
				Device.BeginInvokeOnMainThread(async () => await p.DisplayAlert("info", ex.Message + " " + ex.InnerException.StackTrace, "OK"));
			}
		}

		private tempuri.org.Tarjeta ConvertTarjetaToTemp(Tarjeta t)
		{
			tempuri.org.Tarjeta tot = new tempuri.org.Tarjeta
			{
				idusuario = t.idusuario,
				idpais = t.idpais,
				idemisor = t.idemisor,
				permtoken = t.permtoken,
				Last4 = t.Last4,
				titularFN = t.titularFN,
				titularLN = t.titularLN,
				calleynumero = t.calleynumero,
				idestado = t.idestado,
				idciudad = t.idciudad,
				codigopostal = t.codigopostal,
				expirationMMYY = t.expirationMMYY,
			};
			return (tot);
		}

		private tempuri.org.RecargaMonedero ConvertRecargaMonToTemp(RecargaMonedero rm)
		{
			tempuri.org.RecargaMonedero torm = new tempuri.org.RecargaMonedero()
			{
				ip = rm.ip,
				idusuario = rm.idusuario,
				idrecargamonedero = rm.idrecargamonedero,
				idtarjeta = rm.idtarjeta,
				monto = rm.monto,
				fecha = rm.fecha,
				TransactionID = rm.TransactionID,
				err = rm.err,
				os = rm.os,
			};
			return (torm);
		}

		private tempuri.org.Recarga ConvertRecargaToTemp(Recarga r)
		{
			tempuri.org.Recarga tor = new tempuri.org.Recarga()
			{
				ip = r.ip,
				idusuario = r.idusuario,
				idrecarga = r.idrecarga,
				idpais = r.idpais,
				idoperadora = r.idoperadora,
				idpaquete = r.idpaquete,
				idformapago = r.idformapago,
				idtarjeta = r.idtarjeta,
				numerorecarga = r.numerorecarga,
				contactorecarga = r.contactorecarga,
				fecha = r.fecha,
				TransactionID = r.TransactionID,
				err = r.err,
				os = r.os,
			};
			return (tor);
		}

		private tempuri.org.RecargaProg ConvertRecargaProgToTemp(RecargaProg r)
		{
			tempuri.org.RecargaProg tor = new tempuri.org.RecargaProg()
			{
				idusuario = r.idusuario,
				idrecarga = r.idrecarga,
				idpais = r.idpais,
				idoperadora = r.idoperadora,
				idpaquete = r.idpaquete,
				idformapago = r.idformapago,
				idtarjeta = r.idtarjeta,
				numerorecarga = r.numerorecarga,
				contactorecarga = r.contactorecarga,
				diasmes = r.diasmes,
			};
			return (tor);
		}

        public void AltaProg()
        {
            RecargaProg r = new RecargaProg()
            {
				idusuario = _idusuario,
				idpais = _idpais,
                idoperadora = _idoperadora,
                idpaquete = _idpaquete,
                idformapago = _idformapago,
                idtarjeta = (_idformapago == 2) ? _idtarjeta : -1,
                numerorecarga = _numerorecarga,
                contactorecarga = _contactorecarga,
                diasmes = _diasrecarga,
            };

			EventHandler<AltaRecargaProgCompletedEventArgs> atc = null;
			atc = (s, ea) =>
			{
				App.WS.AltaRecargaProgCompleted -= atc;
				if (ea.Error == null && !ea.Cancelled)
				{
					r.idrecarga = ea.Result;
					App.db.AltaRecargaProg(r);
					Device.BeginInvokeOnMainThread(() => OnRecargaAltaProg(null));
				}
				else
					Device.BeginInvokeOnMainThread(() => OnRecargaAltaErr(null));
			};
			App.Current.IniciaWS();
			App.WS.AltaRecargaProgCompleted += atc;
			App.WS.AltaRecargaProgAsync(ConvertRecargaProgToTemp(r));
        }

        public RecargasViewModel()
        {
            ReadOnly = false;
            EdicionOnly = false;
            _opcpais = new opcMenu(1, 0, "País", "Seleccione un país", new FileImageSource() { File = "globe.png" });
            if (App.usr.idpais != 0)
            {
                _opcpais.idOpcion = App.usr.idpais;
                _opcpais.Desc = lsPais().Where(x => x.idpais == App.usr.idpais).FirstOrDefault().pais;
            }
            _opcoperadora = new opcMenu(2, 0, "Compañía", "Seleccione una compañía", new FileImageSource() { File = "radiotower.png" });
			_opcnumero = new opcMenu(3, 0, "Número", "Seleccione un número", new FileImageSource() { File = "phone.png" });
			_opcpaquete = new opcMenu(4, 0, "Monto", "Seleccione un monto", new FileImageSource() { File = "money.png" });
			_opccupon = new opcMenu(5, 0, "Cupon", "Seleccione un cupon", new FileImageSource() { File = "shop.png" });
			_opclimpiar = new opcMenu(6, 0, "Limpiar", "Nueva recarga (limpiar campos)", new FileImageSource() { File = "backward.png" });

			App.db.DelRecargasFrecuentes();
			RecargaFrecuente rf = new RecargaFrecuente() { idpais = 1, idoperadora = 1, idpaquete = 764, numerorecarga = "7771254671", contactorecarga = "Tia Yuye", fecha = new DateTime(2016, 6, 18), numRecargas = 2 };
            App.db.AltaRecargasFrecuentes(rf);
            rf = new RecargaFrecuente() { idpais = 1, idoperadora = 2, idpaquete = 727, numerorecarga = "5548934201", contactorecarga = "Paty Meza", fecha = new DateTime(2016, 6, 18), numRecargas = 5 };
            App.db.AltaRecargasFrecuentes(rf);
        }
		public void Refresh()
		{
			_idoperadora = 0;
			_idpaquete = 0;
			_numerorecarga = "";
			_contactorecarga = "";
			_idformapago = 0;
			_idtarjeta = 0;

			_opcpais = new opcMenu(1, 0, "País", "Seleccione un país", new FileImageSource() { File = "globe.png" });
			if (App.usr.idpais != 0)
			{
				_opcpais.idOpcion = App.usr.idpais;
				_opcpais.Desc = lsPais().Where(x => x.idpais == App.usr.idpais).FirstOrDefault().pais;
			}
			_opcoperadora = new opcMenu(2, 0, "Compañía", "Seleccione una compañía", new FileImageSource() { File = "radiotower.png" });
			_opcnumero = new opcMenu(3, 0, "Número", "Seleccione un número", new FileImageSource() { File = "phone.png" });
			_opcpaquete = new opcMenu(4, 0, "Monto", "Seleccione un monto", new FileImageSource() { File = "money.png" });
			_opccupon = new opcMenu(5, 0, "Cupon", "Seleccione un cupon", new FileImageSource() { File = "shop.png" });
			_opclimpiar = new opcMenu(6, 0, "Limpiar", "Nueva recarga (limpiar campos)", new FileImageSource() { File = "backward.png" });

			NotifyPropertyChanged("IdTarjeta");
			NotifyPropertyChanged("idFormaPago");
			NotifyPropertyChanged("idpaquete");
			NotifyPropertyChanged("idoperadora");
			NotifyPropertyChanged("NumeroRecarga");
			NotifyPropertyChanged("ContactoRecarga");
			NotifyPropertyChanged("opcOperadora");
			NotifyPropertyChanged("opcPaquete");
		}
        public List<Recarga> SelRecargas()
        {
            return App.db.SelRecargas();
        }
		public List<RecargaMonedero> SelRecargasWallet()
		{
			return App.db.SelRecargasWallet();
		}
        public List<RecargaFrecuente> SelRecargasFrecuentes()
        {
            return App.db.SelRecargasFrecuentes();
        }
        public List<RecargaProg> SelRecargasProg()
        {
            return App.db.SelRecargasProg();
        }
        public void DelRecargaFrecuente(RecargaProg rf)
        {
            App.db.DelRecargaFrecuente(rf);
        }
    }

	public class Proxy : System.Net.IWebProxy
	{
		public System.Net.ICredentials Credentials
		{
			get;
			set;
		}

		private readonly Uri _proxyUri;

		public Proxy(Uri proxyUri)
		{
			_proxyUri = proxyUri;
		}

		public Uri GetProxy(Uri destination)
		{
			return _proxyUri;
		}

		public bool IsBypassed(Uri host)
		{
			return false;
		}
	}

	public sealed class StringWriterWithEncoding : StringWriter
	{
		private readonly Encoding encoding;
		public StringWriterWithEncoding(Encoding encoding)
		{
			this.encoding = encoding;
		}
		public override Encoding Encoding
		{
			get { return encoding; }
		}
	}

	public class FingerPrintEventArgs : EventArgs
	{
		public string url { get; set; }
		public string orgid { get; set; }
		public string webses { get; set; }
	}
	public class RecargaIncompletaEventArgs : EventArgs
	{
		public bool estaincompleta { get; set; }
	}
	public class errRecargaEventArgs : EventArgs
	{
		public int err { get; set; }
		public int errVs { get; set; }
		public int errRs { get; set; }
		public string msg { get; set; }
	}
	public class AdditionalAmountsType
	{
		public decimal Amount { get; set; }
		public string Type { get; set; }
	}
	public class ChargeSale
	{
		public AdditionalAmountsType AdditionalAmounts { get; set; }
		public string AcquirerApprovalCode { get; set; }
		public string AcquirerAVSResponseCode { get; set; }
		public string AcquirerCVNResponseCode { get; set; }
		public string AcquirerResponseCode { get; set; }
		public string AcquirerResponseCodeText { get; set; }
		public decimal AuthorizedAmount { get; set; }
		public int AuthResultCode { get; set; }
		public int AVSResultCode { get; set; }
		public int ChargeAccountFirst6 { get; set; }
		public int ChargeAccountLast4 { get; set; }
		public string ChargePermanentToken { get; set; }
		public int CVNResultCode { get; set; }
		public string PartnerCustomerKey { get; set; }
		public int PartnerDenialReasonCode { get; set; }
		public string PaymentAcquirerName { get; set; }
		public int PaymentDeviceTypeCD { get; set; }
		public string PaymentID { get; set; }
		public int PaymentStatus { get; set; }
		public string ProxiedMessageName { get; set; }
		public int ResponseCode { get; set; }
		public string ResponseText { get; set; }
	}
	public class ChargeAccountToTemporaryToken
	{
		public string ChargeAccountNumberToken { get; set; }
		public string PaymentDeviceLast4 { get; set; }
		public int PaymentDeviceTypeCD { get; set; }
		public int ResponseCode { get; set; }
		public string ResponseText { get; set; }
	}
	public class GetSessionTags
	{
		public string OrgID { get; set; }
		public int ResponseCode { get; set; }
		public string ResponseText { get; set; }
		public string WebSessionID { get; set; }
		public int idrecarga { get; set; }
	}
	public class ReversePayment
	{
		public decimal AvailableRefundAmount { get; set; }
		public string PartnerCustomerKey { get; set; }
		public string PaymentAcquirerName { get; set; }
		public string PaymentID { get; set; }
		public int PaymentStatus { get; set; }
		public string ProxiedMessageName { get; set; }
		public int ResponseCode { get; set; }
		public string ResponseText { get; set; }
		public int ReversalAction { get; set; }
	}
	public class errRecarga
	{
		public int idtarjeta { get; set; }
		public int idrecarga { get; set; }
		public string PaymentID { get; set; }
		public int err { get; set; }
		public int errVs { get; set; }
		public int errRs { get; set; }
		public tresp tresp { get; set; }
	}

	public class tresp
	{
		public string printDatam_data { get; set; }
		public string op_authorization { get; set; }
		public string transaction_id { get; set; }
		public string rcode_description { get; set; }
	}

	public class opcMenu
    {
        public opcMenu(int pos, int idop, string titulo, string desc, ImageSource image)
        {
            this.Pos = pos;
            this.idOpcion = idop;
            this.Titulo = titulo;
            this.Desc = desc;
            this.Image = image;
        }

        public int Pos { private set; get; }
        public int idOpcion { set; get; }
        public string Titulo { set; get; }
        public string Desc { set; get; }
        public ImageSource Image { set; get; }
    }


}
