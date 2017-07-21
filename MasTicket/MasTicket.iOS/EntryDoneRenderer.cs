using System;
using System.Reflection;

using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using CoreGraphics;
using MasTicket.iOS;
using MasTicket;

[assembly: ExportRenderer(typeof(EntryDone), typeof(EntryDoneRenderer))]
namespace MasTicket.iOS
{
	public class EntryDoneRenderer : EntryRenderer
	{
		private MethodInfo baseEntrySendCompleted = null;

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			var toolbar = new UIToolbar(new CGRect(0.0f, 0.0f, Control.Frame.Size.Width, 44.0f));

			toolbar.Items = new[]
			{
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
				new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
					Control.ResignFirstResponder();
					((IEntryController)Element).SendCompleted();

				//	Type baseEntry = this.Element.GetType();
				//if(baseEntrySendCompleted == null)
				//{
    //                // use reflection to find our method
    //                baseEntrySendCompleted = baseEntry.GetMethod("SendCompleted",BindingFlags.NonPublic|BindingFlags.Instance);
				//}

				//try
				//{
				//	baseEntrySendCompleted.Invoke(this.Element,null);
				//}
				//catch (Exception ex)
				//{
    //                // handle the invoke error condition    
    //            }
				}
				                   )
			};

			this.Control.InputAccessoryView = toolbar;
		}
	}
}
