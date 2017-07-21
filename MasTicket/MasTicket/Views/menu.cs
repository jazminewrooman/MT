using System;
using System.Collections.Generic;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Abstractions;

namespace MasTicket
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class menu : ContentPage
    {
        public ListView Menu { get; set; }
        StackLayout layout;

		public void Refrescamenu()
		{
			cargamenu();
		}

		public menu()
		{
			Title = "Menu";
			Icon = "slideout.png";
			cargamenu();
		}

		public void cargamenu()
		{
			Menu = new MenuListView();
			Menu.ItemSelected += (sender, e) => NavigateTo(e.SelectedItem as MenuItem);

			var img = new CircleImage() { HeightRequest = 32, Style = App.Current.Resources["ImgMenu"] as Style };
			Label txtusr = new Label() { VerticalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, FontSize = 20 };
			if (App.usr != null && !String.IsNullOrEmpty(App.usr.email) && App.usr.idusuario != 0)
			{
				try
				{
					if (!String.IsNullOrEmpty(App.usr.picture))
						img.Source = ImageSource.FromUri(new Uri(App.usr.picture));
					else
						img.Source = "acicon.png";
				}
				catch (System.UriFormatException x)
				{
					img.Source = "acicon.png";
				}

				txtusr = new Label();
				if (!String.IsNullOrEmpty(App.usr.name))
					txtusr.Text = App.usr.name;
				else
					txtusr.Text = "Usuario";
				txtusr.FontSize = 20;
			}
			else
				txtusr.Text = "";
			StackLayout menuLabel = new StackLayout
			{
				Padding = new Thickness(10, 0, 10, 0),
				//Spacing = 0,
				//HeightRequest = 40,
				HorizontalOptions = LayoutOptions.Fill,
				Orientation = StackOrientation.Horizontal,
				//HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					img,
					new BoxView() { WidthRequest = 10 },
					txtusr
				}
			};

			ImageButton btnSalir = new ImageButton() { Text = "Cerrar Sesion", Source = "cross.png", Style = App.Current.Resources["ImgButtonRojo"] as Style, };
			btnSalir.Clicked += async (sender, ea) =>
			{
				var answ = await DisplayAlert("Aviso", "¿Desea cerrar la sesion?", "Si", "No");
				if (answ)
				{
					Usuario u = App.db.SelUsr();
					if (u != null)
					{
						App.usr = null;
						App.db.LogoutUsr(u);
					}
					MessagingCenter.Send<Page>(this, "LogOutFace");

					var det = new NavigationPage(new Login(App.Current))
					{
						BarTextColor = Color.White,
						BarBackgroundColor = Color.FromHex("#e35102"),
						Title = "Asi Compras",
					};
					App.Nav = det.Navigation;
					App.Current.MainPage = det;
				}
			};
			ImageButton btnLogin = new ImageButton() { Text = "Ya tengo usuario", Source = "proflew.png", Style = App.Current.Resources["ImgButtonRojo"] as Style, };
			btnLogin.Clicked += (sender, e) =>
			{
				Usuario u = App.db.SelUsr();
				if (u != null)
				{
					App.usr = null;
					App.db.LogoutUsr(u);
				}
				var det = new NavigationPage(new Login(App.Current))
				{
					BarTextColor = Color.White,
					BarBackgroundColor = Color.FromHex("#e35102"),
					Title = "Asi Compras",
				};
				App.Nav = det.Navigation;
				App.Current.MainPage = det;
			};

			layout = new StackLayout
			{
				Spacing = 0,
				VerticalOptions = LayoutOptions.StartAndExpand //FillAndExpand
			};
			StackLayout sllogo = new StackLayout()
			{
				Padding = new Thickness(10, 40, 10, 20),
				Children = {
					new Image() {
						HeightRequest = 120,
#if __IOS__
						Aspect = Aspect.AspectFit,
#endif
#if __ANDROID__
						Aspect = Aspect.AspectFill, 
#endif
						Source = "asicomprasletras.png",
					}
				}
			};
			layout.Children.Add(sllogo);
			if (App.usr != null && !String.IsNullOrEmpty(App.usr.email) && App.usr.idusuario != 0)
			{
				layout.Children.Add(menuLabel);
				layout.Children.Add(new BoxView() { HeightRequest = 1 });//, BackgroundColor = AppStyle.DarkLabelColor });
				layout.Children.Add(Menu);
				//layout.Children.Add(new Image() { HeightRequest = 120, Aspect = Aspect.AspectFill, Source = "masticket.png", });
				//layout.Children.Add(new VwRecompensa() { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center });
				layout.Children.Add(new StackLayout { Padding = new Thickness(10,10,10,10), Children = { btnSalir } });
			}
			else {
				Menu.IsEnabled = false;
				layout.Children.Add(new BoxView() { HeightRequest = 1 });//, BackgroundColor = AppStyle.DarkLabelColor });
				layout.Children.Add(Menu);
				layout.Children.Add(new StackLayout { Padding = new Thickness(10, 10, 10, 10), Children = { btnLogin } });
			}
            Content = layout;
        }

        void NavigateTo(MenuItem menu)
        {
			if (App.master == null)
				App.master = (App.Current.MainPage as MasterDetailPage);

			Page displayPage = (Page)Activator.CreateInstance(menu.TargetType);
			var det = new NavigationPage(displayPage)
			{
				Title = "Asi Compras",
				BarTextColor = Color.White,
				BarBackgroundColor = Color.FromHex("#e35102"),
			};
            App.master.Detail = det;
            App.Nav = det.Navigation;
            App.master.IsPresented = false;
        }
    }

    public class MenuItem
    {
        public string Titulo { get; set; }
        public string Icono { get; set; }
        public Type TargetType { get; set; }
        public Color Color { get; set; }
		public Color TextColor { get; set; }
    }

    public class MenuListView : ListView
    {
        public MenuListView()
        {
            //bool admin = (App.Current.Properties["UsrLogged"] as Usuarios.usuarios).admin;
            List<MenuItem> data = new MenuListData();
            ItemsSource = data;
            VerticalOptions = LayoutOptions.FillAndExpand;
            BackgroundColor = Color.Transparent;
            var cell = new DataTemplate(typeof(menucell));
            //cell.SetBinding (menucell TextCell.TextProperty, "Titulo");
            //cell.SetBinding (ImageCell.ImageSourceProperty, "Icono");
            ItemTemplate = cell;
        }
    }

    public class MenuListData : List<MenuItem>
    {
        public MenuListData()
        {
			Color txt, back;
			if (App.usr != null && !String.IsNullOrEmpty(App.usr.email) && App.usr.idusuario != 0)
			{
				txt = Color.Black;
				back = Color.White;
			}
			else {
				back = Color.FromHex("#cccccc");
				txt = Color.FromHex("#e5e5e5");
			}

			this.Add(new MenuItem()
			{
				Titulo = "Mi perfil",
				Icono = "profle.png",
				Color = back,
				TextColor = txt,
				TargetType = typeof(Miperfil)
			});
			this.Add(new MenuItem()
            {
                Titulo = "Recargar",
                Icono = "money.png",
				Color = back,
				TextColor = txt,
				TargetType = typeof(CargarSaldo)
            });
            this.Add(new MenuItem()
            {
                Titulo = "Invita a tus amigos",
                Icono = "gift.png",
				Color = back,
				TextColor = txt,
				TargetType = typeof(Referidos)
            });
            //this.Add(new MenuItem()
            //{
            //    Titulo = "Recompensas",
            //    Icono = "medal.png",
            //    Color = Color.White,
            //    TargetType = typeof(Reco)
            //});
            //this.Add(new MenuItem()
            //{
            //    Titulo = "Monedero",
            //    Icono = "wallet.png",
            //    Color = Color.White,
            //    TargetType = typeof(Monedero)
            //});
            this.Add(new MenuItem()
            {
                Titulo = "Recargas frecuentes",
                Icono = "calendar.png",
				Color = back,
				TextColor = txt,
				TargetType = typeof(Automaticas)
            });
            this.Add(new MenuItem()
            {
                Titulo = "Histórico de recargas",
                Icono = "clock.png",
				Color = back,
				TextColor = txt,
				TargetType = typeof(Historicos),
				//TargetType = typeof(HistoricosTab),
            });
            //this.Add(new MenuItem()
            //{
            //    Titulo = "Chat",
            //    Icono = "comments.png",
            //    Color = Color.White,
            //    TargetType = typeof(DataTemplateSelector.MainPage)
            //});
        }
                
                   
    }
            

        }
    

