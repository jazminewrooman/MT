using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;

using Xamarin.Forms;
using Acr.UserDialogs;
using Rg.Plugins.Popup.Extensions;

namespace MasTicket
{
    public partial class Contactos : ContentPage
    {
        RecargasViewModel rvm;
        List<letragrupo> Groups = new List<letragrupo>();
        List<Contacto> ls;
        const string phoneRegex = @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}";

		public List<Contacto> GetLista()
		{
			return (ls);
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
			if (ls.Count() == 0)
				UserDialogs.Instance.ShowLoading("Cargando...");
		}

        async void LeeContactos()
        {
			try
			{
				await Task.Run(() =>
				{
					if (ls.Count() == 0)
						ls = DependencyService.Get<IContactos>().GetLista().OrderBy(x => x.Name).ToList();
					Device.BeginInvokeOnMainThread(async () =>
					{
						UserDialogs.Instance.HideLoading();
						await MuestraContactos();
					});
				});
			}
			catch (Exception e)
			{
			}
        }

        public Contactos()
        {
			try
			{
                InitializeComponent();
                
                rvm = new RecargasViewModel();
				ls = new List<Contacto>();
				Title = "Contactos";
				ToolbarItems.Add(new ToolbarItem("Ayuda", "ayuda2.png", () =>
				{
					var page = new InfoAyuda("infocontactos.html");
					Navigation.PushPopupAsync(page);
				}));

				LeeContactos();
			}
			catch (Exception e)
			{
			}
        }

        private async Task MuestraContactos()
        {
			List<RecargaFrecuente> lrf;
            List<char> letras = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList();
			try
			{
				Groups.Clear();
				lrf = rvm.SelRecargasFrecuentes();
				IEnumerable<Contacto> lsrf = lrf.Select(x => new Contacto { Name = x.contactorecarga, Number = x.numerorecarga, Photo = null });
				Groups.Add(new letragrupo("Frecuentes", lsrf));
				foreach (char c in letras)
				{
					Groups.Add(new letragrupo(c.ToString(), ls.Where(x => x.Name.StartsWith(c.ToString())).ToList()));
				}
				lvContactos.ItemSelected += async (sender, e) =>
				{
					if (e.SelectedItem == null)
						return;
					var cs = App.Nav.NavigationStack.OfType<CargarSaldo>().FirstOrDefault();
					Contacto c = (e.SelectedItem as Contacto);
					RecargaFrecuente rf = lrf.Where(x => x.numerorecarga == c.Number).FirstOrDefault(); //App.db.SelRecargaFrecuente(c.Number);
					if (cs == null)
					{
						if (rf == null)
							cs = new CargarSaldo(new Recarga { numerorecarga = c.Number, contactorecarga = c.Name });
						else
							cs = new CargarSaldo(rf);
						await App.Nav.PopAsync(Constantes.animated);
						await App.Nav.PushAsync(cs, Constantes.animated);
					}
					else {
						if (rf == null)
							cs.CargaDeAgenda(new Recarga { numerorecarga = c.Number, contactorecarga = c.Name });
						else
							cs.CargaDeAgenda(rf);
						await App.Nav.PopAsync(Constantes.animated);
					}

					((ListView)sender).SelectedItem = null;
				};
				lvContactos.BeginRefresh();
				lvContactos.IsGroupingEnabled = true;
				lvContactos.ItemsSource = null;
				lvContactos.ItemsSource = Groups;
				lvContactos.EndRefresh();

				srcBuscar.TextChanged += (object sender, TextChangedEventArgs e) =>
				{
					string filtro = e.NewTextValue;
					lvContactos.BeginRefresh();
					if (String.IsNullOrEmpty(filtro))
					{
						lvContactos.ItemsSource = Groups;
					}
					else
					{
						if (filtro.Length == 1)
						{
							List<letragrupo> llg = Groups.Where(x => x.letra.ToUpper() == filtro.ToUpper()).ToList();
							lvContactos.ItemsSource = llg;
						}
						else
						{
							List<letragrupo> lg;
							IEnumerable<Contacto> iec = ls.Where(x => (x.Name.ToUpper().Contains(filtro.ToUpper()) || (x.Number.Contains(filtro.ToUpper()))));
							if (iec.Count() > 0)
							{
								lg = new List<letragrupo>() { new letragrupo("", iec) };
								lvContactos.ItemsSource = lg;
							}
							else
							{
								if (Regex.IsMatch(filtro, phoneRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
								{
									List<Contacto> lc = new List<Contacto>() { new Contacto() { Id = "99999", Name = "Nuevo contacto", Number = filtro, Photo = null } };
									lg = new List<letragrupo>() { new letragrupo("Nuevo contacto", lc.AsEnumerable()) };
									lvContactos.ItemsSource = lg;
								}
							}
						}
					}
					lvContactos.EndRefresh();
				};
			}
			catch (Exception e)
			{

			}
			finally
			{
				UserDialogs.Instance.HideLoading();
			}
        }

    }
    

    public class letragrupo : ObservableCollection<Contacto>
    {
        public string letra { get; set; }
        
        public letragrupo(string letra, IEnumerable<Contacto> col)
        {
            this.ClearItems();
            foreach (Contacto tmp in this.Items.Union(col).ToList())
                this.Add(tmp);
            this.letra = letra;
        }

        public IEnumerable<Contacto> buscar(string c)
        {
            return (this.Items.Where(x => x.Name.ToUpper() == c));
        } 
    }

    //public class letragrupoHist : ObservableCollection<Historico>
    //{
    //    public string letra { get; set; }

    //    public letragrupoHist(string letra, IEnumerable<Historico> col)
    //    {
    //        this.ClearItems();
    //        foreach (Historico tmp in this.Items.Union(col).ToList())
    //            this.Add(tmp);
    //        this.letra = letra;
    //    }

    //    //public IEnumerable<Historico> buscar(string c)
    //    //{
    //    //    return (this.Items.Where(x => x.Name.ToUpper() == c));
    //    //}
    //}

    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ImageSource retSource = null;
            if (value != null)
            {
                byte[] imageAsBytes = (byte[])value;
                retSource = ImageSource.FromStream(() => new MemoryStream(imageAsBytes));
            }
            else
            {
                // Default image in case the contact doesn't have any thumbnail
                retSource = ImageSource.FromFile("profle.png");
            }

            return retSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
