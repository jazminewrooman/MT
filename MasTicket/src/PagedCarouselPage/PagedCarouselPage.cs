#region Usings

using Xamarin.Forms;

#endregion

namespace CaveBirdLabs.Forms
{
	public class CxPagedCarouselPage : CarouselPage
	{
		#region Declarations

		public static readonly BindableProperty IsPagerVisibleProperty =
			BindableProperty.Create<CxPagedCarouselPage, bool>(
				p => p.IsPagerVisible, true);

		public static readonly BindableProperty PagerItemColorProperty =
			BindableProperty.Create<CxPagedCarouselPage, Color>(
				p => p.PagerItemColor, Color.Default);

		public static readonly BindableProperty PagerMinimumWidthProperty =
			BindableProperty.Create<CxPagedCarouselPage, double>(
				p => p.PagerMinimumWidth, 0);

		public static readonly BindableProperty PagerPaddingProperty =
			BindableProperty.Create<CxPagedCarouselPage, Thickness>(
				p => p.PagerPadding, new Thickness(0, 0, 0, 30));

		public static readonly BindableProperty PagerXAlignProperty =
			BindableProperty.Create<CxPagedCarouselPage, TextAlignment>(
				p => p.PagerXAlign, TextAlignment.Center);

		public static readonly BindableProperty PagerYAlignProperty =
			BindableProperty.Create<CxPagedCarouselPage, TextAlignment>(
				p => p.PagerYAlign, TextAlignment.End);

		public static readonly BindableProperty SelectedPagerItemColorProperty =
			BindableProperty.Create<CxPagedCarouselPage, Color>(
				p => p.SelectedPagerItemColor, Color.Default);

		#endregion

		#region Public Properties

		public bool IsPagerVisible
		{
			get { return (bool)GetValue(IsPagerVisibleProperty); }
			set { SetValue(IsPagerVisibleProperty, value); }
		}

		public Color PagerItemColor
		{
			get { return (Color)GetValue(PagerItemColorProperty); }
			set { SetValue(PagerItemColorProperty, value); }
		}

		public double PagerMinimumWidth
		{
			get { return (double)GetValue(PagerMinimumWidthProperty); }
			set { SetValue(PagerMinimumWidthProperty, value); }
		}

		public Thickness PagerPadding
		{
			get { return (Thickness)base.GetValue(PagerPaddingProperty); }
			set { base.SetValue(PagerPaddingProperty, value); }
		}

		public TextAlignment PagerXAlign
		{
			get { return (TextAlignment)base.GetValue(PagerXAlignProperty); }
			set { base.SetValue(PagerXAlignProperty, value); }
		}

		public TextAlignment PagerYAlign
		{
			get { return (TextAlignment)base.GetValue(PagerYAlignProperty); }
			set { base.SetValue(PagerYAlignProperty, value); }
		}

		public Color SelectedPagerItemColor
		{
			get { return (Color)GetValue(SelectedPagerItemColorProperty); }
			set { SetValue(SelectedPagerItemColorProperty, value); }
		}

		#endregion
	}
}