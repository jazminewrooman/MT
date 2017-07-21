using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace MasTicket
{
	public class EntryDone : Entry
	{
	}

    public class EntryAnim : Entry
    {
        public EntryAnim()
        {

        }

        public async Task AnimateWinAsync()
        {
            uint length = 1500;
            //var taskDelay = Task.Delay(2000);

            //Color c = this.BackgroundColor;
            //Color ct = this.TextColor;
            //this.BackgroundColor = Color.FromHex("#e35102");
            //this.TextColor = Color.FromHex("#ffffff");
            for (int i = 0; i < 6; i++)
            {
                await this.TranslateTo(20, 0, 100, Easing.BounceIn);
                await this.TranslateTo(0, 0, 100, Easing.BounceIn);
            }
            //this.BackgroundColor = c;
            //this.TextColor = ct;
            //for(int i = 0; i < 10; i++)
            //    await Task.WhenAll(this.TranslateTo(10, 0, 100), this.TranslateTo(-10, 0, 100));

            //.ScaleTo(3, length), this.RotateTo(180, length));
            //label.Text = isReverse ? normText : winText;
            //await Task.WhenAll(this.ScaleTo(1, length), this.RotateTo(360, length));
            this.Rotation = 0;
        }
    }

    public class SearchBarAnim : SearchBar
    {
        public SearchBarAnim()
        {

        }

        public async Task AnimateWinAsync()
        {
            //Color c = this.BackgroundColor;
            //Color ct = this.TextColor;
            //this.BackgroundColor = Color.FromHex("#e35102");
            //this.TextColor = Color.FromHex("#ffffff");
            for (int i = 0; i < 6; i++)
            {
                await this.TranslateTo(20, 0, 100, Easing.BounceIn);
                await this.TranslateTo(0, 0, 100, Easing.BounceIn);
            }
            //this.BackgroundColor = c;
            //this.TextColor = ct;
            this.Rotation = 0;
        }
    }

    public class ImgAnim : Image
    {
        public ImgAnim()
        {

        }

        public async Task AnimateWinAsync()
        {
            uint length = 1500;
            //var taskDelay = Task.Delay(2000);

            //Color c = this.BackgroundColor;
            //Color ct = this.TextColor;
            //this.BackgroundColor = Color.FromHex("#e35102");
            //this.TextColor = Color.FromHex("#ffffff");
            for (int i = 0; i < 5; i++)
            {
                await this.TranslateTo(0, 10, 400, Easing.CubicIn);
                await this.TranslateTo(0, 0, 400, Easing.CubicOut);
            }
            //this.BackgroundColor = c;
            //this.TextColor = ct;
            //for(int i = 0; i < 10; i++)
            //    await Task.WhenAll(this.TranslateTo(10, 0, 100), this.TranslateTo(-10, 0, 100));

            //.ScaleTo(3, length), this.RotateTo(180, length));
            //label.Text = isReverse ? normText : winText;
            //await Task.WhenAll(this.ScaleTo(1, length), this.RotateTo(360, length));
            this.Rotation = 0;
        }
    }
}
