using System;
using Xamarin.Forms;
using System.Linq;

namespace MasTicket
{
    public class MainPage : MasterDetailPage
    {
		public void Refresh()
		{
			Page p = new menu();
			Master = p;
		}

        public MainPage()
        {
            try
            {
                Page p;
                p = new menu();
                (p as menu).Menu.ItemSelected += (sender, e) => NavigateTo(e.SelectedItem as MenuItem);
                Master = p;
				//var det = new NavigationPage(https))

				Recarga r = App.db.SelRecargas().OrderByDescending(x => x.idrecarga).FirstOrDefault();
				var det = new NavigationPage(new CargarSaldo(r)) 
                {
					Title = "Así Compras",
                    BarTextColor = Color.White,
                    BarBackgroundColor = Color.FromHex("#e35102"),
                };
                Detail = det;
                App.Nav = det.Navigation;
                MasterBehavior = MasterBehavior.Popover;
            }
            catch (Exception e)
            {
            }
        }

        void NavigateTo(MenuItem menu)
        {
            try
            {
                Page displayPage = (Page)Activator.CreateInstance(menu.TargetType);
                var det = new NavigationPage(displayPage)
                {
					Title = "Asi Compras",
                    BarTextColor = Color.White,
                    BarBackgroundColor = Color.FromHex("#e35102"),
                };
                Detail = det;
                App.Nav = det.Navigation;
                IsPresented = false;
            }
            catch (Exception e)
            {
            }
        }

    }
}

