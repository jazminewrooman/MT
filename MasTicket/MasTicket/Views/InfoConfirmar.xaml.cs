using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace MasTicket
{
	public partial class InfoConfirmar : PopupPage
    {
		public InfoConfirmar ()
		{
			InitializeComponent ();

            this.IsBackgroundAnimating = true;
            this.IsCloseOnBackgroundClick = false;
        }

        //private void OnClose(object sender, EventArgs e)
        //{
        //    App.Nav.PushAsync(new nip(typeof(Ticket), TipoRecarga.RecargaTA), Constantes.animated);
        //    PopupNavigation.PopAsync();
        //}

        private void OnCancel(object sender, EventArgs e)
        {
            PopupNavigation.PopAsync();
        }
    }
}
