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
    public partial class InfoProgramar : PopupPage
    {
        CancellationTokenSource ts;
        CancellationToken ct;
   
        public InfoProgramar()
        {
            InitializeComponent();
            this.IsBackgroundAnimating = true;
            this.IsCloseOnBackgroundClick = true;
            this.IsAnimating = true;

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
