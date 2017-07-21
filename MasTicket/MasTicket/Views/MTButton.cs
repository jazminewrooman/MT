using System;
using Xamarin.Forms;
using System.Diagnostics;

namespace MasTicket
{
	public class MTButton : Button
	{
		public MTButton() : base()
		{
            this.BorderRadius = 3;
            this.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button));

            const int _animationTime = 100;
			Clicked += async (sender, e) =>
			{
				var btn = (MTButton)sender;
				await btn.ScaleTo(1.2, _animationTime);
				btn.ScaleTo(1, _animationTime);
			};
		}
	}
}