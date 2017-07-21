using Android.Support.V4.View;

namespace CaveBirdLabs.Forms.Platform.Android
{
	// https://github.com/Cheesebaron/ViewPagerIndicator/blob/master/Library/Interfaces/IPageIndicator.cs
	public interface IPageIndicator : ViewPager.IOnPageChangeListener
	{
		void SetViewPager(ViewPager view);
		void SetViewPager(ViewPager view, int initialPosition);
		int CurrentItem { get; set; }
		void SetOnPageChangeListener(ViewPager.IOnPageChangeListener listener);
		void NotifyDataSetChanged();
	}
}

