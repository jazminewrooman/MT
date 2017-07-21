using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Acr.UserDialogs;
using Splat;

namespace MasTicket
{
    public partial class VwCambiarPais : ContentView
    {
        CatalogosViewModel cvm;
        private int idpais = 0;

        public int IdPais
        {
            get {
                return (idpais);
            }
        }

        private void Cambia(int idpais)
        {
            catPais p = cvm.lsPais().Where(x => x.idpais == idpais).FirstOrDefault();
            if (p != null)
            {
                btnPais.Text = p.pais;
                btnPais.Source = p.img;
                idpais = p.idpais;
                if (App.usr != null)
                {
                    App.usr.idpais = p.idpais;
                    App.db.AltaUsr(App.usr);
                }
            }

        }

        public VwCambiarPais()
		{
			InitializeComponent ();

            cvm = new CatalogosViewModel();

            catPais paisdef = cvm.lsPais().Where(x => x.paisdefault).FirstOrDefault();
            if (paisdef != null)
            {
                btnPais.Text = paisdef.pais;
                btnPais.Source = paisdef.img;
                idpais = paisdef.idpais;
            }
            btnPais.Clicked += (sender, ea) =>
            {
                var cfg = new ActionSheetConfig().SetTitle("Seleccione pais");
                foreach (catPais p in cvm.lsPais())
                    cfg.Add(p.pais, () => Cambia(p.idpais));
                cfg.SetCancel();
                UserDialogs.Instance.ActionSheet(cfg);
            };
        }
	}
}
