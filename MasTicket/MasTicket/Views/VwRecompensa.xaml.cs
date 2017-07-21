using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MasTicket
{
	public partial class VwRecompensa : ContentView
	{
		public VwRecompensa ()
		{
			InitializeComponent ();

            btnReco.Clicked += async (s, e) => {
                await App.Nav.PushAsync(new Reco());
                App.master.IsPresented = false;
            };
		}


	}
}
