using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MasTicket
{
    public class Recarga
    {
        public Recarga()
        {

        }

        [PrimaryKey]
        public int idrecarga { get; set; }
        public int idpais { get; set; }
        public int idoperadora { get; set; }
        public int idpaquete { get; set; }
        public int idformapago { get; set; }
        public int idtarjeta { get; set; }
        public string numerorecarga { get; set; }
        public string contactorecarga { get; set; }
        public DateTime fecha { get; set; }
        public int err { get; set; }
        public int errVs { get; set; }
        public int errRs { get; set; }
        public string TransactionID { get; set; }
        public string PaymentID { get; set; }
        public int idusuario { get; set; }
		public string ip { get; set; }
		public string rsauthorization { get; set; }
		public string rstransactionid { get; set;}
		public string rsrcode { get; set; }
		public string printdata { get; set; }
		public string os { get; set; }
		public string errvestadetallado { get; set; }
    }

    public class RecargaFrecuente : Recarga
    {
        public RecargaFrecuente()
        {

        }
        
        public int numRecargas { get; set; }
    }

    public class RecargaProg : Recarga
    {
        public RecargaProg()
        {

        }

        public string diasmes { get; set; }
    }
}
