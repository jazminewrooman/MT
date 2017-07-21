using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
//using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
//using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;


namespace conAC
{
    class Program
    {
        static private System.Diagnostics.EventLog evt;
        static wsac.IsacClient ws;

        static void Main(string[] args)
        {
            evt = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("AsiCompras"))
            {
                System.Diagnostics.EventLog.CreateEventSource("AsiCompras", "AsiCompras");
            }
            evt.Source = "AsiCompras";
            evt.Log = "AsiCompras";

            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport)
            {
                Name = "basicHttpBinding",
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,

            };
            TimeSpan timeout = new TimeSpan(0, 3, 0);
            binding.SendTimeout = timeout;
            binding.OpenTimeout = timeout;
            binding.ReceiveTimeout = timeout;
            binding.Security = new BasicHttpSecurity
            {
                Mode = BasicHttpSecurityMode.Transport,
                Transport = new HttpTransportSecurity
                {
                    ClientCredentialType = HttpClientCredentialType.Basic,
                    ProxyCredentialType = HttpProxyCredentialType.None,
                    Realm = "",
                },
            };
            EndpointAddress ea = new EndpointAddress("https://asicompras.com/wsac/sac.svc");
            ws = new wsac.IsacClient(binding, ea);
            ws.ClientCredentials.UserName.UserName = "wsac";
            ws.ClientCredentials.UserName.Password = "C0r1t2016";

            evt.WriteEntry("Iniciando recargas programadas");
            var ini = Inicia();
            Task.WaitAll(ini);
        }

        static private async Task Inicia()
        {

            await ws.EncryAsync();

            /*int idrec = 0;
            //wsac.CargaVestaResponse cvr;
            //wsac.AltaRecargaViaWalletResponse vwr;
            try {
                string dia = DateTime.Now.Day.ToString() + ",";
                evt.WriteEntry("Seleccionando recargas del dia " + dia);
                string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
                DataSet ds = new DataSet();
                using (var conn = new MySqlConnection(cs))
                {
                    //string command = "select * from recargaprog where instr(diasmes, '" + dia + "') > 0";
                    //string command = "select* from recargaprog where diasmes like '" + dia + "'";
                    string command = "select r.* from recargaprog r where ExisteDia(r.diasmes, '" + dia + "') = 1";
                    using (var cmd = new MySqlCommand(command, conn))
                    {
                        MySqlDataAdapter adapt = new MySqlDataAdapter(cmd);
                        conn.Open();
                        adapt.Fill(ds, "sql");
                    }
                    foreach (DataRow dr in ds.Tables["sql"].Rows)
                    {
                        idrec = 0;
                        wsac.Recarga r = new wsac.Recarga()
                        {
                            idpais = int.Parse(dr["idpais"].ToString()),
                            idoperadora = int.Parse(dr["idoperadora"].ToString()),
                            idpaquete = int.Parse(dr["idpaquete"].ToString()),
                            idformapago = int.Parse(dr["idformapago"].ToString()),
                            idtarjeta = int.Parse(dr["idtarjeta"].ToString()),
                            numerorecarga = dr["numerorecarga"].ToString(),
                            contactorecarga = (dr["contactorecarga"] != DBNull.Value ? dr["contactorecarga"].ToString() : ""),
                            fecha = DateTime.Now,
                            TransactionID = Guid.NewGuid().ToString(),
                            idusuario = int.Parse(dr["idusuario"].ToString()),
                            ip = "",
                            os = "Programada",
                            esprogramada = true,
                        };

                        if (r.idformapago == 2) //tarjeta
                        {
                            wsac.CargaVesta2aVezResponse cvr2 = new wsac.CargaVesta2aVezResponse();
                            cvr2 = await ws.CargaVesta2aVezAsync(r, null);
                            evt.WriteEntry("Recarga " + cvr2.Body.CargaVesta2aVezResult);
                        }
                        if (r.idformapago == 1) //monedero
                        {
                            wsac.AltaRecargaViaWalletResponse vwr2 = new wsac.AltaRecargaViaWalletResponse();
                            vwr2 = await ws.AltaRecargaViaWalletAsync(r);
                            evt.WriteEntry("Recarga " + vwr2.Body.AltaRecargaViaWalletResult);
                        }
                    }
                    evt.WriteEntry("Terminando recargas programadas");
                }
            }catch(Exception ex)
            {
                evt.WriteEntry(ex.Message + " = " + ex.StackTrace);
            }*/
        }

        static private async Task<int> AltaRecarga(wsac.Recarga r)
        {
            int res = 0;
            string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
            DataSet ds = new DataSet();
            using (var conn = new MySqlConnection(cs))
            {
                string command = "insert into recarga (idpais, idoperadora, idpaquete, idformapago, idtarjeta, numerorecarga, contactorecarga, fecha, TransactionID, idusuario) values (@idpais, @idoperadora, @idpaquete, @idformapago, @idtarjeta, @numerorecarga, @contactorecarga, @fecha, @TransactionID, @idusuario)";
                using (var cmd = new MySqlCommand(command, conn))
                {
                    cmd.Parameters.Add("@idpais", MySqlDbType.Int32).Value = r.idpais;
                    cmd.Parameters.Add("@idoperadora", MySqlDbType.Int32).Value = r.idoperadora;
                    cmd.Parameters.Add("@idpaquete", MySqlDbType.Int32).Value = r.idpaquete;
                    cmd.Parameters.Add("@idformapago", MySqlDbType.Int32).Value = r.idformapago;
                    cmd.Parameters.Add("@idtarjeta", MySqlDbType.Int32).Value = r.idtarjeta;
                    cmd.Parameters.Add("@numerorecarga", MySqlDbType.VarChar, 45).Value = r.numerorecarga;
                    cmd.Parameters.Add("@contactorecarga", MySqlDbType.VarChar, 45).Value = r.contactorecarga;
                    cmd.Parameters.Add("@fecha", MySqlDbType.DateTime).Value = r.fecha;
                    cmd.Parameters.Add("@TransactionID", MySqlDbType.VarChar, 45).Value = r.TransactionID;
                    cmd.Parameters.Add("@idusuario", MySqlDbType.Int32).Value = r.idusuario;
                    conn.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                        res = (int)cmd.LastInsertedId;
                }
            }
            return (res);
        }
    }

    public class tresp
    {
        public string printDatam_data { get; set; }
        public string op_authorization { get; set; }
        public string transaction_id { get; set; }
        public string rcode_description { get; set; }
    }

    public class errRecarga
    {
        public int idrecarga { get; set; }
        public string PaymentID { get; set; }
        public int err { get; set; }
        public int errVs { get; set; }
        public int errRs { get; set; }
        public tresp tresp { get; set; }
    }

}
