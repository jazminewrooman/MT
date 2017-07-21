using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections;

//using Windows.Storage;
using SQLite;

namespace MasTicket
{
	public class BD
	{
		static object locker = new object();
		SQLiteConnection database;

		string DatabasePath
		{
			get
			{
				var sqliteFilename = "mt.db3";
#if __IOS__
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
                string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
                var path = Path.Combine(libraryPath, sqliteFilename);
#else
#if __ANDROID__
				string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
				var path = Path.Combine(documentsPath, sqliteFilename);
#else
					// WinPhone
					var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, sqliteFilename);;
#endif
#endif
				return path;
			}
		}

		public BD()
		{
			database = new SQLiteConnection(DatabasePath, true);
			// create the tables
			database.CreateTable<Usuario>();
			database.CreateTable<RecargaFrecuente>();
			database.CreateTable<SaldoMonedero>();
			database.CreateTable<Tarjeta>();
			database.CreateTable<Recarga>();
			database.CreateTable<RecargaMonedero>();
			database.CreateTable<RecargaProg>();
			database.CreateTable<catPais>();
			database.CreateTable<catEstado>();
			database.CreateTable<catMunicipio>();
			database.CreateTable<catEmisorTC>();
			database.CreateTable<catConfig>();
			database.CreateTable<catOperadora>();
			database.CreateTable<catPaquete>();
			database.CreateTable<catErrores>();

			if (database.Table<SaldoMonedero>().Count() == 0)
			{
				SaldoMonedero mon = new SaldoMonedero() { idmonedero = 1, saldo = 0M, caducidad = DateTime.Now.AddYears(2) };
				database.Insert(mon);
			}
			if (database.Table<catConfig>().Count() == 0)
			{
				catConfig cfg = new catConfig() { idconfig = 1, config = "Ultima vez que actualizo catPaquetes", };
				database.Insert(cfg);
				cfg = new catConfig() { idconfig = 2, config = "Alguna vez loggeo", valor = "0" };
				database.Insert(cfg);

			}
		}

		#region catalogos
		public void AltaConfig(catConfig item)
		{
			lock (locker)
			{
				if ((from i in database.Table<catConfig>() where i.idconfig == item.idconfig select i).Count() > 0)
					database.Update(item);
				else
					database.Insert(item);
			}
		}
		public List<catConfig> SelcatConfig()
		{
			lock (locker)
			{
				return (database.Table<catConfig>().ToList());
			}
		}
		public ObservableCollection<catEmisorTC> SelcatEmisorTC()
		{
			lock (locker)
			{
				return (database.Table<catEmisorTC>().ToObservableCollection());
			}
		}
		public ObservableCollection<catErrores> SelErrores()
		{
			lock (locker)
			{
				return (database.Table<catErrores>().ToObservableCollection());
			}
		}
		public ObservableCollection<catPais> SelPais()
		{
			lock (locker)
			{
				return (database.Table<catPais>().ToObservableCollection());
			}
		}
		public ObservableCollection<catEstado> SelEstado()
		{
			lock (locker)
			{
				return (database.Table<catEstado>().ToObservableCollection());
			}
		}
		public ObservableCollection<catMunicipio> SelMunicipio()
		{
			lock (locker)
			{
				return (database.Table<catMunicipio>().ToObservableCollection());
			}
		}
		public void DescargaErrores(List<catErrores> ls)
		{
			lock (locker)
			{
				database.DeleteAll<catErrores>();
				database.InsertAll(ls);
			}
		}
		public void DescargaPais(List<catPais> ls)
		{
			lock (locker)
			{
				database.DeleteAll<catPais>();
				database.InsertAll(ls);
			}
		}
		public void DescargaEstado(List<catEstado> ls)
		{
			lock (locker)
			{
				database.DeleteAll<catEstado>();
				database.InsertAll(ls);
			}
		}
		public void DescargaMunicipio(List<catMunicipio> ls)
		{
			lock (locker)
			{
				database.DeleteAll<catMunicipio>();
				database.InsertAll(ls);
			}
		}
		public void DescargaEmisorTC(List<catEmisorTC> ls)
		{
			lock (locker)
			{
				database.DeleteAll<catEmisorTC>();
				database.InsertAll(ls);
			}
		}
		public void DescargaOperadora(List<catOperadora> ls)
		{
			lock (locker)
			{
				database.DeleteAll<catOperadora>();
				database.InsertAll(ls);
			}
		}
		public void DescargaPaquete(List<catPaquete> ls)
		{
			lock (locker)
			{
				database.DeleteAll<catPaquete>();
				database.InsertAll(ls);
			}
		}
		public ObservableCollection<catEmisorTC> SelEmisorTC()
		{
			lock (locker)
			{
				return (database.Table<catEmisorTC>().ToObservableCollection());
			}
		}
		public ObservableCollection<catOperadora> SelOperadora()
		{
			lock (locker)
			{
				return (database.Table<catOperadora>().ToObservableCollection());
			}
		}
		public ObservableCollection<catPaquete> SelPaquete()
		{
			lock (locker)
			{
				return (database.Table<catPaquete>().ToObservableCollection());
			}
		}
		#endregion

