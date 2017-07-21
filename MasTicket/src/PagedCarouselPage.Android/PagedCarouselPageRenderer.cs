#region Usings

using System.ComponentModel;

using Android.Support.V4.View;
using Android.Views;

using CaveBirdLabs.Forms;
using CaveBirdLabs.Forms.Platform.Android;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

#endregion

[assembly : ExportRenderer(typeof (CxPagedCarouselPage), typeof (CxPagedCarouselPageRenderer))]

namespace CaveBirdLabs.Forms.Platform.Android
{
	public class CxPagedCarouselPageRenderer : CarouselPageRenderer
	{
		#region Declarations

		private CirclePageIndicator _circlePageIndicator;
		private CxPagedCarouselPage _pagedCarouselPage;
		private ViewPager _viewPager;

		#endregion

		#region Constructors

		public CxPagedCarouselPageRenderer() {}

		#endregion

		#region Event Handlers

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			switch (e.PropertyName)
			{
			case "IsPagerVisible":
				_viewPager.Visibility = _pagedCarouselPage.IsPagerVisible ? ViewStates.Visible : ViewStates.Gone;
				break;
			case "PagerItemColor":
				SetPageIndicatorTintColor();
				break;
			case "SelectedPagerItemColor":
				SetCurrentPageIndicatorTintColor();
				break;
			case "PagerPadding":
			case "PagerXAlign":
			case "PagerYAlign":
				LayoutPageControl();
				break;
			}
		}

		#endregion

		#region Private Methods

		private void LayoutPageControl()
		{
			var density = Resources.DisplayMetrics.Density;

			float width = ViewGroup.MeasuredWidth - (float) _pagedCarouselPage.PagerPadding.Left - (float) _pagedCarouselPage.PagerPadding.Right;

			float height = 30*density; // default height of the UIPageControl

			float x = 0;
			switch (_pagedCarouselPage.PagerXAlign)
			{
				case Xamarin.Forms.TextAlignment.Center:
					if (ViewGroup.MeasuredWidth != width)
						x = (float) _pagedCarouselPage.PagerPadding.Left + (width/2);
				_circlePageIndicator.Centered = true;
					break;
				case Xamarin.Forms.TextAlignment.End:
					x = ViewGroup.MeasuredWidth - width - (float) _pagedCarouselPage.PagerPadding.Right;
				_circlePageIndicator.Centered = false;
					break;
				case Xamarin.Forms.TextAlignment.Start:
					x = (float) _pagedCarouselPage.PagerPadding.Left;
				_circlePageIndicator.Centered = false;
					break;
			}

			float y = 0;
			switch (_pagedCarouselPage.PagerYAlign)
			{
				case Xamarin.Forms.TextAlignment.Center:
					// ignore top and bottom padding
					y = (ViewGroup.MeasuredHeight - height)/2;
					
					break;
				case Xamarin.Forms.TextAlignment.End:
					y = ViewGroup.MeasuredHeight - (float) _pagedCarouselPage.PagerPadding.Bottom - height;
					
					break;
				case Xamarin.Forms.TextAlignment.Start:
					y = (float) _pagedCarouselPage.PagerPadding.Top;
					
					break;
			}

			_circlePageIndicator.Layout((int) x, (int) y, (int) (width + x), (int) (height + y));
		}

		private void SetCurrentPageIndicatorTintColor()
		{
			_circlePageIndicator.PageColor = _pagedCarouselPage.SelectedPagerItemColor != Color.Default ? _pagedCarouselPage.SelectedPagerItemColor.ToAndroid() : global::Android.Graphics.Color.White;
		}

		private void SetPageIndicatorTintColor()
		{
			_circlePageIndicator.FillColor = _pagedCarouselPage.PagerItemColor != Color.Default ? _pagedCarouselPage.PagerItemColor.ToAndroid() : global::Android.Graphics.Color.Transparent;
		}

		#endregion

		#region Protected Methods

		protected override void OnAttachedToWindow()
		{
			base.OnAttachedToWindow();
			_circlePageIndicator.SetViewPager(_viewPager);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<CarouselPage> e)
		{
			base.OnElementChanged(e);

			_pagedCarouselPage = (CxPagedCarouselPage) Element;

			_viewPager = null;

			for (int i = 0; i < ViewGroup.ChildCount; i++)
			{
				_viewPager = ViewGroup.GetChildAt(i) as ViewPager;
				if (_viewPager != null)
					break;
			}
			if (_viewPager == null)
				return;

			var density = Resources.DisplayMetrics.Density;

			_circlePageIndicator = new CirclePageIndicator(base.Context);
			_circlePageIndicator.SetPadding(5, 5, 5, 5);
			_circlePageIndicator.Radius = 5*density;
			SetPageIndicatorTintColor();
			SetCurrentPageIndicatorTintColor();

			AddView(_circlePageIndicator);
			_circlePageIndicator.BringToFront();

		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			base.OnLayout(changed, l, t, r, b);
			LayoutPageControl();
		}

		#endregion
	}
}