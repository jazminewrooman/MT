using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.Net;
using System.Diagnostics;

namespace RSDailyFile
{
    class Program
    {
        private static EventLog evt;

        static void Main(string[] args)
        {
            evt = new EventLog();
            if (!EventLog.SourceExists("AC_RSDialy"))
            {
                EventLog.CreateEventSource("AC_RSDialy", "AC_RSDialy");
            }
            evt.Source = "AC_RSDialy";
            evt.Log = "AC_RSDialy";
            evt.WriteEntry("Iniciando la conexion a FTP Recargasell");

            var ini = Inicia();
            Task.WaitAll(ini);

            /*DateTime dt = new DateTime(2016, 6, 1);
            while (true)
            {
                string fecha = dt.Day.ToString().PadLeft(2, '0') + dt.Month.ToString().PadLeft(2, '0') + dt.Year.ToString();
                Inicia(fecha);
                dt = dt.AddDays(1);
                if (dt.Date == DateTime.Now.Date)
                    break;
            }*/
        }

        //static private void Inicia(string fecha)
        static private async Task Inicia()
        {
            string fecha = DateTime.Now.AddDays(-1).Day.ToString().PadLeft(2, '0') + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Year.ToString();
            //string nombrefile = "000040_" + fecha + ".csv";
            string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
            int numrows = 0;

            using (var conn = new MySqlConnection(cs))
            {
                using (var cmd = new MySqlCommand("select count(procesado) from rsdialydate where fechaprocesado = '" + fecha + "' and procesado = 1", conn))
                {
                    conn.Open();
                    int procesado = int.Parse(cmd.ExecuteScalar().ToString());
                    conn.Close();
                    if (procesado == 0)
                    {
                        try
                        {
                            WebClient request = new WebClient();
                            string url = "ftp://recargaselltesting.ddns.net/" + nombrefile;
                            request.Credentials = new NetworkCredential("asicompras", "4s1K0M@16");
                            byte[] newFileData = request.DownloadData(url);
                            MemoryStream ms = new MemoryStream(newFileData, 0, newFileData.Length, false, true);
                            evt.WriteEntry("Archivo del dia " + fecha + " encontrado");
                            //ms.Seek(0, SeekOrigin.Begin);
                            using (TextFieldParser parser = new TextFieldParser(new MemoryStream(ms.GetBuffer())))
                            {
                                parser.TextFieldType = FieldType.Delimited;
                                parser.SetDelimiters(",");
                                while (!parser.EndOfData)
                                {
                                    string[] fields = parser.ReadFields();
                                    if (numrows == 0)
                                    {
                                        numrows++;
                                        continue;
                                    }
                                    else {
                                        InsertaRow(fields);
                                        numrows++;
                                    }
                                }
                            }
                            if (numrows > 1)
                            {
                                --numrows;
                                evt.WriteEntry("Se proceso " + numrows.ToString() + " rows");
                                using (var cmd2 = new MySqlCommand("insert into rsdialydate (fechaprocesado, procesado, numrows) values ('" + fecha + "', 1, " + numrows.ToString() + ")", conn))
                                {
                                    conn.Open();
                                    cmd2.ExecuteNonQuery();
                                    conn.Close();
                                }
                            }
                            //}
                            //else
                            //    Console.WriteLine("No existe " + nombrefile);
                        }
                        catch (Exception ex)
                        {
                            evt.WriteEntry("Archivo del dia " + fecha + " NO encontrado");
                        }
                        finally
                        {
                            //cli.Disconnect();
                        }
                    }
                }
            }
        }

        static private void InsertaRow(string[] f)
        {
            int res = 0;
            string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
            DataSet ds = new DataSet();
            using (var conn = new MySqlConnection(cs))
            {
                string command = "insert into rsdialy (idtransaccion, fecha, cliente, usuario, celular, monto, folio, resultado, carrier, region) ";
                command += "values (@idtransaccion, STR_TO_DATE('" + f[1] + "', '%d/%m/%Y %H:%i:%s'), @cliente, @usuario, @celular, @monto, @folio, @resultado, @carrier, @region)";
                using (var cmd = new MySqlCommand(command, conn))
                {
                    cmd.Parameters.Add("@idtransaccion", MySqlDbType.Int32).Value = f[0];
                    //cmd.Parameters.Add("@fecha", MySqlDbType.DateTime).Value = f[1];
                    cmd.Parameters.Add("@cliente", MySqlDbType.VarChar, 100).Value = f[2];
                    cmd.Parameters.Add("@usuario", MySqlDbType.VarChar, 100).Value = f[3];
                    cmd.Parameters.Add("@celular", MySqlDbType.VarChar, 10).Value = f[4];
                    cmd.Parameters.Add("@monto", MySqlDbType.Decimal).Value = f[5];
                    cmd.Parameters.Add("@folio", MySqlDbType.VarChar, 30).Value = f[6];
                    cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 100).Value = f[7];
                    cmd.Parameters.Add("@carrier", MySqlDbType.VarChar, 50).Value = f[8];
                    cmd.Parameters.Add("@region", MySqlDbType.VarChar, 10).Value = f[9];
                    conn.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        res = (int)cmd.LastInsertedId;
                    }
                }
            }
        }

    }
}
