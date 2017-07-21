using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using DeviceOrientation.Forms.Plugin.Abstractions;

namespace MasTicket
{
	public partial class VwNuevaTarjeta : ContentView
	{
        private double width;
        private double height;

        private void CambiaOrientacion()
        {
            IDeviceOrientation _deviceOrientationSvc = DependencyService.Get<IDeviceOrientation>();
            DeviceOrientations dvcor = _deviceOrientationSvc.GetOrientation();
            if (dvcor == DeviceOrientations.Landscape)
            {
                grdMainCard.ColumnDefinitions.Clear();
                grdMainCard.RowDefinitions.Clear();
                Enumerable.Range(1, 4).ToList().ForEach(x =>
                    grdMainCard.RowDefinitions.Add(
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                    ));
                Enumerable.Range(1, 3).ToList().ForEach(x =>
                    grdMainCard.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(33, GridUnitType.Star) }
                ));
                //grdMainCard.Children.Clear();
                grdMainCard.Children.Add(btnLeerCard, 0, 0 + 1, 0, 0 + 1);
                grdMainCard.Children.Add(cvwCambiarPais, 1, 1 + 1, 0, 0 + 1);
                //grdMainCard.Children.Add(cvwCambiarCard, 2, 2 + 1, 0, 0 + 1);
                grdMainCard.Children.Add(txtNumCard, 0, 0 + 3, 1, 1 + 1);
                grdMainCard.Children.Add(grdCvvCard, 0, 0 + 3, 2, 2 + 1);
                grdMainCard.Children.Add(txtTitular, 0, 0 + 3, 3, 3 + 1);
            }
            else
            {
                try
                {
                    grdMainCard.ColumnDefinitions.Clear();
                    grdMainCard.RowDefinitions.Clear();
                    Enumerable.Range(1, 6).ToList().ForEach(x =>
                        grdMainCard.RowDefinitions.Add(
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                        ));
                    Enumerable.Range(1, 3).ToList().ForEach(x =>
                        grdMainCard.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(33, GridUnitType.Star) }
                    ));
                    //grdMainCard.Children.Clear();
                    grdMainCard.Children.Add(btnLeerCard, 0, 0 + 3, 0, 0 + 1);
                    grdMainCard.Children.Add(cvwCambiarPais, 0, 0 + 3, 1, 1 + 1);
                    //grdMainCard.Children.Add(cvwCambiarCard, 0, 0 + 3, 2, 2 + 1);
                    grdMainCard.Children.Add(txtNumCard, 0, 0 + 3, 3, 3 + 1);
                    grdMainCard.Children.Add(grdCvvCard, 0, 0 + 3, 4, 4 + 1);
                    grdMainCard.Children.Add(txtTitular, 0, 0 + 3, 5, 5 + 1);
                }
                catch (Exception ex)
                {

                }
            }
        }

        public VwNuevaTarjeta ()
		{
			InitializeComponent ();
            CambiaOrientacion();

            MessagingCenter.Subscribe<DeviceOrientationChangeMessage>(this, DeviceOrientationChangeMessage.MessageId, (message) =>
            {
                CambiaOrientacion();
            });

           // btnLeerCard.Clicked += (s, e) =>
           // {
           //     var plataforma = DependencyService.Get<ICardReader>();
           //     if (plataforma != null)
           //     {
           //         //plataforma.CardFound = delegate (string FormattedCardNumber, int ExpiryMonth, int ExpiryYear, string CardholderName, Card.IO.CardType CardType)
           //         //{
           //         //    txtNumCard.Text = FormattedCardNumber;
           //         //    txtTitular.Text = CardholderName;
           //         //    txtMes.Text = (ExpiryMonth > 0 ? ExpiryMonth.ToString() : "");
           //         //    txtAno.Text = (ExpiryYear > 0 ? ExpiryYear.ToString() : "");
           //         //    if (CardType != null)
           //         //    {
           //         //        cvwCambiarCard.SelVal(CardType);
           //         //    }
           //         //};
           //         plataforma.ReadCard();
           //     }
           //};

        }


        
    }
}
