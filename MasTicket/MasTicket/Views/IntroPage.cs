using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace MasTicket
{
    public class IntroPage : ContentPage
    {
        #region Constructors
        ILoginManager _ilm;

        public IntroPage(string imageName, string header, string detail, bool iniciar, ILoginManager ilm)
        {
            _ilm = ilm;
            
            var grid = new Grid()
            {
                ColumnDefinitions = new ColumnDefinitionCollection() {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() {Height = new GridLength(1, GridUnitType.Auto)},
                    new RowDefinition() {Height = new GridLength(1, GridUnitType.Star)},
                },
                Style = (App.Current.Resources["GridFondoMorado"] as Style),
            };

            var image = new Image()
            {
                Source = ImageSource.FromFile(Device.OnPlatform(iOS: imageName, Android: imageName, WinPhone: "Assets/" + imageName)),
                Aspect = Aspect.AspectFill
            };
            grid.Children.Add(image, 0, 0);

            var textGrid = new Grid()
            {
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() {Height = new GridLength(1, GridUnitType.Auto)},
                    new RowDefinition() {Height = new GridLength(1, GridUnitType.Star)},
                    new RowDefinition() {Height = new GridLength(1, GridUnitType.Auto)},
                    new RowDefinition() {Height = new GridLength(40)},
                },
                Style = (App.Current.Resources["GridFondoMorado"] as Style),
                Padding = new Thickness(10, 2, 10, 0)
            };
            grid.Children.Add(textGrid, 0, 1);

            //TODO: Determine size of text for Smaller phones (now iPhone5, Nexus5 tested)

            var headerLabel = new Xamarin.Forms.Label()
            {
                Text = header,
                TextColor = Color.White,
                XAlign = TextAlignment.Center,
                YAlign = TextAlignment.Center,
                Font = Font.SystemFontOfSize(24),
                LineBreakMode = LineBreakMode.WordWrap
            };
            textGrid.Children.Add(headerLabel, 0, 0);

            var detailLabel = new Xamarin.Forms.Label()
            {
                Text = detail,
                TextColor = Color.White,
                XAlign = TextAlignment.Center,
                YAlign = TextAlignment.Center,
                Font = Font.SystemFontOfSize(16),
                LineBreakMode = LineBreakMode.WordWrap
            };
            textGrid.Children.Add(detailLabel, 0, 1);

            if (iniciar && !Settings.TutoVisto1aVez)
            {
                MTButton btnIniciar = new MTButton()
                {
                    Text = "Iniciar",
                    Image = "rateupw.png",
                    Style = (App.Current.Resources["ButtonRojo"] as Style)
                };
                btnIniciar.Clicked += (sender, e) =>
                {
                    Settings.TutoVisto1aVez = true;
                    var det = new NavigationPage(new Login(_ilm))
                    {
                        BarTextColor = Color.White,
                        BarBackgroundColor = Color.FromHex("#e35102"),
                        Title = "Asi Compras",
                    };
                    App.Nav = det.Navigation;
                    App.Current.MainPage = det;

                    //_ilm.ShowMainPage();
                };
                textGrid.Children.Add(btnIniciar, 0, 2);
            }

            Content = grid;
        }

        public IntroPage(string imageName, bool iniciar, ILoginManager ilm)
        {
            _ilm = ilm;

            var grid = new Grid()
            {
                ColumnDefinitions = new ColumnDefinitionCollection() {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() {Height = new GridLength(1, GridUnitType.Star)},
                    new RowDefinition() {Height = new GridLength(80)},
                },
                Style = (App.Current.Resources["GridFondoMorado"] as Style),
            };

            var image = new Image()
            {
                Source = ImageSource.FromFile(Device.OnPlatform(iOS: imageName, Android: imageName, WinPhone: "Assets/" + imageName)),
                Aspect = Aspect.AspectFill
            };
            grid.Children.Add(image, 0, 0);

            var textGrid = new Grid()
            {
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions = new RowDefinitionCollection()
                {
                    //new RowDefinition() {Height = new GridLength(1, GridUnitType.Auto)},
                    //new RowDefinition() {Height = new GridLength(1, GridUnitType.Star)},
                    new RowDefinition() {Height = new GridLength(40)},
                    new RowDefinition() {Height = new GridLength(40)},
                },
                Style = (App.Current.Resources["GridFondoMorado"] as Style),
                Padding = new Thickness(10, 2, 10, 0)
            };
            grid.Children.Add(textGrid, 0, 1);

            if (!Settings.TutoVisto1aVez)
            {
                MTButton btnIniciar = new MTButton()
                {
					Text = (iniciar ? "Iniciar" : "Saltar"),
					Image = (iniciar ? "rateupw.png" : "cross.png"),
                    Style = (App.Current.Resources["ButtonRojo"] as Style)
                };
                btnIniciar.Clicked += (sender, e) =>
                {
                    Settings.TutoVisto1aVez = true;
					//var det = new NavigationPage(new Login(_ilm))

					//var det = new NavigationPage(new CargarSaldo(null))
					//               {
					//                   BarTextColor = Color.White,
					//                   BarBackgroundColor = Color.FromHex("#e35102"),
					//                   Title = "Así Compras",
					//               };
					//               App.Nav = det.Navigation;
					//               App.Current.MainPage = det;

					App.Current.ShowMainPage();

                };
                textGrid.Children.Add(btnIniciar, 0, 0);
            }

            Content = grid;
        }

        #endregion
    }
}
