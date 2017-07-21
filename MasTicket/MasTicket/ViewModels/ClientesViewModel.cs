using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MasTicket
{
	public class UsuariosViewModel : BaseViewModel
	{
		private bool parasubir, parasubirped;
		private ObservableCollection<Cliente> lsclientes;
		public ObservableCollection<Cliente> lsClientes { 
			get{
				return(new ObservableCollection<Cliente> (parasubir ? App.db.SelClientesPSubir () : (parasubirped ? App.db.SelClientesCPedidosSubir() : App.db.SelClientes ())));
			}
		}

		public ObservableCollection<Pago> lsPagos { get; set;}

		public UsuariosViewModel(Page page, bool ps, bool psp) : base (page)
		{
			parasubir = ps;
			parasubirped = psp;
			lsPagos = new ObservableCollection<Pago> (App.db.SelPagos ());

			if (parasubir)
				lsclientes = new ObservableCollection<Cliente> (App.db.SelClientesPSubir());
			else if (parasubirped)
				lsclientes = new ObservableCollection<Cliente> (App.db.SelClientesCPedidosSubir());
			else
				lsclientes = new ObservableCollection<Cliente> (App.db.SelClientes());
		}

		public async Task DeleteStore(Cliente store)
		{
			if (IsBusy)
				return;
			IsBusy = true;
            var showAlert = false;
			try {
			} catch(Exception ex) {
                showAlert = true;
				Xamarin.Insights.Report (ex);
			}
			finally {
				IsBusy = false;
			}
            if(showAlert)
                await page.DisplayAlert("Uh Oh :(", "Unable to remove store, please try again", "OK");
		}

		public void Descargas(wsClientes wsc){
			string json = "";
			json = wsc.getListaClientes ("", "", (App.Current.Properties ["usrlogged"] as Usuario).IdUsuario);
			List<Cliente> lc = JsonConvert.DeserializeObject<List<Cliente>> (json);
			App.db.DescargaClientes (lc);
		}

		private Command getStoresCommand;
		public Command GetStoresCommand
		{
			get {
				return getStoresCommand ??
					(getStoresCommand = new Command (async () => await ExecuteGetStoresCommand (), () => {return !IsBusy;}));
			}
		}

		private async Task ExecuteGetStoresCommand()
		{
			if (IsBusy)
				return;
			IsBusy = true;
			GetStoresCommand.ChangeCanExecute ();
            var showAlert = false;
			try{
			}
			catch(Exception ex) {
                showAlert = true;
				Xamarin.Insights.Report (ex);
			}
			finally {
				IsBusy = false;
				GetStoresCommand.ChangeCanExecute ();
			}
            if(showAlert)
                await page.DisplayAlert("Uh Oh :(", "Unable to gather stores.", "OK");
		}

		private void Sort()
		{
		}
	}

}

