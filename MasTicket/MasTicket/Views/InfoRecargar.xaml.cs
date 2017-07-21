using System;
using System.Threading.Tasks;
using System.Threading;
//using Demo.Animations;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using System.Timers;
using System.Collections.Generic;

namespace MasTicket
{
	public partial class InfoRecargar : PopupPage
	{
        CancellationTokenSource ts;
        CancellationToken ct;

        public InfoRecargar(string titulo, string lbltxt)
		{
			InitializeComponent();
			this.IsBackgroundAnimating = true;
			this.IsCloseOnBackgroundClick = true;
            this.IsAnimating = true;
			lblTitulo.Text = titulo;
			lblTxt.Text = lbltxt;
            
            try
			{
                //Timer delayTimer = new Timer();
                //delayTimer.Interval = 5000;
                //delayTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
                //{
                //	delayTimer.Stop();
                //	PopupNavigation.PopAsync();
                //};
                //delayTimer.Start();
                ts = new CancellationTokenSource();
                ct = ts.Token;

                Task.Delay(TimeSpan.FromSeconds(5), ct).ContinueWith(x =>
				{
                    Device.BeginInvokeOnMainThread(() =>
					{
						if (PopupNavigation.PopupStack.Count > 0)
							PopupNavigation.PopAsync();
					});
				});

			}
			catch (Exception ex)
			{
			}
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

        protected override void OnDisappearing()
        {
            ts.Cancel();

            base.OnDisappearing();
            
        }

        private void OnClose(object sender, EventArgs e)
        {
            //ts.Cancel();
            PopupNavigation.PopAsync();
        }
    }
}
