using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MasTicket
{
	public partial class HistoricosTab : TabbedPage
	{
		public HistoricosTab()
		{
			InitializeComponent();

			this.Children.Add(new Historicos() { Title = "Recargas", Icon = "smartphonetab.png" });
			this.Children.Add(new HistoricosWallet() { Title = "Monedero", Icon = "wallettab.png" });

		}
	}
}

