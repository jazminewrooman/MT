using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Globalization;

namespace MasTicket
{
	public partial class VwProfile : ContentView
	{
		public VwProfile()
		{
			InitializeComponent();
		}
	}

    public class ImgProfileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageSource imgsrc;
			try
			{
				if (!String.IsNullOrEmpty(App.usr.picture))
					imgsrc = ImageSource.FromUri(new Uri(App.usr.picture));
				else
					imgsrc = ImageSource.FromFile("acicon.png");
			}
			catch (System.UriFormatException x)
			{
				imgsrc = ImageSource.FromFile("acicon.png");
			}
            return imgsrc;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

