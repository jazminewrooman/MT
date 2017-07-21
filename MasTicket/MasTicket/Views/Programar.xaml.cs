using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceOrientation.Forms.Plugin.Abstractions;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using Rg.Plugins.Popup.Extensions;
using Acr.UserDialogs;

namespace MasTicket
{
    public partial class Programar : ContentPage
    {
        CalendarView _calendarView = new CalendarView();
        List<FechaProgramada> lsfechas = new List<FechaProgramada>();
        private double width;
        private double height;
        RecargasViewModel rvm;
        List<int> lsDias = new List<int>();

        private void CambiaOrientacion()
        {
            IDeviceOrientation _deviceOrientationSvc = DependencyService.Get<IDeviceOrientation>();
            DeviceOrientations dvcor = _deviceOrientationSvc.GetOrientation();
            if (dvcor == DeviceOrientations.Landscape)
            //if (width > height) //apaisado
            {
                grdMain.ColumnDefinitions.Clear();
                grdMain.RowDefinitions.Clear();
                grdMain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60, GridUnitType.Star) });
                grdMain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40, GridUnitType.Star) });
                grdMain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100, GridUnitType.Star) });
                grdMain.Children.Add(_calendarView, 0, 0);
                grdMain.Children.Add(lvFechas, 1, 0);
            }
            else //portrait
            {
                grdMain.ColumnDefinitions.Clear();
                grdMain.RowDefinitions.Clear();
                grdMain.ColumnDefinitions.Add(
                        new ColumnDefinition { Width = new GridLength(100, GridUnitType.Star) }
                    );
                Enumerable.Range(1, 2).ToList().ForEach(x =>
                    grdMain.RowDefinitions.Add(
                        new RowDefinition { Height = new GridLength(40, GridUnitType.Star) }
                    ));
                grdMain.Children.Add(_calendarView, 0, 0);
                grdMain.Children.Add(lvFechas, 0, 1);
            }

        }

        protected override void OnAppearing()
        {
            lvFechas.BeginRefresh();
            lvFechas.IsGroupingEnabled = false;
            lvFechas.ItemsSource = lsfechas;
            lvFechas.EndRefresh();

            base.OnAppearing();
        }

        public Programar(RecargasViewModel r)
        {
            try
            {
                InitializeComponent();
                rvm = r;
				NavigationPage.SetBackButtonTitle(this, "");

				MessagingCenter.Subscribe<DeviceOrientationChangeMessage>(this, DeviceOrientationChangeMessage.MessageId, (message) =>
                {
                    CambiaOrientacion();
                });
                Title = "Recarga automatica";

                RelativeLayout _relativeLayout = new RelativeLayout()
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                };
                _calendarView = new CalendarView()
                {
                    //BackgroundColor = Color.Blue
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    MinDate = CalendarView.FirstDayOfMonth(DateTime.Now),
                    MaxDate = CalendarView.LastDayOfMonth(DateTime.Now),
                    HighlightedDateBackgroundColor = Color.FromHex("#553191"),
                    ShouldHighlightDaysOfWeekLabels = false,
                    SelectionBackgroundStyle = CalendarView.BackgroundStyle.CircleFill,
                    SelectedDateBackgroundColor = Color.FromHex("#e35102"),
                    TodayBackgroundStyle = CalendarView.BackgroundStyle.CircleOutline,
                    HighlightedDaysOfWeek = new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday },
                    ShowNavigationArrows = true,
                    MonthTitleFont = Font.OfSize("Open 24 Display St", NamedSize.Medium)
                };
                //grdMain.Children.Add(_calendarView, 0, 0);
                //grdMain.Children.Add(lvFechas, 1, 0);
                _calendarView.DateSelected += (object sender, DateTime e) =>
                {
                    lsfechas.Add(new FechaProgramada() { strfecha = "    Se selecciono el dia " + e.Day.ToString() + " de cada mes" });
                    lsDias.Add(e.Day);
                    lvFechas.BeginRefresh();
                    lvFechas.ItemsSource = null;
                    lvFechas.ItemsSource = lsfechas;
                    lvFechas.EndRefresh();
                };
                CambiaOrientacion();

				rvm.RecargaAltaErr += async (s, e) =>
				{
					UserDialogs.Instance.HideLoading();
					await DisplayAlert("Error", "Ocurrio un error, vuelva a intentar\n", "OK");
					try
					{
						_calendarView = null;
						//await App.Nav.PopToRootAsync(Constantes.animated);
						await App.Nav.PopAsync(Constantes.animated);
					}
					catch (Exception ex)
					{
					}
				};
				rvm.RecargaAltaProg += async (s, e) =>
				{
					UserDialogs.Instance.HideLoading();
					await DisplayAlert("Aviso", "Se programo su recarga", "OK");
					try
					{
						_calendarView = null;
						//await App.Nav.PopToRootAsync(Constantes.animated);
						await App.Nav.PopAsync(Constantes.animated);
					}
					catch (Exception ex)
					{
					}
				};

				btnGuardar.Clicked += (sender, ea) =>
                {
                    string dias = "";
                    for (int i = 0; i < lsDias.Count; i++)
                        dias += lsDias[i] + ",";
                    //dias = dias.Substring(0, dias.Length - 1);
                    rvm.DiasRecarga = dias;
					UserDialogs.Instance.ShowLoading("Programando...");
                    rvm.AltaProg();
					//await App.Nav.PopAsync(Constantes.animated);
                };

                
            }catch(Exception e)
            {

            }
        }


    }

    public class FechaProgramada
    {
        public string strfecha { get; set; }
    }
}