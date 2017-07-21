using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using DeviceOrientation.Forms.Plugin.Abstractions;

using Xamarin.Forms;
using Plugin.Vibrate;
using Plugin.Fingerprint.Abstractions;

namespace MasTicket
{
	public partial class nip : ContentPage
    {
		
		public event EventHandler<EventArgs> NipCorrecto;
		protected virtual void OnNipCorrecto(EventArgs e)
		{
			var handler = NipCorrecto;
			if (handler != null)
			{
				handler(this, null);
			}
		}

		//public nip (Type redirige, params object[] args)
		public nip()
		{
			InitializeComponent ();
            Title = "NIP";
            BindingContext = new nipViewModel();

			//if (
			//var result = await Plugin.Fingerprint.CrossFingerprint.Current.AuthenticateAsync("Prove you have fingers!");
			//if (result.Authenticated)
			//{
			//	await Navigation.PushAsync(new SecretView());
			//}
			//else
			//{
			//	lblStatus.Text = $"{result.Status} : {result.ErrorMessage} ";
			//}

			txtNip.TextChanged += (sender, ea) =>
            {
                if (ea.NewTextValue.Trim().Length == 4)
                {
					if (ea.NewTextValue.Trim() == App.usr.nip)
                    {
						txtNip.Text = "";
						OnNipCorrecto(null);
                        //Page p = (Page)Activator.CreateInstance(redirige, args);
                        //App.Nav.InsertPageBefore(p, this);
                        //await App.Nav.PopAsync(Constantes.animated);
                    }
                    else
                    {
                        txtNip.Text = "";
                        CrossVibrate.Current.Vibration(500);
                    }
                }
            };

            btnCancel.Clicked += async (sender, ea) =>
            {
                await App.Nav.PopAsync(Constantes.animated);
            };
        }

    }
}
