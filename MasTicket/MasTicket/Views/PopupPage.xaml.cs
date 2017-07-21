using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using XLabs.Forms.Controls;
using Acr.UserDialogs;
using Splat;

namespace MasTicket
{
	public partial class PopupPage : ContentPage
	{
		public PopupPage ()
		{
			InitializeComponent ();
            this.OpenButton.Clicked += OpenButtonClicked;
        }

        protected virtual void Result(string msg)
        {
            UserDialogs.Instance.Alert(msg);
        }

        void OpenButtonClicked(object sender, EventArgs e)
        {
            
                var cfg = new ActionSheetConfig().SetTitle("Test Title");

                var testImage = BitmapLoader.Current.LoadFromResource("icon.png", null, null).Result;

                for (var i = 0; i < 5; i++)
                {
                    var display = (i + 1);
                    cfg.Add("Option " + display, () => this.Result($"Option {display} Selected"), testImage);
                }
                cfg.SetDestructive(action: () => this.Result("Destructive BOOM Selected"));
                cfg.SetCancel(action: () => this.Result("Cancel Selected"));

                var disp = UserDialogs.Instance.ActionSheet(cfg);
                //if (this.AutoCancel)
                //{
                //    Task.Delay(TimeSpan.FromSeconds(3))
                //        .ContinueWith(x => disp.Dispose());
                //}
                
        }
    }
}