		#region Tarjetas
		public void DelTarjeta(int idtarjeta)
		{
			lock (locker)
			{
				database.Delete<Tarjeta>(idtarjeta);
			}
		}
		public Tarjeta SelTarjetas(int idtarjeta)
		{
			lock (locker)
			{
				return (database.Table<Tarjeta>().Where(x => x.idtarjeta == idtarjeta).FirstOrDefault());
			}
		}
		public List<Tarjeta> SelTarjetas()
		{
			lock (locker)
			{
				return (database.Table<Tarjeta>().ToList());
			}
		}
		public void AltaTarjeta(Tarjeta item)
		{
			lock (locker)
			{
				item.titularFN = "";
				item.titularLN = "";
				item.calleynumero = "";
				item.idciudad = 0;
				item.codigopostal = "";
				item.idestado = 0;
				item.permtoken = "";
				item.expirationMMYY = "";
				if ((from i in database.Table<Tarjeta>() where i.idtarjeta == item.idtarjeta select i).Count() > 0)
					database.Update(item);
				else
					database.Insert(item);
			}
		}
		public void DescargaTarjetas(List<Tarjeta> ls)
		{
			lock (locker)
			{
				database.DeleteAll<Tarjeta>();
				database.InsertAll(ls);
			}
		}
		#endregion

		#region Monedero
		public SaldoMonedero SelSaldo()
		{
			lock (locker)
			{
				return (database.Table<SaldoMonedero>().FirstOrDefault());
			}
		}
		public void DescargaSaldo(SaldoMonedero sm)
		{
			lock (locker)
			{
				database.DeleteAll<SaldoMonedero>();
				database.Insert(sm);
			}
		}
		public void IniciaMonedero()
		{
			lock (locker)
			{
				if (database.Table<SaldoMonedero>().Count() == 0)
				{
					SaldoMonedero mon = new SaldoMonedero() { idmonedero = 1, saldo = 0M, caducidad = DateTime.Now.AddYears(2) };
					database.Insert(mon);
				}
			}
		}
		#endregion

