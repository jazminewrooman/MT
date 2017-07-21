using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Acr.UserDialogs;
using Newtonsoft.Json;

namespace MasTicket
{
	public partial class OlvideNip : ContentPage
	{
		public OlvideNip()
		{
			InitializeComponent();

			Title = "Olvide mi NIP";
			btnEntrar.Clicked += async (s, e) =>
			{
				string email = "";
				if (!String.IsNullOrEmpty(edtCorreo.Text))
					email = check.SafeSqlLiteral(edtCorreo.Text.Trim());
				if (email == "")
					await DisplayAlert("Error", "No existe ese usuario y/o email", "OK");
				else {
					UserDialogs.Instance.ShowLoading("Cargando...");
					string json = App.WSc.GetUser(0, email, "");
					List<Usuario> lu = JsonConvert.DeserializeObject<List<Usuario>>(json);
					if (lu.Count > 0)
					{
						UserDialogs.Instance.HideLoading();
						App.WS.EnviaMailRecordatorioAsync(lu.FirstOrDefault().idusuario);
						await DisplayAlert("Aviso", "Se envio su nip al correo registrado", "OK");
						await App.Nav.PopAsync(Constantes.animated);
					}
					else {
						UserDialogs.Instance.HideLoading();
						await DisplayAlert("Error", "No existe ese usuario y/o email", "OK");
					}
				}
			};
			btnCancelar.Clicked += async (s, e) =>
			{
				await App.Nav.PopAsync(Constantes.animated);
			};
		}
	}
}

