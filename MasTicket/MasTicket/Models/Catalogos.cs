using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MasTicket
{
    public class catConfig
    {
        public catConfig()
        {

        }
        [PrimaryKey]
        public int idconfig { get; set; }
        public string config { get; set; }
        public string valor { get; set; }
    }

    public class catPais
    {
        public catPais()
        {

        }

        [PrimaryKey]
        public int idpais { get; set; }
        public string pais { get; set; }
        public string img { get; set; }
		public string codigotel { get; set; }
        public bool paisdefault { get; set; }
        public string codigopais { get; set; }
    }

    public class catEstado
    {
        public catEstado()
        {

        }
        [PrimaryKey]
        public int idestado { get; set; }
        public string estado { get; set; }
        public int idpais { get; set; }
    }

    public class catMunicipio
    {
        public catMunicipio()
        {

        }

        public int idmunicipio { get; set; }
        public string municipio { get; set; }
        public int idestado { get; set; }
    }

    public class catOperadora
    {
        public catOperadora()
        {

        }

        [PrimaryKey]
        public int idoperadora { get; set; }
        public string operadora { get; set; }
        public int idpais { get; set; }
        public string img { get; set; }
    }

    public class catPaquete
    {
        public catPaquete()
        {

        }

        [PrimaryKey]
        public int idpaquete { get; set; }
        public string sku { get; set; }
        public string paquete { get; set; }
        public decimal monto { get; set; }
        public int idoperadora { get; set; }
		public int tipo { get; set; }
    }

    public class catFormasPago
    {
        public catFormasPago()
        {

        }

        [PrimaryKey]
        public int idformapago { get; set; }
        public string formapago { get; set; }
    }

    public class catEmisorTC
    {
        public catEmisorTC()
        {

        }

        [PrimaryKey]
        public int idemisor { get; set; }
        public string emisor { get; set; }
		public string img { get; set; }
    }

	public class catErrores
	{
		public catErrores()
		{
		}

		[PrimaryKey]
		public int iderror { get; set; }
		public string error { get; set; }
		public string emisor { get; set; }
	}

    public class msgreferidos{
        public decimal saldo { get; set; }
        public string msg { get; set; }
    }

}
