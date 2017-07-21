using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Acr.UserDialogs;
using Splat;
using Card.IO;

namespace MasTicket
{
    public partial class CambiarCard : ContentView
    {
        ObservableCollection<Opcion> ls;

        private void Cambia(int id)
        {
            btnEmisor.Text = ls.Where(x => x.idopc == id).FirstOrDefault().opc;
            btnEmisor.Image = ls.Where(x => x.idopc == id).FirstOrDefault().imgopc;
        }

        public void SelVal(CardType tipo)
        {
            switch (tipo.Name.ToUpper())
            {
                case "AMEX":
                    Cambia(1); break;
                case "DINERSCLUB":
                    Cambia(3); break;
                case "DISCOVER":
                    Cambia(4); break;
                case "INSUFFICIENT_DIGITS":
                    Cambia(9); break;
                case "JCB":
                    Cambia(9); break;
                case "MAESTRO":
                    Cambia(5); break;
                case "MASTERCARD":
                    Cambia(6); break;
                case "UNKNOWN":
                    Cambia(9); break;
                case "VISA":
                    Cambia(7); break;
                default:
                    Cambia(9); break;
            }
        }

        public CambiarCard()
        {
            InitializeComponent();

            ls = new ObservableCollection<Opcion>();
            ls.Add(new Opcion() { idopc = 1, opc = "Amex", imgopc = "americanexpress.png" });
            ls.Add(new Opcion() { idopc = 2, opc = "Cirrus", imgopc = "cirrus.png" });
            ls.Add(new Opcion() { idopc = 3, opc = "Diners Club", imgopc = "dinersclub.png" });
            ls.Add(new Opcion() { idopc = 4, opc = "Discover", imgopc = "discover.png" });
            ls.Add(new Opcion() { idopc = 5, opc = "Maestro", imgopc = "maestro.png" });
            ls.Add(new Opcion() { idopc = 6, opc = "Mastercard", imgopc = "master.png" });
            ls.Add(new Opcion() { idopc = 7, opc = "Visa", imgopc = "visa1.png" });
            ls.Add(new Opcion() { idopc = 8, opc = "Western", imgopc = "westernunion.png" });
            ls.Add(new Opcion() { idopc = 9, opc = "Otra", imgopc = "genericcard.png" });
          
            btnEmisor.Clicked += (sender, ea) =>
            {
                var cfg = new ActionSheetConfig().SetTitle("Seleccione emisor");
#if __IOS__
                foreach (Opcion o in ls)
                    cfg.Add(o.opc, () => Cambia(o.idopc)); //, BitmapLoader.Current.LoadFromResource(o.imgopc, null, null).Result);
#else
				foreach(Opcion o in ls)
                    cfg.Add(o.opc, () => Cambia(o.idopc), BitmapLoader.Current.LoadFromResource(o.imgopc, null, null).Result);
#endif
                //cfg.SetDestructive(action: () => this.Result("Destructive BOOM Selected"));
                cfg.SetCancel();
                UserDialogs.Instance.ActionSheet(cfg);
            };
        }
    }
}