		#region Recargas
		public void DelRecargaFrecuente(RecargaProg rf)
		{
			lock (locker)
			{
				database.Delete(rf);
			}
		}
		public List<RecargaProg> SelRecargasProg()
		{
			lock (locker)
			{
				return (database.Table<RecargaProg>().ToList<RecargaProg>());
			}
		}
		public void AltaRecargaProg(RecargaProg item)
		{
			lock (locker)
			{
				//IEnumerable<RecargaProg> ls = database.Table<RecargaProg>().Where(i => i.numerorecarga == item.numerorecarga);
				//if (ls.Count() > 0)
				//{
				//	item.idrecarga = ls.FirstOrDefault().idrecarga;
				//	database.Update(item);
				//}
				//else
					database.Insert(item);
			}
		}
		public void AltaRecargaMonedero(RecargaMonedero item, bool reflejasaldo)
		{
			decimal monto = 0.0M; SaldoMonedero montoact;
			monto = item.monto;
			lock (locker)
			{
				if ((from i in database.Table<RecargaMonedero>() where i.idrecargamonedero == item.idrecargamonedero select i).Count() > 0)
					database.Update(item);
				else
					database.Insert(item);
				if (reflejasaldo)
				{
					//if (monto > 0.0M)
					//{
					//	if (database.Table<SaldoMonedero>().Count() > 0)
					//	{
					//		montoact = database.Table<SaldoMonedero>().FirstOrDefault();
					//		montoact.saldo += monto;
					//		database.Update(montoact);
					//	}
					//	else {
					//		SaldoMonedero sl = new SaldoMonedero() { idmonedero = 1, saldo = monto, caducidad = DateTime.Now.AddYears(2) };
					//		database.Insert(sl);
					//	}
					//}
				}
			}
		}
		public void CargaMonedero(decimal monto)
		{
			SaldoMonedero montoact;
			lock(locker)
			{
				if (monto > 0.0M)
				{
					if (database.Table<SaldoMonedero>().Count() > 0)
					{
						montoact = database.Table<SaldoMonedero>().FirstOrDefault();
						montoact.saldo -= monto;
						database.Update(montoact);
					}
					else
					{
						SaldoMonedero sl = new SaldoMonedero() { idmonedero = 1, saldo = monto, caducidad = DateTime.Now.AddYears(2) };
						database.Insert(sl);
					}
				}
			}
		}
		public void AltaRecarga(Recarga item)
		{
			lock (locker)
			{
				if ((from i in database.Table<Recarga>() where i.idrecarga == item.idrecarga select i).Count() > 0)
					database.Update(item);
				else
					database.Insert(item);
			}
		}
		public void AltaRecargasFrecuentes(RecargaFrecuente item)
		{
			lock (locker)
			{
				if ((from i in database.Table<RecargaFrecuente>() where i.idrecarga == item.idrecarga select i).Count() > 0)
					database.Update(item);
				else
					database.Insert(item);
			}
		}
		public RecargaFrecuente SelRecargaFrecuente(string numero)
		{
			lock (locker)
			{
				return (database.Table<RecargaFrecuente>().Where(x => x.numerorecarga == numero).FirstOrDefault());
			}
		}
		public List<RecargaFrecuente> SelRecargasFrecuentes()
		{
			List<Recarga> lsr;
			lock (locker)
			{
				//return(database.Table<RecargaFrecuente>().OrderBy(x => x.numRecargas).Take(3).ToList<RecargaFrecuente>());
				lsr = database.Table<Recarga>().Where(x => x.err == 0).ToList<Recarga>();
			}
			IEnumerable<string> lsnum = lsr.Select(x => x.numerorecarga).Distinct();
			List<RecargaFrecuente> lsnumcount = new List<RecargaFrecuente>();
			lsnumcount = lsnum.Select(x => new RecargaFrecuente { numerorecarga = x, numRecargas = lsr.Where(z => z.numerorecarga == x).Count() }).ToList();
			List<RecargaFrecuente> lsfrec = lsnumcount.OrderBy(x => x.numRecargas).Take(3).ToList();
			foreach (RecargaFrecuente rf in lsfrec)
			{
				Recarga rtmp = lsr.Where(x => x.numerorecarga == rf.numerorecarga).FirstOrDefault();
				rf.contactorecarga = rtmp.contactorecarga;
				rf.idpais = rtmp.idpais;
				rf.idoperadora = rtmp.idoperadora;
				rf.idpaquete = rtmp.idpaquete;
			}
			return (lsfrec);
		}
		public List<Recarga> SelRecargas()
		{
			lock (locker)
			{
				return (database.Table<Recarga>().OrderBy(x => x.fecha).ToList<Recarga>());
			}
		}
		public List<RecargaMonedero> SelRecargasWallet()
		{
			lock (locker)
			{
				return (database.Table<RecargaMonedero>().OrderBy(x => x.fecha).ToList<RecargaMonedero>());
			}
		}
		public void DelRecargasFrecuentes()
		{
			lock (locker)
			{
				database.DeleteAll<RecargaFrecuente>();
			}
		}
		public void DescargaRecargas(List<Recarga> ls)
		{
			lock (locker)
			{
				database.DeleteAll<Recarga>();
				database.InsertAll(ls);
			}
		}
		public void DescargaRecargasProg(List<RecargaProg> ls)
		{
			lock (locker)
			{
				database.DeleteAll<RecargaProg>();
				database.InsertAll(ls);
			}
		}
		public void DescargaRecargasWallet(List<RecargaMonedero> ls)
		{
			lock (locker)
			{
				database.DeleteAll<RecargaMonedero>();
				database.InsertAll(ls);
			}
		}
		#endregion

		#region usuarios
		public void AltaUsr(Usuario item)
		{
			lock (locker)
			{
				if ((from i in database.Table<Usuario>() where i.idusuario == item.idusuario select i).Count() > 0)
					database.Update(item);
				else
					database.Insert(item);
			}
		}
		public Usuario SelUsr()
		{
			lock (locker)
			{
				return database.Table<Usuario>().FirstOrDefault();
			}
		}
		public Usuario SelUsr(int id)
		{
			lock (locker)
			{
				return database.Table<Usuario>().Where(x => x.idusuario == id).FirstOrDefault();
			}
		}
		public Usuario SelUsr(string email)
		{
			lock (locker)
			{
				return database.Table<Usuario>().Where(x => x.email == email).FirstOrDefault();
			}
		}
		public List<Usuario> SelUsrs()
		{
			lock (locker)
			{
				return database.Table<Usuario>().ToList<Usuario>();
			}
		}
		public void LogoutUsr(Usuario item)
		{
			lock (locker)
			{
				database.DeleteAll<Recarga>();
				database.DeleteAll<RecargaProg>();
				database.DeleteAll<RecargaMonedero>();
				database.DeleteAll<RecargaFrecuente>();
				database.DeleteAll<SaldoMonedero>();
				database.DeleteAll<Usuario>();
				database.DeleteAll<Tarjeta>();
			}
		}
		#endregion
	}

	public class RecFrec{
		public string numerorecarga { get; set; }
		public int numveces { get; set; }
	}
}