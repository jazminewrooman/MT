using System;

using Xamarin.Forms;

namespace MasTicket
{
    public class menucell : ViewCell
    {
        public menucell()
        {
            var icono = new Image
            {
                HeightRequest = 28,
                //WidthRequest = 30,
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            icono.SetBinding(Image.SourceProperty, "Icono");

            var titulo = new Label()
            {
                VerticalOptions = LayoutOptions.Center,
                FontFamily = "HelveticaNeue-Medium",
                FontSize = 12,
                //TextColor = Color.FromHex(App.PrimaryColor)
            };
            titulo.SetBinding(Label.TextProperty, "Titulo");

            var statusLayout = new StackLayout
            {
				Padding = new Thickness(10, 0, 10, 0),
                Orientation = StackOrientation.Horizontal,
                Children = { icono, titulo }
            };

            statusLayout.SetBinding(StackLayout.BackgroundColorProperty, "Color");
			titulo.SetBinding(Label.TextColorProperty, "TextColor");

			this.View = statusLayout;
        }
    }
}
