using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using Rg.Plugins.Popup.Extensions;

namespace MasTicket
{
	public partial class Miperfil : ContentPage
	{
		public Miperfil()
		{
			InitializeComponent();
			NavigationPage.SetBackButtonTitle(this, "");

			cvwPerfil.BindingContext = App.usr;

			TapGestureRecognizer tapgrdMonedero = new TapGestureRecognizer();
			tapgrdMonedero.Tapped += async (s, e) =>
			{
				grdMonedero.BackgroundColor = Color.FromHex("#e5e5e5"); //Color.FromHex((App.Current.Resources["ButtonRojo"] as Style).Setters.Where(x => x.Property == BackgroundColorProperty).FirstOrDefault().Value.ToString()); //.Opacity = .5;
				await Task.Delay(100);
				grdMonedero.BackgroundColor = Color.Transparent; //grdEstado.Opacity = 1;
				RecargasViewModel rvm = new RecargasViewModel();
				rvm.Tiporecarga = TipoRecarga.Monedero;
				await App.Nav.PushAsync(new RegPago(rvm), Constantes.animated);
			};
			grdMonedero.GestureRecognizers.Add(tapgrdMonedero);
			//int hab = App.WSc.GetMonederoHab();
			//if (hab == 1)
			//	grdMonedero.IsVisible = true;
			//else
				grdMonedero.IsVisible = false;
			
			//TapGestureRecognizer tapgrdMedios = new TapGestureRecognizer();
			//tapgrdMedios.Tapped += async (s, e) =>
			//{
			//	grdMedios.BackgroundColor = Color.FromHex("#e5e5e5"); //Color.FromHex((App.Current.Resources["ButtonRojo"] as Style).Setters.Where(x => x.Property == BackgroundColorProperty).FirstOrDefault().Value.ToString()); //.Opacity = .5;
			//	await Task.Delay(100);
			//	grdMedios.BackgroundColor = Color.Transparent; //grdEstado.Opacity = 1;
			//	RecargasViewModel rvm = new RecargasViewModel();
            //  rvm.ReadOnly = true;
            //  await App.Nav.PushAsync(new RegPago(rvm), Constantes.animated);
			//};
			//grdMedios.GestureRecognizers.Add(tapgrdMedios);

			TapGestureRecognizer tapgrdChat = new TapGestureRecognizer();
			tapgrdChat.Tapped += async (s, e) =>
			{
				grdChat.BackgroundColor = Color.FromHex("#e5e5e5");
				await Task.Delay(100);
				grdChat.BackgroundColor = Color.Transparent;
				//await App.Nav.PushAsync(new MainChatPage(), Constantes.animated);
				await App.Nav.PushAsync(new webchat(), Constantes.animated);
				//await App.Nav.PushAsync(new ChatPage());
			};
			grdChat.GestureRecognizers.Add(tapgrdChat);

			TapGestureRecognizer tapgrdTuto = new TapGestureRecognizer();
			tapgrdTuto.Tapped += async (s, e) =>
			{
				grdTuto.BackgroundColor = Color.FromHex("#e5e5e5"); //Color.FromHex((App.Current.Resources["ButtonRojo"] as Style).Setters.Where(x => x.Property == BackgroundColorProperty).FirstOrDefault().Value.ToString()); //.Opacity = .5;
				await Task.Delay(100);
				grdTuto.BackgroundColor = Color.Transparent; //grdEstado.Opacity = 1;
				await App.Nav.PushAsync(new Tuto(App.Current), Constantes.animated);
            };
			grdTuto.GestureRecognizers.Add(tapgrdTuto);

			TapGestureRecognizer tappoliticas = new TapGestureRecognizer();
			tappoliticas.Tapped += async (s, e) =>
			{
				grdPolitica.BackgroundColor = Color.FromHex("#e5e5e5");
				await Task.Delay(100);
				grdPolitica.BackgroundColor = Color.Transparent;
				var page = new InfoAyuda("politica.html");
				Navigation.PushPopupAsync(page);
			};
			grdPolitica.GestureRecognizers.Add(tappoliticas);
			TapGestureRecognizer tapterminos = new TapGestureRecognizer();
			tapterminos.Tapped += async (s, e) =>
			{
				grdTerminos.BackgroundColor = Color.FromHex("#e5e5e5");
				await Task.Delay(100);
				grdTerminos.BackgroundColor = Color.Transparent;
				var page = new InfoAyuda("terminos.html");
				Navigation.PushPopupAsync(page);
			};
			grdTerminos.GestureRecognizers.Add(tapterminos);
		}
	}
}

