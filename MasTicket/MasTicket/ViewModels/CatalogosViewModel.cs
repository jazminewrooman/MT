using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MasTicket
{
    public class CatalogosViewModel : BaseViewModel
    {
		private ObservableCollection<catErrores> lserrores;
		public ObservableCollection<catErrores> lsErrores()
		{
			return (lserrores);
		}
        private ObservableCollection<catPais> lspais;
        public ObservableCollection<catPais> lsPais()
        {
            return (lspais);
        }
        private ObservableCollection<catEstado> lsestados;
        public ObservableCollection<catEstado> lsEstados()
        {
            return (lsestados);
        }
        public ObservableCollection<catEstado> lsEstados(int idp)
        {
            return (lsestados.Where(x => x.idpais == idp).ToObservableCollection());
        }
        private ObservableCollection<catMunicipio> lsmunicipios;
        public ObservableCollection<catMunicipio> lsMunicipios()
        {
            return (lsmunicipios);
        }
        public ObservableCollection<catMunicipio> lsMunicipios(int ide)
        {
            return (lsmunicipios.Where(x => x.idestado == ide).ToObservableCollection());
        }
        private ObservableCollection<catOperadora> lsoperadoras;
        public ObservableCollection<catOperadora> LsOperadoras(int idp)
        {
            return (lsoperadoras.Where(x => x.idpais == idp).ToObservableCollection());
        }
        public ObservableCollection<catOperadora> LsOperadoras()
        {
            return (lsoperadoras.ToObservableCollection());
        }

        private ObservableCollection<catPaquete> lspaquetes;
        public ObservableCollection<catPaquete> LsPaquetes(int ido)
        {
            return (lspaquetes.Where(x => x.idoperadora == ido).ToObservableCollection());
        }
        public ObservableCollection<catPaquete> LsPaquetes()
        {
            return (lspaquetes.ToObservableCollection());
        }

        private ObservableCollection<catFormasPago> lstformaspago;
		public ObservableCollection<catFormasPago> LstFormasPago()
		{
			return (lstformaspago);
		}

		private ObservableCollection<catEmisorTC> lsemisores;
		public ObservableCollection<catEmisorTC> LsEmisores()
		{
			return (lsemisores);
		}

        public event EventHandler<GetCatalogoCompletedEventArgs> GetCatalogoBack;
        protected virtual void OnGetCatalogoBack(GetCatalogoCompletedEventArgs e)
        {
            var handler = GetCatalogoBack;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public async Task<bool> Descarga()
        {
			bool err = false;
            string json = "";
			try
			{
				await Task.Run(() =>
				{
					if (App.db.SelPais().Count() == 0)
					{
						lserrores = new ObservableCollection<catErrores>();
						json = App.WSc.GetCatalogo(6, "");
						lserrores = JsonConvert.DeserializeObject<ObservableCollection<catErrores>>(json);
						App.db.DescargaErrores(lserrores.ToList());

						lspais = new ObservableCollection<catPais>();
						json = App.WSc.GetCatalogo(2, "");
						lspais = JsonConvert.DeserializeObject<ObservableCollection<catPais>>(json);
						App.db.DescargaPais(lspais.ToList());

						lsestados = new ObservableCollection<catEstado>();
						json = App.WSc.GetCatalogo(7, "");
						lsestados = JsonConvert.DeserializeObject<ObservableCollection<catEstado>>(json);
						App.db.DescargaEstado(lsestados.ToList());

						lsmunicipios = new ObservableCollection<catMunicipio>();
						json = App.WSc.GetCatalogo(8, "");
						lsmunicipios = JsonConvert.DeserializeObject<ObservableCollection<catMunicipio>>(json);
						App.db.DescargaMunicipio(lsmunicipios.ToList());

						lsemisores = new ObservableCollection<catEmisorTC>();
						json = App.WSc.GetCatalogo(4, "");
						lsemisores = JsonConvert.DeserializeObject<ObservableCollection<catEmisorTC>>(json);
						App.db.DescargaEmisorTC(lsemisores.ToList());
					}
					catConfig cfg = App.db.SelcatConfig().Where(x => x.idconfig == 1).FirstOrDefault();
					string fechaant = cfg.valor;
					DateTime dt = DateTime.MinValue;
					DateTime.TryParseExact(fechaant, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt);
					//if (dt.Date < DateTime.Now.Date)
					//{
						lsoperadoras = new ObservableCollection<catOperadora>();
						json = App.WSc.GetCatalogo(1, "");
						lsoperadoras = JsonConvert.DeserializeObject<ObservableCollection<catOperadora>>(json);
						App.db.DescargaOperadora(lsoperadoras.ToList());

						lspaquetes = new ObservableCollection<catPaquete>();
						json = App.WSc.GetCatalogo(3, "");
						lspaquetes = JsonConvert.DeserializeObject<ObservableCollection<catPaquete>>(json);
						App.db.DescargaPaquete(lspaquetes.ToList());

						cfg.valor = DateTime.Now.ToString("yyyyMMdd");
						App.db.AltaConfig(cfg);
					//}
				});
			}
			catch (Exception ex)
			{
				err = true;
			}
			return (err);
        }

        public CatalogosViewModel()
		{
            try
            {
				lserrores = App.db.SelErrores();
                lspais = App.db.SelPais();
                lsestados = App.db.SelEstado();
                lsmunicipios = App.db.SelMunicipio();
                lsemisores = App.db.SelEmisorTC();
                lsoperadoras = App.db.SelOperadora();
                lspaquetes = App.db.SelPaquete();
                lstformaspago = new ObservableCollection<catFormasPago>();
                lstformaspago.Add(new catFormasPago() { idformapago = 1, formapago = "Monedero" });
                lstformaspago.Add(new catFormasPago() { idformapago = 2, formapago = "Tarjeta" });
            } catch (Exception ex)
            {

            }
        }
    }
}
