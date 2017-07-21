using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Acr.UserDialogs;

namespace MasTicket
{
	public partial class Cargando : ContentPage
	{
		public Cargando()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			UserDialogs.Instance.ShowLoading("Cargando...");
		}
	}
}

