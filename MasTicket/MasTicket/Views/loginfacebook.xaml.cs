using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Acr.UserDialogs;

namespace MasTicket
{
    public enum TipoSocial
    {
        facebook,
        google
    }

	public partial class loginfacebook : ContentPage
	{
        public TipoSocial _ts;

		protected override void OnAppearing()
		{
			//UserDialogs.Instance.ShowLoading("Cargando...");
			base.OnAppearing();
		}

		public loginfacebook (TipoSocial ts)
		{
			InitializeComponent ();

            _ts = ts;
        }
	}
}
