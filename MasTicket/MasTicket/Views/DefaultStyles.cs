using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

namespace MasTicket
{
    public static class DefaultStyles
    {
        public static Style CargarSaldoBtnNombre
        {
            get
            {
                return new Style(typeof(ImageButton))
                {
                    Setters = {
                        new Setter { Property = ImageButton.SourceProperty, Value = "smartphonew.png" },
                        new Setter { Property = ImageButton.TextProperty, Value = "" },
                    }
                };
            }
        }
    }
}
