using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MasTicket
{
	public partial class EstamosListos : ContentView
	{
        public void Anim()
        {
            imgFlecha.AnimateWinAsync();
        }

        public EstamosListos()
		{
			InitializeComponent();
        }
	}
}

