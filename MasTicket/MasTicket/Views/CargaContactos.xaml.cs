using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MasTicket
{
	public partial class CargaContactos : TabbedPage
	{
		public CargaContactos (List<Contacto> ls)
		{
			InitializeComponent ();

            //this.Children.Add(new Contactos(ls) { Title = "Contactos" });
            //this.Children.Add(new Frecuentes() { Title = "Historico de recargas" });
        }
	}
}
