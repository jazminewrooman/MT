using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;

namespace MasTicket
{
	public partial class HistoricosWallet : ContentPage
	{
		List<RecargaMonedero> lsr = new List<RecargaMonedero>();
		RecargasViewModel rvm;

		protected override void OnAppearing()
		{
			lvHistoricos.BeginRefresh();
			lvHistoricos.IsGroupingEnabled = false;
			lvHistoricos.ItemsSource = lsr;
			lvHistoricos.EndRefresh();

			base.OnAppearing();
		}

		public HistoricosWallet()
		{
			InitializeComponent();
			rvm = new RecargasViewModel();
			Title = "Historico de recargas";
			NavigationPage.SetBackButtonTitle(this, "");

			lsr = rvm.SelRecargasWallet().Where(x => x.err == 0 && !String.IsNullOrEmpty(x.PaymentID)).OrderByDescending(x => x.fecha).ToList();
			lvHistoricos.ItemSelected += (sender, e) =>
			{
				if (e.SelectedItem == null)
					return;

				((ListView)sender).SelectedItem = null;
			};
		}	
	}

}

