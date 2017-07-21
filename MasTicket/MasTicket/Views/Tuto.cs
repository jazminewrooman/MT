using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;
using CaveBirdLabs.Forms;
using Acr.UserDialogs;

namespace MasTicket
{
    public class Tuto : CxPagedCarouselPage
    {
        ILoginManager _ilm;

        public Tuto(ILoginManager ilm)
        {
            _ilm = ilm;
            Title = "Introducción";
            UserDialogs.Instance.HideLoading();
            //Children.Add(new IntroPage("tuto1.png", "Bienvenido", "MasTicket es la forma mas rapida y facil de recargar tiempo aire", false, _ilm));
            //Children.Add(new IntroPage("tuto2.png", "Escoge", "Manejamos todas las operadoras de México", false, _ilm));
            //Children.Add(new IntroPage("tuto3.png", "Aplica", "Introduce tu tarjeta y ya. Asi de facil!", false, _ilm));
            //Children.Add(new IntroPage("tuto4.png", "Programa", "Puedes programar recargas frecuentes para tus familiares, solo escoge el dia que quieras!", false, _ilm));
            //Children.Add(new IntroPage("chicoschat2.png", "Recarga", "", true, _ilm));

            Children.Add(new IntroPage("welcome1.jpg", false, _ilm));
            Children.Add(new IntroPage("welcome2.jpg", false, _ilm));
            Children.Add(new IntroPage("welcome3.jpg", true, _ilm));
            //Children.Add(new IntroPage("welcome4a.png", true, _ilm));
            //Children.Add(new IntroPage("chicoschat2.png", "Recarga", "", true, _ilm));

            PagerPadding = new Thickness(0, 0, 0, 0);
            SelectedPagerItemColor = Color.FromRgb(230, 70, 41);
            PagerItemColor = Color.FromRgb(153, 153, 153);

            //Padding = new Thickness(0, 0, 0, 50);
            

        }
    }

}
