#region Usings

using System;
using System.ComponentModel;
using System.Drawing;
using CaveBirdLabs.Forms;
using CaveBirdLabs.Forms.PlatformiOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

#endregion

[assembly: ExportRenderer(typeof(CxPagedCarouselPage), typeof(CaveBirdLabs.Forms.PlatformiOS.CxPagedCarouselPageRenderer))]

namespace CaveBirdLabs.Forms.PlatformiOS
{
	public class CxPagedCarouselPageRenderer : CarouselPageRenderer
	{
		#region Declarations

		private bool _currentPageSetByUser = false;
		private CxPagedCarouselPage _pagedCarouselPage;
		private UIPageControl _uiPageControl;
		private UIView _view;

		#endregion

		#region Constructors

		public CxPagedCarouselPageRenderer() { }

		#endregion

		#region Event Handlers

		private void OnPagedCarouselPagePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "IsPagerVisible":
					_uiPageControl.Hidden = !_pagedCarouselPage.IsPagerVisible;
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
				case "CurrentPage":
					if (!_currentPageSetByUser)
						_uiPageControl.CurrentPage = SelectedIndex;
					break;
			}
		}

		private void OnUIPageControlValueChanged(object sender, EventArgs eventArgs)
		{
			_currentPageSetByUser = true;
			_pagedCarouselPage.CurrentPage = _pagedCarouselPage.Children[(int)_uiPageControl.CurrentPage];
			_currentPageSetByUser = false;
		}

		#endregion

		#region Public Methods

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			_uiPageControl.Pages = _pagedCarouselPage.Children.Count;
			_pagedCarouselPage.PagerMinimumWidth = (float)_uiPageControl.SizeForNumberOfPages(_uiPageControl.Pages).Width;
			LayoutPageControl();
		}

		#endregion

		#region Private Methods

		private void LayoutPageControl()
		{
			float width = (float)_view.Frame.Width - (float)_pagedCarouselPage.PagerPadding.Left - (float)_pagedCarouselPage.PagerPadding.Right;
			
			const float HEIGHT = 36; // default height of the UIPageControl

			float x = 0;
			switch (_pagedCarouselPage.PagerXAlign)
			{
				case TextAlignment.Center:
					if (_view.Frame.Width != width)
						x = (float)_pagedCarouselPage.PagerPadding.Left + (width / 2);

					break;
				case TextAlignment.End:
					x = (float)_view.Frame.Width - width - (float)_pagedCarouselPage.PagerPadding.Right;
					break;
				case TextAlignment.Start:
					x = (float)_pagedCarouselPage.PagerPadding.Left;
					break;
			}

			float y = 0;
			switch (_pagedCarouselPage.PagerYAlign)
			{
				case TextAlignment.Center:
					// ignore top and bottom padding
					y = (float)(_view.Frame.Height - HEIGHT) / 2;
					_uiPageControl.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
					break;
				case TextAlignment.End:
					y = (float)_view.Frame.Height - (float)_pagedCarouselPage.PagerPadding.Bottom - HEIGHT;
					_uiPageControl.HorizontalAlignment = UIControlContentHorizontalAlignment.Right;
					break;
				case TextAlignment.Start:
					y = (float)_pagedCarouselPage.PagerPadding.Top;
					_uiPageControl.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
					break;
			}

			_uiPageControl.Frame = new RectangleF(x, y, width, HEIGHT);
		}

		private void SetCurrentPageIndicatorTintColor()
		{
			_uiPageControl.CurrentPageIndicatorTintColor = _pagedCarouselPage.SelectedPagerItemColor != Color.Default ? _pagedCarouselPage.SelectedPagerItemColor.ToUIColor() : UIColor.White;
		}

		private void SetPageIndicatorTintColor()
		{
			_uiPageControl.PageIndicatorTintColor = _pagedCarouselPage.PagerItemColor != Color.Default ? _pagedCarouselPage.PagerItemColor.ToUIColor() : UIColor.Gray;
		}

		#endregion

		#region Protected Methods

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				_pagedCarouselPage.PropertyChanged -= OnPagedCarouselPagePropertyChanged;
				_uiPageControl.ValueChanged -= OnUIPageControlValueChanged;
			}
		}

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			_pagedCarouselPage = (CxPagedCarouselPage)e.NewElement;
			_view = NativeView;

			_uiPageControl = new UIPageControl();
			SetPageIndicatorTintColor();
			SetCurrentPageIndicatorTintColor();

			_view.Add(_uiPageControl);

			_pagedCarouselPage.PropertyChanged += OnPagedCarouselPagePropertyChanged;
			_uiPageControl.ValueChanged += OnUIPageControlValueChanged;
		}

		#endregion

	}
}