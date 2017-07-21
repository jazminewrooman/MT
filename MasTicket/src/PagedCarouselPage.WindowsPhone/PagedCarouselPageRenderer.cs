#region Usings

using CaveBirdLabs.Forms;
using CaveBirdLabs.Forms.Platform.WindowsPhone;

using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

#endregion

[assembly : ExportRenderer(typeof (CxPagedCarouselPage), typeof (CxPagedCarouselPageRenderer))]

namespace CaveBirdLabs.Forms.Platform.WindowsPhone
{
	public class CxPagedCarouselPageRenderer : CarouselPageRenderer
	{
		#region Declarations

		private CxPagedCarouselPage _pagedCarouselPage;

		#endregion

		#region Constructors

		public CxPagedCarouselPageRenderer() {}

		#endregion

		#region Protected Methods

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			_pagedCarouselPage = (CxPagedCarouselPage) e.NewElement;
		}

		#endregion
	}
}