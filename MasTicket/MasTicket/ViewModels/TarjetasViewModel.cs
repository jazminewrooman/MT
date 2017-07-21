using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;

namespace MasTicket
{
	public class TarjetasViewModel : CatalogosViewModel
	{
		private string _last4 = "";
		public string Last4
		{
			get
			{
				return _last4;
			}
			set
			{
				if (value != _last4)
				{
					_last4 = value;
				}
			}
		}
		private string _expirationMMYY = "";
		public string ExpirationMMYY
		{
			get
			{
				return _expirationMMYY;
			}
			set
			{
				if (value != _expirationMMYY)
				{
					_expirationMMYY = value;
				}
			}
		}
		private string _numerotarjeta;
		public string NumeroTarjeta
		{
			get
			{
				return _numerotarjeta;
			}
			set
			{
				if (value != _numerotarjeta)
				{
					_numerotarjeta = value;
					NotifyPropertyChanged("NumeroTarjeta");
				}
				OnTarjetaIncompleta(null);
			}
		}
		private string _expiramm;
		public string ExpiraMM
        {
            get
            {
                return _expiramm;
            }
            set
            {
                if (value != _expiramm)
                {
                    _expiramm = value;
                    NotifyPropertyChanged("ExpiraMM");
                }
                OnTarjetaIncompleta(null);
            }
        }
		private string _expirayy;
		public string ExpiraYY
        {
            get
            {
                return _expirayy;
            }
            set
            {
                if (value != _expirayy)
                {
                    _expirayy = value;
                    NotifyPropertyChanged("ExpiraYY");
                }
                OnTarjetaIncompleta(null);
            }
        }
		private string _cvc;
		public string Cvc
        {
            get
            {
                return _cvc;
            }
            set
            {
                if (value != _cvc)
                {
                    _cvc = value;
                    NotifyPropertyChanged("Cvc");
                }
                OnTarjetaIncompleta(null);
            }
        }
        private string _titularFN;
        public string TitularFN
        {
            get
            {
                return _titularFN;
            }
            set
            {
                if (value != _titularFN)
                {
                    _titularFN = value;
                    NotifyPropertyChanged("TitularFN");
                }
                OnTarjetaIncompleta(null);
            }
        }
        private string _titularLN;
        public string TitularLN
        {
            get
            {
                return _titularLN;
            }
            set
            {
                if (value != _titularLN)
                {
                    _titularLN = value;
                    NotifyPropertyChanged("TitularLN");
                }
                OnTarjetaIncompleta(null);
            }
        }
        private string _calleynum;
        public string CalleyNum
        {
            get
            {
                return _calleynum;
            }
            set
            {
                if (value != _calleynum)
                {
                    _calleynum = value;
                    NotifyPropertyChanged("CalleyNum");
                }
                OnTarjetaIncompleta(null);
            }
        }
        private string _cp;
        public string CP
        {
            get
            {
                return _cp;
            }
            set
            {
                if (value != _cp)
                {
                    _cp = value;
                    NotifyPropertyChanged("CP");
                }
                OnTarjetaIncompleta(null);
            }
        }
		private int _idusuario;
		public int Idusuario
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
					NotifyPropertyChanged("Idusuario");
				}
				OnTarjetaIncompleta(null);
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
		private opcMenu _opcemisor;
		public opcMenu opcEmisor
		{
			get
			{
				return _opcemisor;
			}
			set
			{
				if (value != _opcemisor)
				{
					_opcemisor = value;
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
		private opcMenu _opcfecha;
		public opcMenu opcFecha
		{
			get
			{
				return _opcfecha;
			}
			set
			{
				if (value != _opcfecha)
				{
					_opcfecha = value;
					NotifyPropertyChanged();
				}
			}
		}
		private opcMenu _opctitular;
		public opcMenu opcTitular
		{
			get
			{
				return _opctitular;
			}
			set
			{
				if (value != _opctitular)
				{
					_opctitular = value;
					NotifyPropertyChanged();
				}
			}
		}
        private opcMenu _opccalle;
        public opcMenu opcCalle
        {
            get
            {
                return _opccalle;
            }
            set
            {
                if (value != _opccalle)
                {
                    _opccalle = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private opcMenu _opcestado;
        public opcMenu opcEstado
        {
            get
            {
                return _opcestado;
            }
            set
            {
                if (value != _opcestado)
                {
                    _opcestado = value;
					NotifyPropertyChanged();
                }
            }
        }
        private opcMenu _opcciudad;
        public opcMenu opcCiudad
        {
            get
            {
                return _opcciudad;
            }
            set
            {
                if (value != _opcciudad)
                {
                    _opcciudad = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private opcMenu _opccp;
        public opcMenu opcCP
        {
            get
            {
                return _opccp;
            }
            set
            {
                if (value != _opccp)
                {
                    _opccp = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private opcMenu _opcscannear;
        public opcMenu opcScannear
        {
            get
            {
                return _opcscannear;
            }
            set
            {
                if (value != _opcscannear)
                {
                    _opcscannear = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<TipoPago> _lm;
		public ObservableCollection<TipoPago> lm
		{
			get
			{
				return _lm;
			}
		}
		private ObservableCollection<TipoPago> _lt;
		public ObservableCollection<TipoPago> lt
		{
			get
			{
				return _lt;
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
                        _idestado = 0;
                        _idciudad = 0;
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
                    _opcestado.idOpcion = 0;
                    _opcestado.Desc = "Seleccione estado";
                    _opcciudad.idOpcion = 0;
                    _opcciudad.Desc = "Seleccione ciudad";
                    NotifyPropertyChanged("opcPais");
                    NotifyPropertyChanged("opcEstado");
                    NotifyPropertyChanged("opcCiudad");
                }
				OnTarjetaIncompleta(null);
			}
		}
        private int _idestado;
        public int IdEstado
        {
            get
            {
                return _idestado;
            }
            set
            {
                if (value != _idestado)
                {
                    _idestado = value;
                    _idciudad = 0;
                    _opcestado.idOpcion = value;
                    _opcestado.Desc = lsEstados(_idpais).Where(x => x.idestado == value).FirstOrDefault().estado;
                    _opcciudad.idOpcion = 0;
                    _opcciudad.Desc = "Seleccione ciudad";
                    NotifyPropertyChanged("opcEstado");
                    NotifyPropertyChanged("opcCiudad");
                }
                OnTarjetaIncompleta(null);
            }
        }
        private int _idciudad;
        public int IdCiudad
        {
            get
            {
                return _idciudad;
            }
            set
            {
                if (value != _idciudad)
                {
                    _idciudad = value;
                    _opcciudad.idOpcion = value;
                    _opcciudad.Desc = lsMunicipios(_idestado).Where(x => x.idmunicipio == value).FirstOrDefault().municipio;
                    NotifyPropertyChanged("opcCiudad");
                }
                OnTarjetaIncompleta(null);
            }
        }
        private int _idemisor;
		public int idemisor
		{
			get
			{
				return _idemisor;
			}
			set
			{
				if (value != _idemisor)
				{
					if (value > 0)
					{
						_idemisor = value;
						_opcemisor.idOpcion = value;
						_opcemisor.Desc = LsEmisores().Where(x => x.idemisor == value).FirstOrDefault().emisor;
						_opcemisor.Image = LsEmisores().Where(x => x.idemisor == value).FirstOrDefault().img;
					}
					else {
						_idemisor = 0;
						_opcemisor.idOpcion = 0;
						_opcemisor.Desc = "Seleccione emisor";
						_opcemisor.Image = new FileImageSource() { File = "mastercard.png" };
					}
					NotifyPropertyChanged("opcEmisor");
				}
				OnTarjetaIncompleta(null);
			}
		}

		public event EventHandler<NuevaTarjetaEventArgs> NuevaTarjeta;
		protected virtual void OnNuevaTarjeta(NuevaTarjetaEventArgs e)
        {
            var handler = NuevaTarjeta;
			if (handler != null)
			{
				string tipo = "";
				_lt.Clear();
				foreach (Tarjeta t in App.db.SelTarjetas())
				{
					catEmisorTC em = LsEmisores().Where(x => x.idemisor == t.idemisor).FirstOrDefault();
					tipo = (em != null ? em.emisor + " " : "") + t.Last4;
					_lt.Add(new TipoPago() { idtipo = 2, extra = t.idtarjeta, tipo = tipo, saldo = 0, imgtipo = (em != null ? em.img : "") });
				}
				handler(this, new NuevaTarjetaEventArgs { lista = _lt });
			}
        }

        public event EventHandler<TarjetaIncompletaEventArgs> TarjetaIncompleta;
		protected virtual void OnTarjetaIncompleta(TarjetaIncompletaEventArgs e)
		{
			var handler = TarjetaIncompleta;
			if (handler != null)
			{
				if (EsIncompleta())
					handler(this, new TarjetaIncompletaEventArgs { estaincompleta = true });
				else
					handler(this, new TarjetaIncompletaEventArgs { estaincompleta = false });
			}
		}
        public void ChecaIncompleta()
        {
            OnTarjetaIncompleta(null);
        }
		public bool EsIncompleta()
		{
			bool ret;
			if (_idpais == 0 || _idemisor == 0 || String.IsNullOrEmpty(_numerotarjeta) || String.IsNullOrEmpty(_expiramm) || String.IsNullOrEmpty(_expirayy) || String.IsNullOrEmpty(_cvc) || String.IsNullOrEmpty(_titularFN) || String.IsNullOrEmpty(_titularLN) || String.IsNullOrEmpty(_calleynum) || _idestado == 0 || _idciudad == 0 || String.IsNullOrEmpty(_cp) )
				ret = true;
			else
				ret = false;
			return (ret);
		}

		public event EventHandler<EliminaTarjetaCompletedEventArgs> TarjetaBaja;
		protected virtual void OnTarjetaBaja(EliminaTarjetaCompletedEventArgs ea)
		{
			var handler = TarjetaBaja;
			if (handler != null)
			{
				handler(this, ea);
			}
		}
		public void Baja(int idtarjeta)
		{
			EventHandler<EliminaTarjetaCompletedEventArgs> atc = null;
			atc = (s, ea) =>
			{
				App.WS.EliminaTarjetaCompleted -= atc;
				if (ea.Error == null && !ea.Cancelled)
				{
					if (ea.Result)
						App.db.DelTarjeta(idtarjeta);
					Device.BeginInvokeOnMainThread( () => OnNuevaTarjeta(null));
				}
				Device.BeginInvokeOnMainThread(() => OnTarjetaBaja(ea));
				App.WS.Close();
			};
			App.Current.IniciaWS();
			App.WS.EliminaTarjetaCompleted += atc;
			App.WS.EliminaTarjetaAsync(idtarjeta);
		}

        /*public event EventHandler<AltaTarjetaCompletedEventArgs> TarjetaAlta;
		protected virtual void OnTarjetaAlta(AltaTarjetaCompletedEventArgs ea)
        {
            var handler = TarjetaAlta;
            if (handler != null)
            {
                handler(this, ea);
            }
        }*/
        /*public void Alta()
        {
			byte[] cvccrypto = Crypto.EncryptAes(_cvc.ToString(), Settings.PwdCrypto, Crypto.String2ByteArray(Settings.SaltCrypto));

			Tarjeta t = new Tarjeta()
			{
				idusuario = _idusuario,
                idpais = _idpais,
                idemisor = _idemisor,
				permtoken = _numerotarjeta,
				Last4 = _last4,
                titularFN = _titularFN,
                titularLN = _titularLN,
                calleynumero = _calleynum,
                idestado = _idestado,
                idciudad = _idciudad,
                codigopostal = _cp
            };
            EventHandler<AltaTarjetaCompletedEventArgs> atc = null;
			atc = (s, ea) =>
			{
				App.WS.AltaTarjetaCompleted -= atc;
				if (ea.Error == null && !ea.Cancelled)
				{
					t.idtarjeta = ea.Result;
					App.db.AltaTarjeta(t);
				}
				Device.BeginInvokeOnMainThread(() => OnTarjetaAlta(ea));
				Device.BeginInvokeOnMainThread(() => OnNuevaTarjeta(null));
				App.WS.Close();
			};
			App.Current.IniciaWS();
			App.WS.AltaTarjetaCompleted += atc;
			App.WS.AltaTarjetaAsync(ConvertTarjetaToTemp(t));
		}*/

        tempuri.org.Tarjeta ConvertTarjetaToTemp(Tarjeta t)
        {
            //string cvccrypto = Crypto.DecryptAes(t.cvv, Settings.PwdCrypto, Crypto.String2ByteArray(Settings.SaltCrypto));
            tempuri.org.Tarjeta tot = new tempuri.org.Tarjeta
            {
				idusuario = t.idusuario,
                idpais = t.idpais, idemisor = t.idemisor,
				permtoken = t.permtoken, Last4 = t.Last4, expirationMMYY = t.expirationMMYY,
                titularFN = t.titularFN, titularLN = t.titularLN,
                calleynumero = t.calleynumero, idestado = t.idestado,
                idciudad = t.idciudad, codigopostal = t.codigopostal
            };
            return (tot);
        }
		public void RefreshMedios()
		{
			_lm = new ObservableCollection<TipoPago>();
			_lt = new ObservableCollection<TipoPago>();
			if (App.db.SelSaldo() == null)
				App.db.IniciaMonedero();
			_lm.Add(new TipoPago() { idtipo = 1, tipo = "Saldo", saldo = App.db.SelSaldo().saldo });
			string tipo = "";
			foreach (Tarjeta t in App.db.SelTarjetas())
			{
				catEmisorTC em = LsEmisores().Where(x => x.idemisor == t.idemisor).FirstOrDefault();
				tipo = (em != null ? em.emisor + " " : "") + t.Last4;
				_lt.Add(new TipoPago() { idtipo = 2, extra = t.idtarjeta, tipo = tipo, saldo = 0, imgtipo = (em != null ? em.img : "") });
			}
		}
		public TarjetasViewModel()
		{
            _opcpais = new opcMenu(1, 0, "País", "Seleccione un país", new FileImageSource() { File = "globe.png" });
			if (App.usr.idpais != 0)
			{
				_opcpais.idOpcion = App.usr.idpais;
				_opcpais.Desc = lsPais().Where(x => x.idpais == App.usr.idpais).FirstOrDefault().pais;
			}
			_opcemisor = new opcMenu(2, 0, "Tipo tarjeta", "Seleccione emisor", new FileImageSource() { File = "mastercard.png" });
			_opcnumero = new opcMenu(3, 0, "Número de tarjeta", "Seleccione un número", new FileImageSource() { File = "creditcard.png" });
			_opcfecha = new opcMenu(4, 0, "Fecha", "Seleccione un monto", new FileImageSource() { File = "cal.png" });
			_opctitular = new opcMenu(5, 0, "Titular", "Seleccione un cupon", new FileImageSource() { File = "idcard.png" });
            _opcscannear = new opcMenu(6, 0, "Escanear", "Escanear tarjeta", new FileImageSource() { File = "camera.png" });
            _opccalle = new opcMenu(7, 0, "Dirección", "Calle y número", new FileImageSource() { File = "house.png" });
            _opcestado = new opcMenu(8, 0, "Estado", "Seleccione estado", new FileImageSource() { File = "globe.png" });
            _opcciudad = new opcMenu(9, 0, "Ciudad", "Seleccione ciudad", new FileImageSource() { File = "globe.png" });
            _opccp = new opcMenu(10, 0, "Código postal", "Código postal", new FileImageSource() { File = "mappin.png" });

            _lm = new ObservableCollection<TipoPago>();
			_lt = new ObservableCollection<TipoPago>();
			if (App.db.SelSaldo() == null)
				App.db.IniciaMonedero();
			_lm.Add(new TipoPago() { idtipo = 1, tipo = "Saldo", saldo = App.db.SelSaldo().saldo });
			string tipo = "";
			foreach (Tarjeta t in App.db.SelTarjetas())
			{
				catEmisorTC em = LsEmisores().Where(x => x.idemisor == t.idemisor).FirstOrDefault();
				tipo = (em != null ? em.emisor + " " : "") + t.Last4;
				_lt.Add(new TipoPago() { idtipo = 2, extra = t.idtarjeta, tipo = tipo, saldo = 0, imgtipo = (em != null ? em.img : "") });
			}
        }
    }

	public class TarjetaIncompletaEventArgs : EventArgs
	{
		public bool estaincompleta { get; set; }
	}

	public class NuevaTarjetaEventArgs : EventArgs
	{
		public ObservableCollection<TipoPago> lista { get; set; }
	}
}


