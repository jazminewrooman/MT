using System;
using System.Collections.Generic;
using System.Text;

namespace MasTicket
{
    public class Tarjeta
    {
        public Tarjeta()
        {

        }

        public int idtarjeta { get; set; }
        public int idpais { get; set; }
        public int idemisor { get; set; }
        public string permtoken { get; set; }
        public string Last4 { get; set; }
        public string titularFN { get; set; }
        public string titularLN { get; set; }
        public string calleynumero { get; set; }
        public int idciudad { get; set; }
        public string codigopostal { get; set; }
        public int idestado { get; set; }
        public int idusuario { get; set; }
        public string expirationMMYY { get; set; }
    }

	public class RecargaMonedero
	{
		public RecargaMonedero()
		{
		}

		public int idrecargamonedero { get; set; }
		public int idtarjeta { get; set; }
		public decimal monto { get; set; }
        public DateTime fecha { get; set; }
        public int err { get; set; }
        public int errVs { get; set; }
        public string TransactionID { get; set; }
        public string PaymentID { get; set; }
        public int idusuario { get; set; }
        public string ip { get; set; }
        public string os { get; set; }
    }

	public class SaldoMonedero
	{
		public SaldoMonedero()
		{

		}

		public int idmonedero { get; set; }
		public decimal saldo { get; set; }
		public DateTime caducidad { get; set; }
	}
}
