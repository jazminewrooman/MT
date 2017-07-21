using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;

namespace MasTicket
{
    public partial class ListaOpciones : ContentPage
    {
        public int IdOpc { set; get; }
        private ObservableCollection<Opcion> lsOpciones;
        public ObservableCollection<Opcion> LsOpciones
        {
            set { lsOpciones = value; }
        }

		public ListaOpciones(ObservableCollection<Opcion> ls, string title, int tipo, bool agrupados = false)
        {
            InitializeComponent();
            lsOpciones = new ObservableCollection<Opcion>();

            lsOpciones = ls;
            Title = title;

			if (agrupados)
			{
				ObservableCollection<GrupoOpc> grptipos = new ObservableCollection<GrupoOpc>();
				var difids = ls.Select(x => x.idagrupador).Distinct();
				foreach (int i in difids)
					grptipos.Add(new GrupoOpc(ls.Where(x => x.idagrupador == i).FirstOrDefault().agrupador, i, ls.Where(x => x.idagrupador == i)));
				lvOpciones.IsGroupingEnabled = true;
				lvOpciones.ItemsSource = grptipos;
			}
			else {
				lvOpciones.IsGroupingEnabled = false;
				lvOpciones.ItemsSource = lsOpciones;
			}
            lvOpciones.ItemSelected += async (sender, e) => {
                if (e.SelectedItem == null)
                    return;

                if (tipo == 1)
                {
                    var cs = App.Nav.NavigationStack.OfType<CargarSaldo>().First();
                    (cs as CargarSaldo).SetVal(IdOpc, e.SelectedItem as Opcion);
                    await App.Nav.PopAsync(Constantes.animated);
                }
                if (tipo == 2)
                {
                    var cs = App.Nav.NavigationStack.OfType<NuevaTarjeta>().First();
                    (cs as NuevaTarjeta).SetVal(IdOpc, e.SelectedItem as Opcion);
                    await App.Nav.PopAsync(Constantes.animated);
                }

                ((ListView)sender).SelectedItem = null;
            };
        }
    }

    public class Opcion
    {
        public int idopc { get; set; }
        public string opc { get; set; }
        public string imgopc { get; set; }
		public int idagrupador { get; set; }
		public string agrupador { get; set; }
    }

	public class GrupoOpc : ObservableCollection<Opcion>
	{
		public String Name { get; private set; }
		public int Tipo { get; private set; }

		public GrupoOpc(String Name, int tipo, IEnumerable<Opcion> col)
		{
			this.ClearItems();
			foreach (Opcion tmp in this.Items.Union(col).ToList())
				this.Add(tmp);
			this.Name = Name;
			this.Tipo = tipo;
		}

		public GrupoOpc(IEnumerable<Opcion> col)
		{
			this.ClearItems();
			foreach (Opcion tmp in this.Items.Union(col).ToList())
				this.Add(tmp);
		}
	}

	public class MuestraImgConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool ret = false;
			if (value == null)
				ret = false;
			else {
				if (String.IsNullOrEmpty(value.ToString()))
					ret = false;
				else
					ret = true;
			}
			return (ret);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}
