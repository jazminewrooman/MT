using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Xml;
using System.Globalization;
using MasTicket;
using MasTicket.wsMoon;
using System.Text.RegularExpressions;

public enum TipoRecarga
{
    RecargaTA,
    Monedero
}

/*[ServiceBehavior(IncludeExceptionDetailInFaults = true)]*/
public class sac : Isac
{
    string PaymentAPI = ConfigurationManager.AppSettings["PaymentAPI"];
    string TokenizationAPI = ConfigurationManager.AppSettings["TokenizationAPI"];
    string FingerprintAPI = ConfigurationManager.AppSettings["FingerprintAPI"];
    string APIUsername = ConfigurationManager.AppSettings["APIUsername"];
    string APIPassword = ConfigurationManager.AppSettings["APIPassword"];
    string MerchantRoutingID = ConfigurationManager.AppSettings["MerchantRoutingID"];

    #region WebMethods
    private string GeneraRisk(Recarga r, RecargaMonedero rm, Tarjeta t, catPaquete p, catPais ps, catEstado ed, catMunicipio m, Usuario u)
    {
        string xml = "";
        TransactionTypeChannelChannelCode ch = TransactionTypeChannelChannelCode.Web;
        if ((r != null && !r.esprogramada) || (rm != null))
            ch = TransactionTypeChannelChannelCode.Mobile;
        if (r != null && r.esprogramada)
            ch = TransactionTypeChannelChannelCode.Other;
        var data = new RiskInformation
        {
            version = RiskInformationVersion.Item15,
            Transaction = new TransactionType
            {
                IsB2BTransaction = false,
                IsOneTimePayment = true,
                Account = new TransactionTypeAccount
                {
                    AccountID = (r == null ? rm.idusuario.ToString() : r.idusuario.ToString()), //ids de usuario en AC
                    Email = u.email,
                    FirstName = t.titularFN,
                    LastName = t.titularLN,
                    Address = new AddressType[] { new AddressType
                    {
                        AddressLine1 = (t.calleynumero.Length > 30 ? t.calleynumero.Substring(0, 30) : t.calleynumero),
                        City = m.municipio,
                        State = ed.estado,
                        PostalCode = t.codigopostal,
                        CountryCode = ps.codigopais
                    }
                    },
                    MobilePhoneNumber = (ps.codigotel + u.numerocontacto).PadLeft(15, '0'),
                    CreatedDTM = u.fechaalta.ToString("yyyy-MM-ddTHH:mm:ssZ"), //fecha de alta de usuario
                    HasNegativeHistory = false,
                },
                Authentication = new TransactionTypeAuthentication
                {
                    IsAuthenticated = true,
                    AuthenticationMethod = TransactionTypeAuthenticationAuthenticationMethod.PIN,
                    AuthenticationMethodSpecified = true,
                    LoginID = u.email
                },
                Channel = new TransactionTypeChannel
                {
                    ChannelCode = ch,
                    IPAddress = (r == null ? rm.ip : r.ip)
                },
                TimeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            },
            Billing = new BillingType
            {
                Email = u.email,
                ContactPhoneNumber = (ps.codigotel + u.numerocontacto).PadLeft(15, '0'),
                //BankPhoneNumber
                PaymentDeviceCreatedDtm = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                PaymentDeviceCreatedDtmSpecified = true,
                HasNegativeHistory = false,
            },
            ShoppingCartItems = new ShoppingCartItemsType
            {
                count = "1",
                ShoppingCartItem = new ShoppingCartItemsTypeShoppingCartItem[]
                {
                    new ShoppingCartItemsTypeShoppingCartItem
                    {
                        DeliveryMethod = ShoppingCartItemsTypeShoppingCartItemDeliveryMethod.Reload,
                        DeliveryRecipient = new ShoppingCartItemsTypeShoppingCartItemDeliveryRecipient
                        {
                            FirstName = (r == null ? u.name : r.contactorecarga),
                            PhoneNumber = (ps.codigotel + (r == null ? u.numerocontacto : r.numerorecarga)).PadLeft(15, '0'),
                            Address = new AddressType
                            {
                                AddressLine1 = (t.calleynumero.Length > 30 ? t.calleynumero.Substring(0, 30) : t.calleynumero),
                                City = m.municipio,
                                State = ed.estado,
                                PostalCode = t.codigopostal,
                                CountryCode = ps.codigopais
                            },
                            StoredValueAccountID = (ps.codigotel + (r == null ? u.numerocontacto : r.numerorecarga)).PadLeft(15, '0'),
                        },
                        LineItems = new LineItemsType
                        {
                            count = "1",
                            LineItem = new LineItemsTypeLineItem[]
                            {
                                new LineItemsTypeLineItem
                                {
                                    ProductCode = (r == null ? "CORRS001" : p.sku),
                                    ProductDescription = (r == null ? "Recarga de monedero" : p.paquete),
                                    UnitPrice = (r == null ? rm.monto : p.monto),
                                    Quantity = "1"
                                }
                            }
                        },
                    }
                },
            },
        };
        var serializer = new XmlSerializer(typeof(RiskInformation));
        using (StringWriterWithEncoding textWriter = new StringWriterWithEncoding(System.Text.Encoding.UTF8))
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(textWriter, data, ns);
            xml = textWriter.ToString();
        }
        return (xml);
    }

    public int AltaAdmin(MasTicket.administrador a)
    {
        int res = 0;
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();
        using (var conn = new MySqlConnection(cs))
        {
            string command = "insert into administrador (email, numerocontacto, name, firs_name, last_name,  nip, fechaalta) values (@email, @numerocontacto, @name, @firs_name, @last_name, @nip, @fechaalta)";
            using (var cmd = new MySqlCommand(command, conn))
            {
                cmd.Parameters.Add("@email", MySqlDbType.VarChar, 45).Value = a.email;
                cmd.Parameters.Add("@numerocontacto", MySqlDbType.VarChar, 45).Value = a.numerocontacto;
                cmd.Parameters.Add("@name", MySqlDbType.VarChar, 100).Value = a.name;
                cmd.Parameters.Add("@first_name", MySqlDbType.VarChar, 50).Value = a.firs_name;
                cmd.Parameters.Add("@last_name", MySqlDbType.VarChar, 50).Value = a.last_name;
                cmd.Parameters.Add("@nip", MySqlDbType.VarChar, 45).Value = a.nip;
                cmd.Parameters.Add("@fechaalta", MySqlDbType.DateTime).Value = DateTime.Now;
                conn.Open();
                if (cmd.ExecuteNonQuery() > 0)
                {
                    res = (int)cmd.LastInsertedId;
                }
            }
        }
        return (res);
    }

    public int Mod_administrador(MasTicket.administrador u)
    {
        int res = 0;
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        //DataSet ds = new DataSet();
        string command = "update administrador set administrador.nip='" + u.nip + "' where idadministrador='" + u.idadministrador + "' ";
        MySqlConnection conn = new MySqlConnection(cs);
        conn.Open();
        MySqlCommand cmd = new MySqlCommand(command, conn);
        cmd.ExecuteNonQuery();

        if (cmd.ExecuteNonQuery() > 0)
        {
            res = (int)cmd.LastInsertedId;
        }

        conn.Close();
        return (res);
    }

    public bool Delete_administrador(int idadministrador)
    {
        bool res;
        string stm = "delete from administrador where idadministrador = " + idadministrador.ToString();
        if (ExecuteSQL(stm) > 0)
            res = true;
        else
            res = false;
        return (res);
    }

    public int Mod_Usuario(MasTicket.Usuario u)
    {
        int res = 0;
        int nip = 0;
        if (int.TryParse(u.nip, out nip))
        {
            string SaltCryptoKey = ConfigurationManager.AppSettings["SaltCryptoKey"];
            string PwdCryptoKey = ConfigurationManager.AppSettings["PwdCryptoKey"];
            string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
            string command = "update usuario set nip = @nip, bloqueado = @bloqueado, intentos = @intentos where idusuario = @idusuario";
            MySqlConnection conn = new MySqlConnection(cs);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(command, conn);
            cmd.Parameters.Add("@nip", MySqlDbType.VarChar, 50).Value = BitConverter.ToString(Crypto.EncryptAes(u.nip, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
            cmd.Parameters.Add("@idusuario", MySqlDbType.Int32, 50).Value = u.idusuario;
            cmd.Parameters.Add("@bloqueado", MySqlDbType.VarChar, 50).Value = BitConverter.ToString(Crypto.EncryptAes("0", PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
            cmd.Parameters.Add("@intentos", MySqlDbType.VarChar, 50).Value = BitConverter.ToString(Crypto.EncryptAes("0", PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));

            if (cmd.ExecuteNonQuery() > 0)
            {
                res = 1;
            }
            conn.Close();
        }
        return (res);
    }

    public int AltaUsr(MasTicket.Usuario u, int idusrreferido = 0)
    {
        int res = 0;
        Random r = new Random();
        int verif = r.Next(100000, 999999);
        string SaltCryptoKey = ConfigurationManager.AppSettings["SaltCryptoKey"];
        string PwdCryptoKey = ConfigurationManager.AppSettings["PwdCryptoKey"];

        const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

      //  const string nameRegex = @"^([a-zA-Z]?[aeiou]+[a-zA-Z]+[a-zA-Z]\s?)*$";

        int nip = 0;
        int b = 0;
        //&& Regex.IsMatch(u.name, nameRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))
        if (Regex.IsMatch(u.email, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)) && int.TryParse(u.nip, out nip))
        {
            string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
            DataSet ds = new DataSet();
            using (var conn = new MySqlConnection(cs))
            {
              /*  do
                {
                    if (u.verificacion.Equals(""))
                    {
                        u.verificacion = u.nip + verif;
                        b = 1;
                    }
                    else
                    {
                        DataTable dtusr = SelectSQL("select * from usuario where verificacion = '" + u.verificacion + "' and activo=0");
                        if (dtusr.Rows.Count > 0)
                        {
                            u.verificacion = u.verificacion.Substring(0, 4) + verif;
                            b = 0;
                        }
                        else
                        {

                            b = 1;
                        }
                    }
                } while (b!=1);*/


                    string command = "insert into usuario (email, numerocontacto, name, first_name, last_name, gender, picture, idpais, nip, fechaalta, bloqueado, intentos,activo,verificacion) values (@email, @numerocontacto, @name, @first_name, @last_name, @gender, @picture, @idpais, @nip, @fechaalta, @bloqueado, @intentos,@activo, @verificacion)";
                using (var cmd = new MySqlCommand(command, conn))
                {
                    cmd.Parameters.Add("@email", MySqlDbType.VarChar, 200).Value = BitConverter.ToString(Crypto.EncryptAes(u.email, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey))); ;
                    cmd.Parameters.Add("@numerocontacto", MySqlDbType.VarChar, 100).Value = BitConverter.ToString(Crypto.EncryptAes(u.numerocontacto, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar, 200).Value = BitConverter.ToString(Crypto.EncryptAes(u.name, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                    cmd.Parameters.Add("@first_name", MySqlDbType.VarChar, 200).Value = (!String.IsNullOrEmpty(u.first_name) ? BitConverter.ToString(Crypto.EncryptAes(u.first_name, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey))) : "");
                    cmd.Parameters.Add("@last_name", MySqlDbType.VarChar, 200).Value = (!String.IsNullOrEmpty(u.last_name) ? BitConverter.ToString(Crypto.EncryptAes(u.last_name, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey))) : "");
                    cmd.Parameters.Add("@gender", MySqlDbType.VarChar, 45).Value = u.gender;
                    cmd.Parameters.Add("@picture", MySqlDbType.VarChar, 100).Value = u.picture;
                    cmd.Parameters.Add("@idpais", MySqlDbType.Int32).Value = u.idpais;
                    cmd.Parameters.Add("@nip", MySqlDbType.VarChar, 50).Value = BitConverter.ToString(Crypto.EncryptAes(u.nip, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                    cmd.Parameters.Add("@fechaalta", MySqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@bloqueado", MySqlDbType.VarChar, 50).Value = BitConverter.ToString(Crypto.EncryptAes("0", PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                    cmd.Parameters.Add("@intentos", MySqlDbType.VarChar, 50).Value = BitConverter.ToString(Crypto.EncryptAes("0", PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));

                    cmd.Parameters.Add("@activo", MySqlDbType.Int32, 1).Value = 1;
                    cmd.Parameters.Add("@verificacion", MySqlDbType.VarChar, 10).Value = "010";
                    conn.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        res = (int)cmd.LastInsertedId;
                        InsertaReferido(res, idusrreferido);
                        EnviaMail(u.email, 'B', "", u);
                        EnviaMail(GetConfig(10).ToString(), 'L', "", u, null, null,null);
                    }
                }
            }
        }
        return (res);
    }

    private int AltaTarjeta(MasTicket.Tarjeta t)
    {
        string SaltCryptoKey = ConfigurationManager.AppSettings["SaltCryptoKey"];
        string PwdCryptoKey = ConfigurationManager.AppSettings["PwdCryptoKey"];

        int res = -1;
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();
        using (var conn = new MySqlConnection(cs))
        {
            string command = "insert into tarjeta (idusuario, idpais, idemisor, titularFN, titularLN, calleynumero, idciudad, codigopostal, idestado, permtoken, last4, expirationMMYY) values (@idusuario, @idpais, @idemisor, @titularFN, @titularLN, @calleynumero, @idciudad, @codigopostal, @idestado, @permtoken, @last4, @expirationMMYY)";
            using (var cmd = new MySqlCommand(command, conn))
            {
                cmd.Parameters.Add("@idusuario", MySqlDbType.Int32).Value = t.idusuario;
                cmd.Parameters.Add("@idpais", MySqlDbType.Int32).Value = t.idpais;
                cmd.Parameters.Add("@idemisor", MySqlDbType.Int32).Value = t.idemisor;
                cmd.Parameters.Add("@titularFN", MySqlDbType.VarChar, 300).Value = BitConverter.ToString(Crypto.EncryptAes(t.titularFN, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                cmd.Parameters.Add("@titularLN", MySqlDbType.VarChar, 300).Value = BitConverter.ToString(Crypto.EncryptAes(t.titularLN, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                cmd.Parameters.Add("@calleynumero", MySqlDbType.VarChar, 300).Value = BitConverter.ToString(Crypto.EncryptAes(t.calleynumero, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                cmd.Parameters.Add("@idciudad", MySqlDbType.Int32).Value = t.idciudad;
                cmd.Parameters.Add("@codigopostal", MySqlDbType.VarChar, 50).Value = BitConverter.ToString(Crypto.EncryptAes(t.codigopostal, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                cmd.Parameters.Add("@idestado", MySqlDbType.Int32).Value = t.idestado;
                cmd.Parameters.Add("@permtoken", MySqlDbType.VarChar, 100).Value = BitConverter.ToString(Crypto.EncryptAes(t.permtoken, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey))); ;
                cmd.Parameters.Add("@last4", MySqlDbType.VarChar, 4).Value = t.Last4;
                cmd.Parameters.Add("@expirationMMYY", MySqlDbType.VarChar, 50).Value = BitConverter.ToString(Crypto.EncryptAes(t.expirationMMYY, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                conn.Open();
                if (cmd.ExecuteNonQuery() > 0)
                    res = (int)cmd.LastInsertedId;
                else
                    res = -1;
            }
        }
        return (res);
    }

    public bool EliminaTarjeta(int idtarjeta)
    {
        bool res;
        string stm = "delete from tarjeta where idtarjeta = " + idtarjeta.ToString();
        if (ExecuteSQL(stm) > 0)
            res = true;
        else
            res = false;
        return (res);
    }

    public bool EliminaRecProg(int idrecprog)
    {
        bool res;
        string stm = "delete from recargaprog where idrecarga = " + idrecprog.ToString();
        if (ExecuteSQL(stm) > 0)
            res = true;
        else
            res = false;
        return (res);
    }

    public void EnviaMailRecordatorio(int idusuario)
    {
        Usuario u = LlenaUsr(idusuario);
        EnviaMail(u.email, 'N', "", u);
    }

    public void recargaRegaloCat(int idusuario){
        //var verfi=tipo+"_"+monto+"_0";
        string q = "Select verificacion from usuario where idusuario="+idusuario;
        try
        {
            DataTable dtusr = SelectSQL(q);
            if (dtusr.Rows.Count > 0)
            {
                string verificador = dtusr.Rows[0]["verificacion"].ToString();
                try
                {
                    string[] datos = verificador.Split('_');
                    if (datos.Length > 1)
                    {
                        if (datos[2].Equals('0'))
                        {

                            //no ha recargadoy se mando mnsj

                            //realiza abono a monedero del monto
                            RecargaMonedero r = new RecargaMonedero();
                            r.idtarjeta = 0;
                            r.monto = Decimal.Parse(datos[1]);
                            r.fecha = DateTime.Now;
                            r.TransactionID = "00";
                            r.idusuario = idusuario;
                            r.ip = "";
                            r.os = "ProgramadaReg";

                            string res = "";
                            GetSessionTags st = new GetSessionTags();
                            string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
                            DataSet ds = new DataSet();
                            using (var conn = new MySqlConnection(cs))
                            {
                                string command = "insert into recargamonedero (idtarjeta, monto, fecha, TransactionID, idusuario, ip, os) values (@idtarjeta, @monto, @fecha, @TransactionID, @idusuario, @ip, @os)";
                                using (var cmd = new MySqlCommand(command, conn))
                                {
                                    cmd.Parameters.Add("@idtarjeta", MySqlDbType.Int32).Value = r.idtarjeta;
                                    cmd.Parameters.Add("@monto", MySqlDbType.Decimal).Value = r.monto;
                                    cmd.Parameters.Add("@fecha", MySqlDbType.DateTime).Value = r.fecha;
                                    cmd.Parameters.Add("@TransactionID", MySqlDbType.VarChar, 2).Value = r.TransactionID;
                                    cmd.Parameters.Add("@idusuario", MySqlDbType.Int32).Value = r.idusuario;
                                    cmd.Parameters.Add("@ip", MySqlDbType.VarChar, 20).Value = r.ip;
                                    cmd.Parameters.Add("@os", MySqlDbType.VarChar, 100).Value = r.os;
                                    conn.Open();
                                    if (cmd.ExecuteNonQuery() > 0)
                                    {
                                        r.idrecargamonedero = (int)cmd.LastInsertedId;

                                    }
                                    res = JsonConvert.SerializeObject(st);
                                }
                            }
                            //actualiza verificador a 1
                            string ver = datos[0] + "_" + datos[1] + "_1";
                            var stmrs = "update usuario set verificacion = '" + ver + "' where idusuario = " + idusuario;
                            ExecuteSQL(stmrs);
                        }
                    }
                    else
                    {
                        //no tiene categoria aun

                    }
                }
                catch (Exception e) { }

            }
        }
        catch (Exception e) { }
    }

    public async Task<string> AltaRecargaViaWallet(MasTicket.Recarga r)
    {
        //----------------------------------------------------------
        //  0 exito
        //  1 error de RS
        //  2 error no hay suficiente saldo
        //  3 error de sistema (BD)
        //
        //----------------------------------------------------------
        string res = "";
        int err = -1;
        int errVs = 0;
        int errRs = -1;
        string errorvestadetallado = "";
        tresp tresp = new tresp();
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();
        using (var conn = new MySqlConnection(cs))
        {
            DataTable dtusr = SelectSQL("select * from usuario where idusuario = '" + r.idusuario + "' and activo=1");
            if (dtusr.Rows.Count > 0)
            {
                string command = "insert into recarga (idpais, idoperadora, idpaquete, idformapago, idtarjeta, numerorecarga, contactorecarga, fecha, TransactionID, idusuario, ip, os) values (@idpais, @idoperadora, @idpaquete, @idformapago, @idtarjeta, @numerorecarga, @contactorecarga, @fecha, @TransactionID, @idusuario, @ip, @os)";
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
                    cmd.Parameters.Add("@ip", MySqlDbType.VarChar, 20).Value = r.ip;
                    cmd.Parameters.Add("@os", MySqlDbType.VarChar, 100).Value = r.os;
                    conn.Open();

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        r.idrecarga = (int)cmd.LastInsertedId;
                        catPaquete p = LlenaPaquete(r.idpaquete);
                        Usuario u = LlenaUsr(r.idusuario);


						RecargaMonederoXReferido(u.idusuario);


						using (var cmd2 = new MySqlCommand("CargaMonedero", conn))
                        {
                            cmd2.CommandType = CommandType.StoredProcedure;
                            cmd2.Parameters.Add("@idusr", MySqlDbType.Int32).Value = r.idusuario;
                            cmd2.Parameters.Add("@monto", MySqlDbType.Decimal).Value = p.monto;
                            if (int.Parse(cmd2.ExecuteScalar().ToString()) == 0)
                            {
                                //-------------------------------------------------------
                                //------    conectar a ws RecargaSell   -----------------
                                //-------------------------------------------------------
                                errRecarga tmp = await RecargaRS(r, p);
                                errVs = 0;
                                err = tmp.err;
                                errRs = tmp.errRs;
                                tresp = tmp.tresp;
                                string stmrs = "";
                                if (errRs == 0)
                                {
                                    if (String.IsNullOrEmpty(tmp.tresp.op_authorization) && String.IsNullOrEmpty(tmp.tresp.transaction_id))
                                        errorvestadetallado = GeneraDevolucionViaWallet(r, u, p);
                                    stmrs = "update recarga set errvestadetallado = '" + errorvestadetallado + "', rsauthorization = '" + tmp.tresp.op_authorization + "', rstransactionid = '" + tmp.tresp.transaction_id + "', rsrcode = '" + tmp.tresp.rcode_description + "' where idrecarga = " + r.idrecarga.ToString();
                                    ExecuteSQL(stmrs);
                                    recargaRegaloCat(r.idusuario);
                                }
                                else
                                {
                                    //if (String.IsNullOrEmpty(tmp.tresp.op_authorization) && String.IsNullOrEmpty(tmp.tresp.transaction_id))
                                    errorvestadetallado = GeneraDevolucionViaWallet(r, u, p);
                                    stmrs = "update recarga set errvestadetallado = '" + errorvestadetallado + "', rsrcode = '" + tmp.tresp.rcode_description + "' where idrecarga = " + r.idrecarga.ToString();
                                    ExecuteSQL(stmrs);
                                    err = 1;
                                }
                            }
                            else
                                err = 2;
                        }
                    }
                    else
                    {
                        res = "";
                    }
                }
            }
            else {
                err = 4;
            }
            
        }
        string stm = "";
        stm = "update recarga set err = " + err.ToString() + ", errVs = " + errVs.ToString() + ", errRs = " + errRs.ToString() + " where idrecarga = " + r.idrecarga.ToString();
        ExecuteSQL(stm);

        errRecarga e = new errRecarga { idrecarga = r.idrecarga, err = err, errVs = errVs, errRs = errRs, tresp = tresp };
        return (JsonConvert.SerializeObject(e));
    }

    private int AltaPara2aVez(MasTicket.Recarga r, MasTicket.RecargaMonedero rm)
    {
        int res = -1;
        try
        {
            string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
            DataSet ds = new DataSet();
            using (var conn = new MySqlConnection(cs))
            {
                if (r != null && rm == null)
                {
                   
                    string command = "insert into recarga (idpais, idoperadora, idpaquete, idformapago, idtarjeta, numerorecarga, contactorecarga, fecha, TransactionID, idusuario, ip, os) values (@idpais, @idoperadora, @idpaquete, @idformapago, @idtarjeta, @numerorecarga, @contactorecarga, @fecha, @TransactionID, @idusuario, @ip, @os)";
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
                        cmd.Parameters.Add("@ip", MySqlDbType.VarChar, 20).Value = r.ip;
                        cmd.Parameters.Add("@os", MySqlDbType.VarChar, 100).Value = r.os;
                        conn.Open();
                        if (cmd.ExecuteNonQuery() > 0)
                            res = (int)cmd.LastInsertedId;
                    }
                }
                if (rm != null && r == null)
                {
                    string command = "insert into recargamonedero (idtarjeta, monto, fecha, TransactionID, idusuario, ip, os) values (@idtarjeta, @monto, @fecha, @TransactionID, @idusuario, @ip, @os)";
                    using (var cmd = new MySqlCommand(command, conn))
                    {
                        cmd.Parameters.Add("@idtarjeta", MySqlDbType.Int32).Value = rm.idtarjeta;
                        cmd.Parameters.Add("@monto", MySqlDbType.Decimal).Value = rm.monto;
                        cmd.Parameters.Add("@fecha", MySqlDbType.DateTime).Value = rm.fecha;
                        cmd.Parameters.Add("@TransactionID", MySqlDbType.VarChar, 45).Value = rm.TransactionID;
                        cmd.Parameters.Add("@idusuario", MySqlDbType.Int32).Value = rm.idusuario;
                        cmd.Parameters.Add("@ip", MySqlDbType.VarChar, 20).Value = rm.ip;
                        cmd.Parameters.Add("@os", MySqlDbType.VarChar, 100).Value = rm.os;
                        conn.Open();
                        if (cmd.ExecuteNonQuery() > 0)
                            res = (int)cmd.LastInsertedId;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //evt.WriteEntry(ex.Message + " " + ex.StackTrace);
        }
        return (res);
    }

    public async Task<string> AltaRecarga(MasTicket.Recarga r)
    {
        string res = "";
        GetSessionTags st = new GetSessionTags();
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();
        using (var conn = new MySqlConnection(cs))
        {
            //string command = "insert into recarga (idpais, idoperadora, idpaquete, idformapago, idtarjeta, numerorecarga, contactorecarga, fecha, TransactionID, idusuario, ip, os, err, errVs, errvestadetallado, PaymentID) values (@idpais, @idoperadora, @idpaquete, @idformapago, @idtarjeta, @numerorecarga, @contactorecarga, @fecha, @TransactionID, @idusuario, @ip, @os, @err, @errVs, @errvestadetallado, @PaymentID)";
            string command = "insert into recarga (idpais, idoperadora, idpaquete, idformapago, idtarjeta, numerorecarga, contactorecarga, fecha, TransactionID, idusuario, ip, os) values (@idpais, @idoperadora, @idpaquete, @idformapago, @idtarjeta, @numerorecarga, @contactorecarga, @fecha, @TransactionID, @idusuario, @ip, @os)";
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
                cmd.Parameters.Add("@ip", MySqlDbType.VarChar, 20).Value = r.ip;
                cmd.Parameters.Add("@os", MySqlDbType.VarChar, 100).Value = r.os;
                //cmd.Parameters.Add("@err", MySqlDbType.Int32).Value = r.err;
                //cmd.Parameters.Add("@errVs", MySqlDbType.Int32).Value = r.errVs;
                //cmd.Parameters.Add("@errvestadetallado", MySqlDbType.VarChar, 200).Value = r.errvestadetallado;
                //cmd.Parameters.Add("@PaymentID", MySqlDbType.VarChar, 45).Value = r.PaymentID;
                conn.Open();
                if (cmd.ExecuteNonQuery() > 0)
                {
                    r.idrecarga = (int)cmd.LastInsertedId;
                    st = await LeeTags(r, null);
                }
                res = JsonConvert.SerializeObject(st);
            }
        }
        return (res);
    }

    public async Task<int> AltaRecargaProg(MasTicket.RecargaProg r)
    {
        int res = -1;
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();
        using (var conn = new MySqlConnection(cs))
        {
            string command = "insert into recargaprog (idpais, idoperadora, idpaquete, idformapago, idtarjeta, numerorecarga, contactorecarga, idusuario, diasmes) values (@idpais, @idoperadora, @idpaquete, @idformapago, @idtarjeta, @numerorecarga, @contactorecarga, @idusuario, @diasmes)";
            using (var cmd = new MySqlCommand(command, conn))
            {
                cmd.Parameters.Add("@idpais", MySqlDbType.Int32).Value = r.idpais;
                cmd.Parameters.Add("@idoperadora", MySqlDbType.Int32).Value = r.idoperadora;
                cmd.Parameters.Add("@idpaquete", MySqlDbType.Int32).Value = r.idpaquete;
                cmd.Parameters.Add("@idformapago", MySqlDbType.Int32).Value = r.idformapago;
                cmd.Parameters.Add("@idtarjeta", MySqlDbType.Int32).Value = r.idtarjeta;
                cmd.Parameters.Add("@numerorecarga", MySqlDbType.VarChar, 45).Value = r.numerorecarga;
                cmd.Parameters.Add("@contactorecarga", MySqlDbType.VarChar, 45).Value = r.contactorecarga;
                cmd.Parameters.Add("@idusuario", MySqlDbType.Int32).Value = r.idusuario;
                cmd.Parameters.Add("@diasmes", MySqlDbType.VarChar, 100).Value = r.diasmes;
                conn.Open();
                if (cmd.ExecuteNonQuery() > 0)
                    res = (int)cmd.LastInsertedId;
            }
        }
        return (res);
    }

    public async Task<string> AltaRecargaMonedero(MasTicket.RecargaMonedero r)
    {
        string res = "";
        GetSessionTags st = new GetSessionTags();
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();
        using (var conn = new MySqlConnection(cs))
        {
            string command = "insert into recargamonedero (idtarjeta, monto, fecha, TransactionID, idusuario, ip, os) values (@idtarjeta, @monto, @fecha, @TransactionID, @idusuario, @ip, @os)";
            using (var cmd = new MySqlCommand(command, conn))
            {
                cmd.Parameters.Add("@idtarjeta", MySqlDbType.Int32).Value = r.idtarjeta;
                cmd.Parameters.Add("@monto", MySqlDbType.Decimal).Value = r.monto;
                cmd.Parameters.Add("@fecha", MySqlDbType.DateTime).Value = r.fecha;
                cmd.Parameters.Add("@TransactionID", MySqlDbType.VarChar, 45).Value = r.TransactionID;
                cmd.Parameters.Add("@idusuario", MySqlDbType.Int32).Value = r.idusuario;
                cmd.Parameters.Add("@ip", MySqlDbType.VarChar, 20).Value = r.ip;
                cmd.Parameters.Add("@os", MySqlDbType.VarChar, 100).Value = r.os;
                conn.Open();
                if (cmd.ExecuteNonQuery() > 0)
                {
                    r.idrecargamonedero = (int)cmd.LastInsertedId;
                    st = await LeeTags(null, r);
                }
                res = JsonConvert.SerializeObject(st);
            }
        }
        return (res);
    }

	private static Random random = new Random();
	public string GetCodigoReferidoUsr(int idusr)
	{
		string stm = "";
		string codigo = "";
		stm = "select codigo from codigosreferidos where idusuario = " + idusr.ToString();
		DataTable dt = SelectSQL(stm);
        
		if (dt.Rows.Count == 0 || String.IsNullOrEmpty(dt.Rows[0]["codigo"].ToString()))
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			codigo = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
			ExecuteSQL("insert into codigosreferidos (idusuario, codigo, fecha, activo) values(" + idusr.ToString() + ", '" + codigo + "', now(), 1)");
		}
		else
			codigo = dt.Rows[0]["codigo"].ToString();
		return (codigo);
	}

    public int ExisteCodigoReferidoUsr(string codigo){
        int res = 0;
        string stm = "select idusuario from codigosreferidos where codigo = '" + codigo + "'";
		DataTable dt = SelectSQL(stm);
        if (dt.Rows.Count == 0 || String.IsNullOrEmpty(dt.Rows[0]["idusuario"].ToString()))
            res = 0;
        else
            res = int.Parse(dt.Rows[0]["idusuario"].ToString());
        return (res);
	}

    private void InsertaReferido(int idusrnuevo, int idusrreferido){
        if (idusrreferido > 0)
        {
            string stm = "insert into referidos (idusuario, idreferido, usado, fecha) values (" + idusrnuevo + ", " + idusrreferido + ", 0, now())";
            ExecuteSQL(stm);
        }
	}

    private void RecargaMonederoXReferido(int idusr){
        int err = 0;
		string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
		DataSet ds = new DataSet();
		using (var conn = new MySqlConnection(cs))
		{
			string command = "GuardaMonederoXReferido";
			using (var cmd = new MySqlCommand(command, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@idusr", MySqlDbType.Int32).Value = idusr;
				conn.Open();
				try
				{
                    int idref = int.Parse(cmd.ExecuteScalar().ToString());
                    if (idref > 0){
						wsMoon ws = new wsMoon();
						Usuario u = LlenaUsr(idusr);
                        EnviaMail(u.email, 'F', "", u, msg: "Por usar el codigo promocional de uno de tus amigos, se te ha abonado credito, disponible en tu monedero!");
						ws.getSN("dragonballZ@", "wsM00n", "ws4d1mnt43@", u.numerocontacto, "Por usar el codigo promocional de uno de tus amigos, se te ha abonado credito, disponible en tu monedero!");
                        Task.Delay(TimeSpan.FromMilliseconds(500));
                        u = LlenaUsr(idref);
						EnviaMail(u.email, 'F', "", u, msg: "Uno de tus amigos recargo exitosamente, con lo cual te has ganado crédito, disponible en tu monedero!");
						ws.getSN("dragonballZ@", "wsM00n", "ws4d1mnt43@", u.numerocontacto, "Uno de tus amigos recargo exitosamente, con lo cual te has ganado crédito, disponible en tu monedero!");

						err = 0;

                    }
					else
						err = 1;
				}
				catch (Exception exct)
				{
				}
			}
		}
    }

    public string GetMsgReferidosPUsr(int idusr){
        string json = "";
        string stm = "select distinct s.saldo, 'Por usar el codigo promocional de uno de tus amigos, se te ha abonado credito, disponible en tu monedero!' as msg from referidos r left join saldomonedero s on r.idusuario = s.idusuario where r.idusuario = " + idusr.ToString() + " and mostradousr = 0 and usado = 1";
        DataTable dt = SelectSQL(stm);
        json = JsonConvert.SerializeObject(dt);
        if (dt.Rows.Count > 0)
            ExecuteSQL("update referidos set mostradousr = 1 where idusuario = " + idusr.ToString() + " and mostradousr = 0 and usado = 1");

        return (json);
    }

	public string GetMsgReferidosPRef(int idref)
	{
        string json = "";
        string stm = "select distinct s.saldo, 'Uno de tus amigos recargo exitosamente, con lo cual te has ganado crédito, disponible en tu monedero!' as msg from referidos r left join saldomonedero s on r.idreferido = s.idusuario where idreferido = " + idref.ToString() + " and mostradoref = 0 and usado = 1";
		DataTable dt = SelectSQL(stm);
		json = JsonConvert.SerializeObject(dt);
        if (dt.Rows.Count > 0)
            ExecuteSQL("update referidos set mostradoref = 1 where idreferido = " + idref.ToString() + " and mostradoref = 0 and usado = 1");

		return (json);
	}
	
    private async Task<GetSessionTags> LeeTags(Recarga r, RecargaMonedero rm)
    {
        GetSessionTags tags = null;
        try {
            using (var client = new HttpClient())
            {
                var values1 = new Dictionary<string, string>
                {
                    { "AccountName", APIUsername },
                    { "Password", APIPassword },
                    { "TransactionID", (r == null ? rm.TransactionID : r.TransactionID) } //UUID?
                };
                string json = JsonConvert.SerializeObject(values1);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsync(PaymentAPI + "/GetSessionTags", new StringContent(json, Encoding.UTF8, "application/json"));
                var responseString = await response.Content.ReadAsStringAsync();
                tags = JsonConvert.DeserializeObject<GetSessionTags>(responseString);
                tags.idrecarga = (r == null ? rm.idrecargamonedero : r.idrecarga);
            }
        }
        catch (Exception ex)
        {

        }
        return (tags);
    }
    /*
    public async Task<string> CargaVesta1aVez(Recarga r, RecargaMonedero rm, Tarjeta t)
    {
        errRecarga er = new errRecarga();
        catPaquete p = LlenaPaquete(r.idpaquete);
        Usuario u = LlenaUsr(r == null && rm != null ? rm.idusuario : r.idusuario);

        if (r != null && rm == null)
        {
            t.idtarjeta = AltaTarjeta(t);
            if (t.idtarjeta > 0)
            {
                r.idtarjeta = t.idtarjeta;
                r.idrecarga = AltaRecarga(r);
                if (r.idrecarga > 0)
                {
                    er = await RecargaRS(r, p);
                    string stmrs = "";
                    if (er.errRs == 0)
                    {
                        stmrs = "update recarga set rsauthorization = '" + er.tresp.op_authorization + "', rstransactionid = '" + er.tresp.transaction_id + "', rsrcode = '" + er.tresp.rcode_description + "' where idrecarga = " + r.idrecarga.ToString();
                        ExecuteSQL(stmrs);
                    }
                    else
                    {
                        stmrs = "update recarga set rsrcode = '" + er.tresp.rcode_description + "' where idrecarga = " + r.idrecarga.ToString();
                        ExecuteSQL(stmrs);
                    }
                }
            }
        }

        if (rm != null && r == null)
        {
            t.idtarjeta = AltaTarjeta(t);
            if (t.idtarjeta > 0)
            {
                rm.idtarjeta = t.idtarjeta;
                rm.idrecargamonedero = AltaRecargaMonedero(rm);
                if (rm.idrecargamonedero > 0)
                {
                    string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
                    DataSet ds = new DataSet();
                    using (var conn = new MySqlConnection(cs))
                    {
                        string command = "GuardaMonedero";
                        using (var cmd = new MySqlCommand(command, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@idusr", MySqlDbType.Int32).Value = u.idusuario;
                            cmd.Parameters.Add("@monto", MySqlDbType.Decimal).Value = rm.monto;
                            conn.Open();
                            if (cmd.ExecuteNonQuery() > 0)
                                er.err = 0;
                            else
                                er.err = 1;
                        }
                    }
                }
            }
        }
        return (JsonConvert.SerializeObject(er));
    }
    */
    public async Task<string> CargaVesta2aVez(Recarga r, RecargaMonedero rm, string WebSessionID)
    {
        int err = -1; int errVs = -1; int errRs = -1; string paymentid = "";
        int idnuevatarjeta = 0; int idnuevarecarga = 0;
        errRecarga e = new errRecarga();
        string riskxml = ""; string json = ""; tresp tresp = new tresp();
        string errorvestadetallado = "";
        string responseString2 = "";

        /*System.Diagnostics.EventLog evt = new System.Diagnostics.EventLog();
        if (!System.Diagnostics.EventLog.SourceExists("AsiCompras"))
        {
            System.Diagnostics.EventLog.CreateEventSource("AsiCompras", "AsiCompras");
        }
        evt.Source = "AsiCompras";
        evt.Log = "AsiCompras";*/

        if (r == null && rm != null)
        {
            e = new errRecarga { idtarjeta = idnuevatarjeta, PaymentID = paymentid, err = 3, errVs = 11, errRs = errRs, tresp = new tresp { rcode_description = "Por el momento el monedero esta deshabilitado para hacer recargas" } };
            return (JsonConvert.SerializeObject(e));
        }

        try
        {
            catPaquete p = null;
            Usuario u = LlenaUsr((r == null && rm != null ? rm.idusuario : r.idusuario));
            catPais ps = LlenaPais(u.idpais);
            Tarjeta t = LlenaTarjeta((r == null && rm != null ? rm.idtarjeta : r.idtarjeta));
            catEstado ed = LlenaEstado(t.idestado);
            catMunicipio m = LlenaMunic(t.idestado, t.idciudad);
            if (rm == null && r != null)
                p = LlenaPaquete(r.idpaquete);

            //idnuevarecarga = AltaPara2aVez(r, rm);
            //if (idnuevarecarga > 0)
            //{
            //    if (r != null && rm == null)
            //        r.idrecarga = idnuevarecarga;
            //    if (r == null && rm != null)
            //        rm.idrecargamonedero = idnuevarecarga;
            using (var client = new HttpClient())
            {
                riskxml = GeneraRisk(r, rm, t, p, ps, ed, m, u);
                var values3 = new Dictionary<string, string>
                    {
                    { "AccountName", APIUsername },
                    { "CardHolderAddressLine1", (t.calleynumero.Length > 30 ? t.calleynumero.Substring(0, 30) : t.calleynumero) },
                    { "CardHolderCity", (m.municipio.Length > 30 ? m.municipio.Substring(0, 30) : m.municipio) },
                    { "CardHolderCountryCode", ps.codigopais },
                    { "CardHolderFirstName", t.titularFN },
                    { "CardHolderLastName", t.titularLN },
                    { "CardHolderPostalCode", t.codigopostal },
                    { "CardHolderRegion", (ed.estado.Length > 30 ? ed.estado.Substring(0,30) : ed.estado) },
                    { "ChargeAccountNumber", t.permtoken },
                    { "ChargeAccountNumberIndicator", "3"}, //3 token permanente 2 token temporal 1 tarjeta
                    { "ChargeAmount", (rm == null && r != null ? p.monto.ToString() : rm.monto.ToString()) },
                    { "ChargeExpirationMMYY", t.expirationMMYY },
                    { "ChargeSource", "PPD" },
                    { "MerchantRoutingID", MerchantRoutingID },
                    { "Password", APIPassword },
                    { "TransactionID", (r == null ? rm.TransactionID : r.TransactionID) },
                    { "RiskInformation", riskxml },
                    { "WebSessionID", WebSessionID },
                    //{ "ChargeCVN", cvv },
                    { "StoreCard", "false" },
                };
                json = JsonConvert.SerializeObject(values3);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response2 = await client.PostAsync(PaymentAPI + "/ChargeSale", new StringContent(json, Encoding.UTF8, "application/json"));
                responseString2 = await response2.Content.ReadAsStringAsync();

                string stmresp = "";
                if (rm == null && r != null)
                    stmresp = "update recarga set riskxml = '" + responseString2 + "' where idrecarga = " + r.idrecarga.ToString();
                else
                    stmresp = "update recargamonedero set riskxml = '" + responseString2 + "' where idrecargamonedero = " + rm.idrecargamonedero.ToString();
                ExecuteSQL(stmresp);

                ChargeSale sale = JsonConvert.DeserializeObject<ChargeSale>(responseString2);
                paymentid = sale.PaymentID;
                if (sale.ResponseCode == 0 && sale.PaymentStatus == 10)
                {
					RecargaMonederoXReferido(u.idusuario);

					errVs = 0;
                    if (rm == null && r != null)
                    {
                        //-------------------------------------------------------
                        //------    conectar a ws RecargaSell   -----------------
                        //-------------------------------------------------------
                        errRecarga tmp = await RecargaRS(r, p);
                        //if (tmp.tresp.printData.Length > 0)
                        //    printdata = tmp.tresp.printData[0].m_data;
                        err = tmp.err;
                        errRs = tmp.errRs;
                        tresp = tmp.tresp;
                        string stmrs = "";
                        if (errRs == 0)
                        {
                            if (String.IsNullOrEmpty(tmp.tresp.op_authorization) && String.IsNullOrEmpty(tmp.tresp.transaction_id))
                            {
                                if (r != null && rm == null)
                                {
                                    //  18Mar2017
                                    //if (r.idformapago == 2)
                                    //    errorvestadetallado = await GeneraDevolucion(r, t, u, paymentid, p);
                                    if (r.idformapago == 2)
                                    {
                                        errorvestadetallado = GeneraDevolucionViaWallet(r, u, p);
                                        //tresp.rcode_description += Environment.NewLine + "Tu recarga no pudo ser procesada, verifica el número y el operador. Tu saldo se abonó al monedero. Intenta la recarga más tarde y paga seleccionado el saldo del monedero.";
                                        tresp.rcode_description = "Estimado " + u.name + ", hemos abonado el saldo de la última compra a tu monedero, debido a que no logró ser procesada";
                                        EnviaMail(GetConfig(9).ToString(), 'E', "", u, null, r, p);
                                    }
                                    if (r.idformapago == 1)
                                        errorvestadetallado = GeneraDevolucionViaWallet(r, u, p);
                                }
                            }
                            stmrs = "update recarga set rsauthorization = '" + tmp.tresp.op_authorization + "', rstransactionid = '" + tmp.tresp.transaction_id + "', rsrcode = '" + tmp.tresp.rcode_description + "' where idrecarga = " + r.idrecarga.ToString();
                            ExecuteSQL(stmrs);
                            recargaRegaloCat(r.idusuario);
                        }
                        else
                        {
                            //if (String.IsNullOrEmpty(tmp.tresp.op_authorization) && String.IsNullOrEmpty(tmp.tresp.transaction_id))
                            //{
                            if (r != null && rm == null)
                            {
                                //  18Mar2017
                                //if (r.idformapago == 2)
                                //    errorvestadetallado = await GeneraDevolucion(r, t, u, paymentid, p);
                                if (r.idformapago == 2)
                                {
                                    errorvestadetallado = GeneraDevolucionViaWallet(r, u, p);
                                    //tresp.rcode_description += Environment.NewLine + "Tu recarga no pudo ser procesada, verifica el número y el operador. Tu saldo se abonó al monedero. Intenta la recarga más tarde y paga seleccionado el saldo del monedero.";
                                    tresp.rcode_description = "Estimado " + u.name + ", hemos abonado el saldo de la última compra a tu monedero, debido a que no logró ser procesada";
                                    EnviaMail(GetConfig(9).ToString(), 'E', "", u, null, r, p);
                                }
                                if (r.idformapago == 1)
                                    errorvestadetallado = GeneraDevolucionViaWallet(r, u, p);
                            }
                            //}
                            stmrs = "update recarga set rsrcode = '" + tmp.tresp.rcode_description + "' where idrecarga = " + r.idrecarga.ToString();
                            ExecuteSQL(stmrs);
                        }
                    }
                    else
                    {
                        //--------------------------------------------------
                        //----------    monedero    ------------------------
                        //--------------------------------------------------
                        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
                        DataSet ds = new DataSet();
                        using (var conn = new MySqlConnection(cs))
                        {
                            string command = "GuardaMonedero";
                            using (var cmd = new MySqlCommand(command, conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@idusr", MySqlDbType.Int32).Value = u.idusuario;
                                cmd.Parameters.Add("@monto", MySqlDbType.Decimal).Value = rm.monto;
                                conn.Open();
                                try
                                {
                                    if (cmd.ExecuteNonQuery() > 0)
                                        err = 0;
                                    else
                                        err = 1;
                                }
                                catch (Exception exct)
                                {
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (sale.AuthResultCode == 1)
                        errVs = 3;
                    if (sale.AVSResultCode != 4096 && sale.AVSResultCode != 0)
                        errVs = 4;
                    if (sale.CVNResultCode != 17 && sale.CVNResultCode != 0)
                        errVs = 5;
                    if (sale.PaymentStatus != 6 && sale.PaymentStatus != 10)
                        errVs = 6;
                    if (sale.PaymentStatus == 6)
                        errVs = 1;
                    List<int> salerrs5 = new List<int> { 510, 511, 512, 513, 514 };
                    if (sale.ResponseCode == 1 || salerrs5.Contains(sale.ResponseCode) || sale.ResponseCode == 1005)
                        errVs = 1;
                    if (sale.ResponseCode == 521 || sale.ResponseCode == 1002 || sale.ResponseCode == 1003)
                        errVs = 2;
                    if (sale.ResponseCode == 1014)
                        errVs = 7;
                    if (sale.ResponseCode == 1016)
                        errVs = 4;
                    err = 2;
                    errorvestadetallado = "Charge= AuthResultCode: " + sale.AuthResultCode + " AVSResultCode: " + sale.AVSResultCode + " CVNResultCode: " + sale.CVNResultCode + " PaymentStatus: " + sale.PaymentStatus + " ResponseCode: " + sale.ResponseCode;

                    int numhoy = int.Parse(SelectCountSQL("SET time_zone = '-6:00'; select count(distinct r.idrecarga), date_format(u.fechaalta, '%Y%m%d'), date_format(now(), '%Y%m%d') from recarga r left join usuario u on r.idusuario = u.idusuario where r.idusuario = 201 and r.idformapago = 2 and r.errVs = 0 and date_format(u.fechaalta, '%Y%m%d') = date_format(now(), '%Y%m%d')").ToString());
                    if (numhoy == 3)
                        tresp.rcode_description = "En su primer dia solo se permite hacer 3 recargas. Si desea hacer mas, intente mañana.";
                }
            }
            //}
        }
        catch (Exception ex)
        {
            //evt.WriteEntry(ex.Message + " " + ex.StackTrace);
        }
        string stm = "";
        if (rm == null && r != null)
            stm = "update recarga set chargesale = '" + json + "', riskxml = '" + responseString2 + "', errvestadetallado = '" + errorvestadetallado + "', err = " + err.ToString() + ", errVs = " + errVs.ToString() + ", errRs = " + errRs.ToString() + ", PaymentID = '" + paymentid + "' where idrecarga = " + r.idrecarga.ToString();
        else
            stm = "update recargamonedero set chargesale = '" + json + "', riskxml = '" + responseString2 + "', errvestadetallado = '" + errorvestadetallado + "', err = " + err.ToString() + ", errVs = " + errVs.ToString() + ", PaymentID = '" + paymentid + "' where idrecargamonedero = " + rm.idrecargamonedero.ToString();
        ExecuteSQL(stm);

        e = new errRecarga { idrecarga = idnuevarecarga, idtarjeta = idnuevatarjeta, PaymentID = paymentid, err = err, errVs = errVs, errRs = errRs, tresp = tresp };
        return (JsonConvert.SerializeObject(e));
    }

    /*private async Task<string> GeneraDevolucionMON(RecargaMonedero rm, Tarjeta t, Usuario u, string paymentid, decimal monto)
    {
        string errorvestadetallado = "";
        using (var client = new HttpClient())
        {
            ReversePayment rev = new ReversePayment();
            string json = "";
            //if (r != null && r.idformapago == 2) //tarjeta
            //{
            var values2 = new Dictionary<string, string>
                    {
                        { "AccountName", APIUsername },
                    //{ "ChargeAccountNumber", t.numero },
                    //{ "ChargeAccountNumberIndicator", "1" },
                    //-----------------------------------------------------------------------
                    //{ "ChargeExpirationMMYY", t.expiramm.ToString() + t.expirayy.ToString() },
                    //-----------------------------------------------------------------------
                        { "MerchantRoutingID", MerchantRoutingID },
                        { "PartnerCustomerKey", u.idusuario.ToString() },
                        { "Password", APIPassword },
                        { "PaymentID", paymentid },
                        { "RefundAmount", monto.ToString() },
                        //{ "ReportingInformation", "" },
                        { "TransactionID", rm.TransactionID },
                    };
            json = JsonConvert.SerializeObject(values2);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.PostAsync(PaymentAPI + "/ReversePayment", new StringContent(json, Encoding.UTF8, "application/json"));
            var responseString = await response.Content.ReadAsStringAsync();
            rev = JsonConvert.DeserializeObject<ReversePayment>(responseString);
            errorvestadetallado = "ReversePayment= PaymentStatus: " + rev.PaymentStatus.ToString() + " ResponseCode: " + rev.ResponseCode.ToString() + " ResponseText: " + rev.ResponseText + " ReversalAction:" + rev.ReversalAction.ToString();
            //}
        }
        return (errorvestadetallado);
    }

    public async Task DEV()
    {
        string errorvestadetallado = "";
        try
        {
            DataTable dt = SelectSQL("select * from recargamonedero where idrecargamonedero in (140,144,147)");
            foreach (DataRow dr in dt.Rows)
            {
                RecargaMonedero rm = new RecargaMonedero()
                {
                    idrecargamonedero = int.Parse(dr["idrecargamonedero"].ToString()),
                    TransactionID = dr["TransactionID"].ToString(),
                    PaymentID = dr["PaymentID"].ToString(),
                    monto = decimal.Parse(dr["monto"].ToString()),
                };
                catPaquete p = null;
                Usuario u = LlenaUsr(103);
                catPais ps = LlenaPais(u.idpais);
                Tarjeta t = LlenaTarjeta(148);
                catEstado ed = LlenaEstado(t.idestado);
                catMunicipio m = LlenaMunic(t.idestado, t.idciudad);
                errorvestadetallado = await GeneraDevolucionMON(rm, t, u, rm.PaymentID, rm.monto);
                string stmrs = "update recargamonedero set errvestadetallado = '" + errorvestadetallado + "' where idrecargamonedero = " + rm.idrecargamonedero.ToString();
                ExecuteSQL(stmrs);
            }
        }
        catch (Exception ex)
        {

        }
    }
    
    public async Task Encry()
    {
        string SaltCryptoKey = ConfigurationManager.AppSettings["SaltCryptoKey"];
        string PwdCryptoKey = ConfigurationManager.AppSettings["PwdCryptoKey"];
        DataTable dt = SelectSQL("select * from administrador where idadministrador > 1");
        foreach (DataRow dr in dt.Rows)
        {
            string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
            DataSet ds = new DataSet();
            using (var conn = new MySqlConnection(cs))
            {
                string command = "update administrador set email = @email, numerocontacto = @numerocontacto, name = @name, first_name = @first_name, last_name = @last_name, nip = @nip where idadministrador = @idadministrador";
                using (var cmd = new MySqlCommand(command, conn))
                {
                    cmd.Parameters.Add("@email", MySqlDbType.VarChar, 200).Value = BitConverter.ToString(Crypto.EncryptAes(dr["email"].ToString(), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey))); ;
                    cmd.Parameters.Add("@numerocontacto", MySqlDbType.VarChar, 100).Value = BitConverter.ToString(Crypto.EncryptAes(dr["numerocontacto"].ToString(), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar, 200).Value = BitConverter.ToString(Crypto.EncryptAes(dr["name"].ToString(), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                    cmd.Parameters.Add("@first_name", MySqlDbType.VarChar, 200).Value = (!String.IsNullOrEmpty(dr["first_name"].ToString()) ? BitConverter.ToString(Crypto.EncryptAes(dr["first_name"].ToString(), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey))) : "");
                    cmd.Parameters.Add("@last_name", MySqlDbType.VarChar, 200).Value = (!String.IsNullOrEmpty(dr["last_name"].ToString()) ? BitConverter.ToString(Crypto.EncryptAes(dr["last_name"].ToString(), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey))) : "");
                    cmd.Parameters.Add("@nip", MySqlDbType.VarChar, 50).Value = BitConverter.ToString(Crypto.EncryptAes(dr["nip"].ToString(), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                    cmd.Parameters.Add("@idadministrador", MySqlDbType.Int32).Value = int.Parse(dr["idadministrador"].ToString());
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }*/

    public async Task<string> CargaVesta1aVez(Recarga r, RecargaMonedero rm, string WebSessionID, Tarjeta t, string cvv)
    {
        int err = -1; int errVs = -1; int errRs = -1; string paymentid = ""; int idnuevatarjeta = 0;
        errRecarga e = new errRecarga();
        string riskxml = ""; string json = ""; tresp tresp = new tresp();
        string errorvestadetallado = "";
        string responseString2 = "";
        /*System.Diagnostics.EventLog evt = new System.Diagnostics.EventLog();
        if (!System.Diagnostics.EventLog.SourceExists("AsiCompras"))
        {
            System.Diagnostics.EventLog.CreateEventSource("AsiCompras", "AsiCompras");
        }
        evt.Source = "AsiCompras";
        evt.Log = "AsiCompras";
        evt.WriteEntry("Iniciando Guardamonedero");*/
        //-------------------------------------------------------------
        //  0 Exito en Vs y Exito en Rs = exito
        //  1 Exito en Vs, Rs no responde, tendran q comunicarse para ver manualmente
        //  2 Trono Vs, regresar error al cliente q cheque su tarjeta
        //  3 Error de comunicacion o de bd, vuelva a intentar
        //-------------------------------------------------------------
        //  ERRORES DE VESTA
        //  1 Error de Vesta o comunicaciones, vuelva a intentar
        //  2 Error en TC, esta mal capturada o faltan digitos, revise
        //  3 NSF no tiene fondos suficientes la tarjeta
        //  4 Direccion incorrecta o no coincide con el nombre del titular
        //  5 CVN incorrecto
        //  6 Banco denego la compra, intente con otra tarjeta
        //  7 No aceptamos esta TC, intente con Visa, Discover o MasterCard
        //-------------------------------------------------------------

        if (r == null && rm != null)
        {
            e = new errRecarga { idtarjeta = idnuevatarjeta, PaymentID = paymentid, err = 3, errVs = 11, errRs = errRs, tresp = new tresp { rcode_description = "Por el momento el monedero esta deshabilitado para hacer recargas" } };
            return (JsonConvert.SerializeObject(e));
        }

        try
        {
            catPaquete p = null;
            Usuario u = LlenaUsr((r == null && rm != null ? rm.idusuario : r.idusuario));
            catPais ps = LlenaPais(u.idpais);
            catEstado ed = LlenaEstado(t.idestado);
            catMunicipio m = LlenaMunic(t.idestado, t.idciudad);
            if (rm == null && r != null)
                p = LlenaPaquete(r.idpaquete);

            using (var client = new HttpClient())
            {
                riskxml = GeneraRisk(r, rm, t, p, ps, ed, m, u);
                var values3 = new Dictionary<string, string>
                {
                    { "AccountName", APIUsername },
                    { "CardHolderAddressLine1", (t.calleynumero.Length > 30 ? t.calleynumero.Substring(0, 30) : t.calleynumero) },
                    { "CardHolderCity", (m.municipio.Length > 30 ? m.municipio.Substring(0, 30) : m.municipio) },
                    { "CardHolderCountryCode", ps.codigopais },
                    { "CardHolderFirstName", t.titularFN },
                    { "CardHolderLastName", t.titularLN },
                    { "CardHolderPostalCode", t.codigopostal },
                    { "CardHolderRegion", (ed.estado.Length > 30 ? ed.estado.Substring(0,30) : ed.estado) },
                    { "ChargeAccountNumber", t.permtoken },
                    { "ChargeAccountNumberIndicator", "2"}, //2 token temporal 1 tarjeta
                    { "ChargeAmount", (rm == null && r != null ? p.monto.ToString() : rm.monto.ToString()) },
                    { "ChargeExpirationMMYY", t.expirationMMYY },
                    { "ChargeSource", "WEB" },
                    //{ "Fingerprint", "" },
                    { "MerchantRoutingID", MerchantRoutingID },
                    { "Password", APIPassword },
                    { "TransactionID", (r == null ? rm.TransactionID : r.TransactionID) },
                    { "RiskInformation", riskxml },
                    { "WebSessionID", WebSessionID },
                    { "ChargeCVN", cvv },
                    { "StoreCard", "true" },
                };
                json = JsonConvert.SerializeObject(values3);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response2 = await client.PostAsync(PaymentAPI + "/ChargeSale", new StringContent(json, Encoding.UTF8, "application/json"));
                responseString2 = await response2.Content.ReadAsStringAsync();

                string stmresp = "";
                if (rm == null && r != null)
                    stmresp = "update recarga set riskxml = '" + responseString2 + "' where idrecarga = " + r.idrecarga.ToString();
                else
                    stmresp = "update recargamonedero set riskxml = '" + responseString2 + "' where idrecargamonedero = " + rm.idrecargamonedero.ToString();
                ExecuteSQL(stmresp);

                ChargeSale sale = JsonConvert.DeserializeObject<ChargeSale>(responseString2);
                paymentid = sale.PaymentID;
                if (sale.ResponseCode == 0 && sale.PaymentStatus == 10)
                {
                    RecargaMonederoXReferido(u.idusuario);

                    t.permtoken = sale.ChargePermanentToken;
                    idnuevatarjeta = AltaTarjeta(t);
                    errVs = 0;
                    if (rm == null && r != null)
                    {
                        //-------------------------------------------------------
                        //------    conectar a ws RecargaSell   -----------------
                        //-------------------------------------------------------
                        errRecarga tmp = await RecargaRS(r, p);
                        //if (tmp.tresp.printData.Length > 0)
                        //    printdata = tmp.tresp.printData[0].m_data;
                        err = tmp.err;
                        errRs = tmp.errRs;
                        tresp = tmp.tresp;
                        string stmrs = "";
                        if (errRs == 0)
                        {
                            if (String.IsNullOrEmpty(tmp.tresp.op_authorization) && String.IsNullOrEmpty(tmp.tresp.transaction_id))
                            {
                                if (r != null && rm == null)
                                {
                                    //  18Mar2017
                                    //if (r.idformapago == 2)
                                    //    errorvestadetallado = await GeneraDevolucion(r, t, u, paymentid, p);
                                    if (r.idformapago == 2)
                                    {
                                        errorvestadetallado = GeneraDevolucionViaWallet(r, u, p);
                                        //tresp.rcode_description += Environment.NewLine + "Tu recarga no pudo ser procesada, verifica el número y el operador. Tu saldo se abonó al monedero. Intenta la recarga más tarde y paga seleccionado el saldo del monedero.";
                                        tresp.rcode_description = "Estimado " + u.name + ", hemos abonado el saldo de la última compra a tu monedero, debido a que no logró ser procesada";
                                        EnviaMail(GetConfig(9).ToString(), 'E', "", u, null, r, p);
                                    }
                                    if (r.idformapago == 1)
                                        errorvestadetallado = GeneraDevolucionViaWallet(r, u, p);
                                }
                            }
                            stmrs = "update recarga set rsauthorization = '" + tmp.tresp.op_authorization + "', rstransactionid = '" + tmp.tresp.transaction_id + "', rsrcode = '" + tmp.tresp.rcode_description + "' where idrecarga = " + r.idrecarga.ToString();
                            ExecuteSQL(stmrs);
                            recargaRegaloCat(r.idusuario);
                        }
                        else
                        {
                            //if (String.IsNullOrEmpty(tmp.tresp.op_authorization) && String.IsNullOrEmpty(tmp.tresp.transaction_id))
                            //{
                            if (r != null && rm == null)
                            {
                                //  18Mar2017
                                //if (r.idformapago == 2)
                                //    errorvestadetallado = await GeneraDevolucion(r, t, u, paymentid, p);
                                if (r.idformapago == 2)
                                {
                                    errorvestadetallado = GeneraDevolucionViaWallet(r, u, p);
                                    //tresp.rcode_description += Environment.NewLine + "Tu recarga no pudo ser procesada, verifica el número y el operador. Tu saldo se abonó al monedero. Intenta la recarga más tarde y paga seleccionado el saldo del monedero.";
                                    tresp.rcode_description = "Estimado " + u.name + ", hemos abonado el saldo de la última compra a tu monedero, debido a que no logró ser procesada";
                                    EnviaMail(GetConfig(9).ToString(), 'E', "", u, null, r, p);
                                }
                                if (r.idformapago == 1)
                                    errorvestadetallado = GeneraDevolucionViaWallet(r, u, p);
                            }
                            //}
                            stmrs = "update recarga set rsrcode = '" + tmp.tresp.rcode_description + "' where idrecarga = " + r.idrecarga.ToString();
                            ExecuteSQL(stmrs);
                        }
                    }
                    else
                    {
                        //--------------------------------------------------
                        //----------    monedero    ------------------------
                        //--------------------------------------------------
                        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
                        DataSet ds = new DataSet();
                        using (var conn = new MySqlConnection(cs))
                        {
                            string command = "GuardaMonedero";
                            using (var cmd = new MySqlCommand(command, conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@idusr", MySqlDbType.Int32).Value = u.idusuario;
                                cmd.Parameters.Add("@monto", MySqlDbType.Decimal).Value = rm.monto;
                                conn.Open();
                                try
                                {
                                    if (cmd.ExecuteNonQuery() > 0)
                                        err = 0;
                                    else
                                        err = 1;
                                }
                                catch (Exception exct)
                                {
                                    //evt.WriteEntry(exct.InnerException.ToString());
                                    //evt.WriteEntry(exct.StackTrace);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (sale.AuthResultCode == 1)
                        errVs = 3;
                    if (sale.AVSResultCode != 4096 && sale.AVSResultCode != 0)
                        errVs = 4;
                    if (sale.CVNResultCode != 17 && sale.CVNResultCode != 0)
                        errVs = 5;
                    if (sale.PaymentStatus != 6 && sale.PaymentStatus != 10)
                        errVs = 6;
                    if (sale.PaymentStatus == 6)
                        errVs = 1;
                    List<int> salerrs5 = new List<int> { 510, 511, 512, 513, 514 };
                    if (sale.ResponseCode == 1 || salerrs5.Contains(sale.ResponseCode) || sale.ResponseCode == 1005)
                        errVs = 1;
                    if (sale.ResponseCode == 521 || sale.ResponseCode == 1002 || sale.ResponseCode == 1003)
                        errVs = 2;
                    if (sale.ResponseCode == 1014)
                        errVs = 7;
                    if (sale.ResponseCode == 1016)
                        errVs = 4;
                    err = 2;
                    errorvestadetallado = "Charge= AuthResultCode: " + sale.AuthResultCode + " AVSResultCode: " + sale.AVSResultCode + " CVNResultCode: " + sale.CVNResultCode + " PaymentStatus: " + sale.PaymentStatus + " ResponseCode: " + sale.ResponseCode;

                    int numhoy = int.Parse(SelectCountSQL("SET time_zone = '-6:00'; select count(distinct r.idrecarga), date_format(u.fechaalta, '%Y%m%d'), date_format(now(), '%Y%m%d') from recarga r left join usuario u on r.idusuario = u.idusuario where r.idusuario = 201 and r.idformapago = 2 and r.errVs = 0 and date_format(u.fechaalta, '%Y%m%d') = date_format(now(), '%Y%m%d')").ToString());
                    if (numhoy == 3)
                        tresp.rcode_description = "En su primer dia solo se permite hacer 3 recargas. Si desea hacer mas, intente mañana.";
                }
            }
        }
        catch (Exception ex)
        {
            //evt.WriteEntry(ex.InnerException.ToString() + " " + ex.StackTrace.ToString());
        }
        string stm = "";
        if (rm == null && r != null)
            stm = "update recarga set chargesale = '" + json + "', riskxml = '" + responseString2 + "', idtarjeta = " + idnuevatarjeta.ToString() + ", errvestadetallado = '" + errorvestadetallado + "', err = " + err.ToString() + ", errVs = " + errVs.ToString() + ", errRs = " + errRs.ToString() + ", PaymentID = '" + paymentid + "' where idrecarga = " + r.idrecarga.ToString();
        else
            stm = "update recargamonedero set chargesale = '" + json + "', riskxml = '" + responseString2 + "', idtarjeta = " + idnuevatarjeta.ToString() + ", errvestadetallado = '" + errorvestadetallado + "', err = " + err.ToString() + ", errVs = " + errVs.ToString() + ", PaymentID = '" + paymentid + "' where idrecargamonedero = " + rm.idrecargamonedero.ToString();
        ExecuteSQL(stm);

        e = new errRecarga { idtarjeta = idnuevatarjeta, PaymentID = paymentid, err = err, errVs = errVs, errRs = errRs, tresp = tresp };
        return (JsonConvert.SerializeObject(e));
    }

    private string GeneraDevolucionViaWallet(Recarga r, Usuario u, catPaquete cp)
    {
        string ret = "";
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();
        using (var conn = new MySqlConnection(cs))
        {
            string command = "GuardaMonedero";
            using (var cmd = new MySqlCommand(command, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@idusr", MySqlDbType.Int32).Value = u.idusuario;
                cmd.Parameters.Add("@monto", MySqlDbType.Decimal).Value = cp.monto;
                conn.Open();
                if (cmd.ExecuteNonQuery() > 0)
                    ret = "Se Regreso " + cp.monto.ToString("c") + " al monedero";
            }
        }
        return (ret);
    }

    private async Task<string> GeneraDevolucion(Recarga r, Tarjeta t, Usuario u, string paymentid, catPaquete cp)
    {
        string errorvestadetallado = "";
        using (var client = new HttpClient())
        {
            ReversePayment rev = new ReversePayment();
            string json = "";
            //if (r != null && r.idformapago == 2) //tarjeta
            //{
                var values2 = new Dictionary<string, string>
                    {
                        { "AccountName", APIUsername },
                    //{ "ChargeAccountNumber", t.numero },
                    //{ "ChargeAccountNumberIndicator", "1" },
                    //-----------------------------------------------------------------------
                    //{ "ChargeExpirationMMYY", t.expiramm.ToString() + t.expirayy.ToString() },
                    //-----------------------------------------------------------------------
                        { "MerchantRoutingID", MerchantRoutingID },
                        { "PartnerCustomerKey", u.idusuario.ToString() },
                        { "Password", APIPassword },
                        { "PaymentID", paymentid },
                        { "RefundAmount", cp.monto.ToString() },
                        //{ "ReportingInformation", "" },
                        { "TransactionID", r.TransactionID },
                    };
                json = JsonConvert.SerializeObject(values2);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsync(PaymentAPI + "/ReversePayment", new StringContent(json, Encoding.UTF8, "application/json"));
                var responseString = await response.Content.ReadAsStringAsync();
                rev = JsonConvert.DeserializeObject<ReversePayment>(responseString);
                errorvestadetallado = "ReversePayment= PaymentStatus: " + rev.PaymentStatus.ToString() + " ResponseCode: " + rev.ResponseCode.ToString() + " ResponseText: " + rev.ResponseText + " ReversalAction:" + rev.ReversalAction.ToString();
            //}
        }
        return (errorvestadetallado);
    }

    private static async Task<errRecarga> ChecaRS(string treqid, TimeSpan intervalo, CancellationTokenSource ts)
    {
        errRecarga e = new errRecarga();
        int errRs = -1;
        wsRecargaSell.TResponse tresp;
        wsRecargaSell.transact wsr = new wsRecargaSell.transact();
        wsr.Url = ConfigurationManager.AppSettings["wsRecargaSell"];
        wsr.Timeout = 20000;
        while (true)
        {
            tresp = wsr.CheckTransaction(treqid, ConfigurationManager.AppSettings["userrecsell"]);
            e.tresp = new global::tresp { op_authorization = tresp.op_authorization, transaction_id = tresp.transaction_id.ToString(), rcode_description = tresp.rcode_description, printDatam_data = (tresp.printData.Length > 0 ? tresp.printData[0].m_data : "") };
            if (tresp.rcode != 0)  //en proceso
                e.errRs = 8;
            if (tresp.rcode == 0)  //exito
            {
                e.errRs = 0;
                ts.Cancel();
            }
            Task t = Task.Delay(intervalo, ts.Token);
            try
            {
                await t;
            }
            catch (TaskCanceledException) {
                return (e);
            }
        }
    }

    private async Task<errRecarga> RecargaRS(Recarga r, catPaquete p)
    {
        errRecarga res = new errRecarga();
        wsRecargaSell.transact wsr = new wsRecargaSell.transact();
        wsr.Url = ConfigurationManager.AppSettings["wsRecargaSell"];
        wsr.Timeout = 20000;
        string treqid = wsr.GetTRequestID(ConfigurationManager.AppSettings["userrecsell"], ConfigurationManager.AppSettings["pwdrecsell"], "");
        wsRecargaSell.TResponse tresp = wsr.DoT(treqid, ConfigurationManager.AppSettings["userrecsell"], p.sku, r.numerorecarga, (float)p.monto, null);
        if (tresp.rcode == 2)
        { //en proceso
            //int total = 2; //segundos
            //var t0 = new System.Timers.Timer();
            //t0.Interval = 2000;
            //t0.Elapsed += (object sender, System.Timers.ElapsedEventArgs ea) =>
            //{
            //    total += 2;
            //    tresp = null;
            //    tresp = wsr.CheckTransaction(treqid, ConfigurationManager.AppSettings["userrecsell"]);
            //    if (tresp.rcode == 2)  //en proceso
            //    {
            //        if (total >= 60)
            //        {
            //            t0.Stop();
            //            err = 1;
            //            errRs = 8;
            //        }
            //    }
            //    if (tresp.rcode == 0)  //exito
            //    {
            //        t0.Stop();
            //        err = 0;
            //        errRs = 0;
            //    }
            //    if (tresp.rcode != 0 && tresp.rcode != 2) //error
            //    {
            //        t0.Stop();
            //        err = 1;
            //        errRs = 8;
            //    }
            //};
            //t0.Start();
            CancellationTokenSource ts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            res = await ChecaRS(treqid, TimeSpan.FromSeconds(2), ts);
            if (res.errRs == 0)
                res.err = 0;
            if (res.errRs == 8)
                res.err = 1;
        }
        if (tresp.rcode == 0)
        {
            res.err = 0;
            res.errRs = 0;
        }
        if (tresp.rcode != 0 && tresp.rcode != 2)
        {
            res.err = 1;
            res.errRs = 8;
        }
        res.tresp = new global::tresp { op_authorization = tresp.op_authorization, transaction_id = tresp.transaction_id.ToString(), rcode_description = tresp.rcode_description, printDatam_data = (tresp.printData.Length > 0 ? tresp.printData[0].m_data : "") };
        return (res);
    }
    
    public string GetUser(int idusr, string email, string nip)
    {
        int nummaxdeintentos = int.Parse(GetConfig(3).ToString());
        string SaltCryptoKey = ConfigurationManager.AppSettings["SaltCryptoKey"];
        string PwdCryptoKey = ConfigurationManager.AppSettings["PwdCryptoKey"];
        string ret = "[]";
        const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();
        string command = "";
        if ((idusr > 0 && email.Length == 0) || (idusr == 0 && email.Length > 0 && Regex.IsMatch(email, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))))
        {
            string vemail = BitConverter.ToString(Crypto.EncryptAes(email, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
            string vnip = BitConverter.ToString(Crypto.EncryptAes(nip, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
            if (nip.Length > 0)
            {
                DataTable dtusr = SelectSQL("select * from usuario where email = '" + vemail + "' and nip = '" + vnip + "' and activo=1");
                if (dtusr.Rows.Count == 0)
                {
                    DataTable dtusr2 = SelectSQL("select * from usuario where email = '" + vemail + "' and activo=1");
                    if (dtusr2.Rows.Count > 0)
                    {
                        int intentos = int.Parse(Crypto.DecryptAes(BackBitConv(dtusr2.Rows[0]["intentos"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                        intentos++;
                        if (intentos == nummaxdeintentos)
                        {
                            string bloq = BitConverter.ToString(Crypto.EncryptAes("1", PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                            ExecuteSQL("update usuario set nip = '', bloqueado = '" + bloq + "' where idusuario = " + dtusr2.Rows[0]["idusuario"].ToString());
							string cadena = "https://asicompras.com/rec_nip.aspx?rtgc=" + Uri.EscapeDataString(MyFuntion.numerico.Encripta(nip + DateTime.Now.ToShortDateString() + dtusr2.Rows[0]["idusuario"].ToString(), MyFuntion.numerico.encrip));
                            EnviaMail(email, 'Q', cadena);
                            Usuario ut = new Usuario() { idusuario = -1, email = "", fechaalta = DateTime.MinValue, name = "", nip = "", numerocontacto = "", registrado = false };
                            List<Usuario> lu = new List<Usuario>(); lu.Add(ut);
                            ret = JsonConvert.SerializeObject(lu, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        }
                        else
                        {
                            string intenc = BitConverter.ToString(Crypto.EncryptAes(intentos.ToString(), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                            ExecuteSQL("update usuario set intentos = '" + intenc + "' where idusuario = " + dtusr2.Rows[0]["idusuario"].ToString());
                        }
                    }
                }
            }
            else {
                string bloq = BitConverter.ToString(Crypto.EncryptAes("1", PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                DataTable dtusr3 = SelectSQL("select * from usuario where email = '" + vemail + "' and bloqueado = '" + bloq + "' and activo=1");
                if (dtusr3.Rows.Count > 0)
                {
                    string cadena = "https://asicompras.com/rec_nip.aspx?rtgc=" + Uri.EscapeDataString(MyFuntion.numerico.Encripta(nip + DateTime.Now.ToShortDateString() + dtusr3.Rows[0]["idusuario"].ToString(), MyFuntion.numerico.encrip));
                    EnviaMail(email, 'Q', cadena);
                    Usuario ut = new Usuario() { idusuario = -1, email = "", fechaalta = DateTime.MinValue, name = "", nip = "", numerocontacto = "", registrado = false };
                    List<Usuario> lu = new List<Usuario>(); lu.Add(ut);
                    ret = JsonConvert.SerializeObject(lu, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }
            }
            using (var conn = new MySqlConnection(cs))
            {
                if (idusr > 0)
                    command = "select * from usuario where idusuario = @idusuario and bloqueado = @bloqueado and activo=1 limit 1;";
                else
                {
                    if (nip.Length > 0)
                        command = "select * from usuario where email = @email and nip = @nip and bloqueado = @bloqueado and activo=1 limit 1;";
                    else
                        command = "select * from usuario where email = @email and bloqueado = @bloqueado and activo=1 limit 1;";
                }
                using (var cmd = new MySqlCommand(command, conn))
                {
                    if (idusr > 0)
                    {
                        cmd.Parameters.Add("@idusuario", MySqlDbType.Int32).Value = idusr;
                        cmd.Parameters.Add("@bloqueado", MySqlDbType.VarChar, 50).Value = BitConverter.ToString(Crypto.EncryptAes("0", PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                    }
                    else
                    {
                        cmd.Parameters.Add("@email", MySqlDbType.VarChar, 200).Value = vemail;
                        if (nip.Length > 0)
                            cmd.Parameters.Add("@nip", MySqlDbType.VarChar, 50).Value = BitConverter.ToString(Crypto.EncryptAes(nip, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                        cmd.Parameters.Add("@bloqueado", MySqlDbType.VarChar, 50).Value = BitConverter.ToString(Crypto.EncryptAes("0", PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                    }

                    conn.Open();
                    MySqlDataAdapter adapt = new MySqlDataAdapter(cmd);
                    adapt.Fill(ds, "cat");
                    if (ds.Tables["cat"].Rows.Count > 0)
                    {
                        string bloq = BitConverter.ToString(Crypto.EncryptAes("0", PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                        ExecuteSQL("update usuario set intentos = '" + bloq + "', bloqueado = '" + bloq + "' where idusuario = " + ds.Tables["cat"].Rows[0]["idusuario"].ToString());
                        foreach (DataRow dr in ds.Tables["cat"].Rows)
                        {
                            dr["email"] = (!String.IsNullOrEmpty(dr["email"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["email"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : "");
                            dr["name"] = (!String.IsNullOrEmpty(dr["name"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["name"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : "");
                            dr["first_name"] = (!String.IsNullOrEmpty(dr["first_name"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["first_name"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : "");
                            dr["last_name"] = (!String.IsNullOrEmpty(dr["last_name"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["last_name"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : "");
                            dr["nip"] = (!String.IsNullOrEmpty(dr["nip"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["nip"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : "");
                            dr["numerocontacto"] = (!String.IsNullOrEmpty(dr["numerocontacto"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["numerocontacto"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : "");
                        }
                        ret = JsonConvert.SerializeObject(ds.Tables["cat"], Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    }
                }
            }
        }
        return (ret);
    }

    public string GetAdmin(int idusr, string email, string nip)
    {
        string SaltCryptoKey = ConfigurationManager.AppSettings["SaltCryptoKey"];
        string PwdCryptoKey = ConfigurationManager.AppSettings["PwdCryptoKey"];
        string ret = "[]";
        const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();
        string command = "";
        if ((idusr > 0 && email.Length == 0) || (idusr == 0 && email.Length > 0 && Regex.IsMatch(email, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))))
        {
            using (var conn = new MySqlConnection(cs))
            {
                if (idusr > 0)
                    command = "select * from administrador where idadministrador = @idusuario;";
                else
                    command = "select * from administrador where email = @email and nip = @nip;";
                using (var cmd = new MySqlCommand(command, conn))
                {
                    if (idusr > 0)
                        cmd.Parameters.Add("@idusuario", MySqlDbType.Int32).Value = idusr;
                    else
                    {
                        cmd.Parameters.Add("@email", MySqlDbType.VarChar, 200).Value = BitConverter.ToString(Crypto.EncryptAes(email, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                        cmd.Parameters.Add("@nip", MySqlDbType.VarChar, 50).Value = BitConverter.ToString(Crypto.EncryptAes(nip, PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)));
                    }
                    conn.Open();
                    MySqlDataAdapter adapt = new MySqlDataAdapter(cmd);
                    adapt.Fill(ds, "cat");
                    foreach (DataRow dr in ds.Tables["cat"].Rows)
                    {
                        dr["email"] = (!String.IsNullOrEmpty(dr["email"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["email"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : "");
                        dr["name"] = (!String.IsNullOrEmpty(dr["name"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["name"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : "");
                        dr["first_name"] = (!String.IsNullOrEmpty(dr["first_name"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["first_name"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : "");
                        dr["last_name"] = (!String.IsNullOrEmpty(dr["last_name"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["last_name"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : "");
                        dr["nip"] = (!String.IsNullOrEmpty(dr["nip"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["nip"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : "");
                        dr["numerocontacto"] = (!String.IsNullOrEmpty(dr["numerocontacto"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["numerocontacto"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : "");
                    }
                    ret = JsonConvert.SerializeObject(ds.Tables["cat"], Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }
            }
        }
        return (ret);
    }

    public int GetMonederoHab()
    {
        int hab = 0;
        int.TryParse(GetConfig(5).ToString(), out hab);
        return (hab);
    }

    public decimal GetMontoMax()
    {
        decimal monto = 0M;
        decimal.TryParse(GetConfig(2).ToString(), out monto);
        return (monto);
    }

    public string GetCatalogo(int idcatalogo, string where)
    {
        string nombrecat = "";
        switch (idcatalogo)
        {
            case 1:
                nombrecat = "catoperadora"; break;
            case 2:
                nombrecat = "catpais"; break;
            case 3:
                nombrecat = "catpaquete";
                ActualizaPaquetes();
                break;
            case 4:
                nombrecat = "catemisortc"; break;
            case 5:
                nombrecat = "catformaspago"; break;
            case 6:
                nombrecat = "caterrores"; break;
            case 7:
                nombrecat = "catestado"; break;
            case 8:
                nombrecat = "catmunicipio"; break;
            case 9:
                nombrecat = "tarjeta"; break;
            case 10:
                nombrecat = "usuario"; break;
            case 11:
                nombrecat = "recarga"; break;
            case 12:
                nombrecat = "saldomonedero"; break;
            case 13:
                nombrecat = "recargaprog"; break;
            case 14:
                nombrecat = "recargamonedero"; break;
            case 15:
                nombrecat = "administrador"; break;
        }
        string json = "";
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();
        using (var conn = new MySqlConnection(cs))
        {
            string command = "select * from " + nombrecat + " ";
            if (where != "")
                command += where;
            using (var cmd = new MySqlCommand(command, conn))
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter(cmd);
                conn.Open();
                adapt.Fill(ds, "cat");
                json = JsonConvert.SerializeObject(ds.Tables["cat"], Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }
        return (json);
    }

    public string GetCatalogoPersonalizado(string consulta)
    {
        
        string json = "";
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();
        using (var conn = new MySqlConnection(cs))
        {
            string command = consulta;

            using (var cmd = new MySqlCommand(command, conn))
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter(cmd);
                conn.Open();
                adapt.Fill(ds, "cat");
                json = JsonConvert.SerializeObject(ds.Tables["cat"], Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }
        return (json);
    }

    public byte[] ExportaRepUsr()
    {
        byte[] ret;
        DataTable dt = new DataTable();
        dt = JsonConvert.DeserializeObject<DataTable>(GetUsers());
        using (var exporter = new NpoiExport())
        {
            exporter.ExportDataTableToWorkbook(dt, "Results", "U");
            ret = exporter.GetBytes();
        }
        return (ret);
    }

    public byte[] ExportaRepConc(string fini, string ffin)
    {
        byte[] ret;
        DataTable dt = new DataTable();
        dt = GeneraTablaRep(fini, ffin);
        using (var exporter = new NpoiExport())
        {
            exporter.ExportDataTableToWorkbook(dt, "Results", "C");
            ret = exporter.GetBytes();
        }
        return (ret);
    }

    public void EnviaReporteConciliacion(string fini, string ffin)
    {
        byte[] rep = ExportaRepConc(fini, ffin);
        string emails = GetConfig(6).ToString();
        EnviaMail(emails, 'R', "", null, rep);
    }

    public void EnviaReporteUsuarios()
    {
        byte[] ret = ExportaRepUsr();
        string emails = GetConfig(6).ToString();
        EnviaMail(emails, 'U', "", null, ret);
    }

    public string GetUsers()
    {
        string SaltCryptoKey = ConfigurationManager.AppSettings["SaltCryptoKey"];
        string PwdCryptoKey = ConfigurationManager.AppSettings["PwdCryptoKey"];
        string json = "";
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();
        using (var conn = new MySqlConnection(cs))
        {
            string command = "select u.idusuario, u.fechaalta, (select fecha from recarga where err = 0 and idusuario = u.idusuario order by fecha asc limit 1) as FechaPrimera, (select fecha from recarga where err = 0 and idusuario = u.idusuario order by fecha desc limit 1) as FechaUltima, (select count(idrecarga) from recarga where err = 0 and idusuario = u.idusuario) as NumRecargas, u.email, u.name, u.numerocontacto from usuario u";
            using (var cmd = new MySqlCommand(command, conn))
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter(cmd);
                conn.Open();
                adapt.Fill(ds, "sql");
            }
            foreach (DataRow dr in ds.Tables["sql"].Rows)
            {
                dr["email"] = (!String.IsNullOrEmpty(dr["email"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["email"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : "");
                dr["name"] = (!String.IsNullOrEmpty(dr["name"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["name"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : "");
                dr["numerocontacto"] = (!String.IsNullOrEmpty(dr["numerocontacto"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["numerocontacto"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : "");
            }
            json = JsonConvert.SerializeObject(ds.Tables["sql"], Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
        return (json);
    }

    public bool verificaCliente(string verif) {
        bool res =false;
        DataSet ds = new DataSet();
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;

       using (var conn = new MySqlConnection(cs))
        {
            string command ="select * from usuario where verificacion = '" + verif + "' and activo = '0'";

            using (var cmd = new MySqlCommand(command, conn))
            {
             
                MySqlDataAdapter adapt = new MySqlDataAdapter(cmd);
                conn.Open();
                adapt.Fill(ds, "verf");
                if (ds.Tables["verf"].Rows.Count > 0)
                {
                    if (ExecuteSQL("update usuario set activo = '1' where verificacion = '" + verif+"'") != 0)
                    {
                        res = true;
                    }
                }
            }
            
        }
        return res;
    }

    public string GetAClientes(int tipo) {
        string SaltCryptoKey = ConfigurationManager.AppSettings["SaltCryptoKey"];
        string PwdCryptoKey = ConfigurationManager.AppSettings["PwdCryptoKey"];
        string json = "";
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();
        string command = "";
        using (var conn = new MySqlConnection(cs))
        {
            switch (tipo) {
                case 1: { //altas y ventas
                        command = "Select count(u.idusuario) as altas ,week(u.fechaalta)  as semana , year(u.fechaalta) as anio ,(select sum(p.monto) from recarga r inner join catpaquete p on p.idpaquete = r.idpaquete where week(r.fecha) = semana and r.err=0) ventas from usuario u where u.activo=1 group by anio,semana order by u.fechaalta desc limit 100;";
                        break;
                    }
                case 2:
                    { //eventos
                        command = "Select count(r.idrecarga) as altas ,week(r.fecha)  as semana , year(r.fecha) as anio,count(r.idrecarga) as ventas  from recarga r group by anio,semana order by r.fecha desc limit 100;";
                        break;
                    }
            }
                
            using (var cmd = new MySqlCommand(command, conn))
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter(cmd);
                conn.Open();
                adapt.Fill(ds, "sql");
            }
            json = JsonConvert.SerializeObject(ds.Tables["sql"], Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
        return (json);
    }

    public string GetMesDatos(int tipo) {
        string SaltCryptoKey = ConfigurationManager.AppSettings["SaltCryptoKey"];
        string PwdCryptoKey = ConfigurationManager.AppSettings["PwdCryptoKey"];
        string json = "";
        string command="";
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();
        //set time 
        ExecuteSQL("SET lc_time_names = 'es_MX';");
        using (var conn = new MySqlConnection(cs))
        {
            switch (tipo)
            {
                case 1:
                    { //altas 
                        command = " select monthname(u.fechaalta) as mes, year(u.fechaalta) as anio, count(u.idusuario) as num from usuario u   where u.activo = 1 group by anio, mes order by u.fechaalta desc limit 100;";
                        break;
                    }
                case 2:
                    { //ventas
                        command = "select monthname(r.fecha) as mes, year(r.fecha) as anio , SUM(p.monto) as num from recarga r  inner join catpaquete p on p.idpaquete = r.idpaquete   where r.err = 0 group by anio,mes order by r.fecha desc limit 100;";
                        break;
                    }
                case 3: {
                        //eventos
                        command = "select monthname(r.fecha) as mes, year(r.fecha) as anio , count(r.idrecarga) as num from recarga r   group by anio,mes order by r.fecha desc limit 100;";
                        break;
                    }
            }
           
            using (var cmd = new MySqlCommand(command, conn))
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter(cmd);
                conn.Open();
                adapt.Fill(ds, "sql");
            }
            json = JsonConvert.SerializeObject(ds.Tables["sql"], Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
        return (json);

    }

    public string GetDiaDatos(int tipo) {
        string SaltCryptoKey = ConfigurationManager.AppSettings["SaltCryptoKey"];
        string PwdCryptoKey = ConfigurationManager.AppSettings["PwdCryptoKey"];
        string json = "";
        string command = "";
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();

        using (var conn = new MySqlConnection(cs))
        {
            switch (tipo)
            {
                case 1:
                    { //altas 
                        command = "select day(fechaalta)  dia,count(u.idusuario) num from usuario u where week(fechaalta)=week(CURDATE()) and activo=1 group by day(fechaalta) order by fechaalta limit 100;";
                        break;
                    }
                case 2:
                    { //ventas
                        command = "select day(r.fecha) dia,sum(p.monto) num from recarga r inner join catpaquete p on p.idpaquete = r.idpaquete where week(r.fecha)=week(CURDATE()) and r.err=0 group by day(r.fecha) order by r.fecha limit 100;";
                        break;
                    }
                case 3:
                    {
                        //eventos
                        command = "select day(r.fecha) dia,count(r.idrecarga) num from recarga r  where week(r.fecha)=week(CURDATE())  group by day(r.fecha) order by r.fecha desc limit 100;";
                        break;
                    }
            }
            
            using (var cmd = new MySqlCommand(command, conn))
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter(cmd);
                conn.Open();
                adapt.Fill(ds, "sql");
            }
            json = JsonConvert.SerializeObject(ds.Tables["sql"], Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
        return (json);
    }

    private string PaymentStatus(int ps)
    {
        string ret = "";
        switch (ps)
        {
            case 1:
                ret = "Failed"; break;
            case 3:
                ret = "Denied"; break;
            case 4:
                ret = "Cancelled"; break;
            case 6:
                ret = "Failed Payment"; break;
            case 10:
                ret = "Successful Payment"; break;
            case 13:
                ret = "Business Rules Denied"; break;
            case 100:
                ret = "Failed Payment"; break;
        }
        return (ret);
    }

    private DataTable GeneraTablaRep(string fini, string ffin)
    {
        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[] {
                new DataColumn() { ColumnName = "err", DataType = Type.GetType("System.Int32") },
                new DataColumn() { ColumnName = "Operadora", DataType = Type.GetType("System.String") },
                new DataColumn() { ColumnName = "Paquete", DataType = Type.GetType("System.String") },
                new DataColumn() { ColumnName = "Numero", DataType = Type.GetType("System.String") },
                new DataColumn() { ColumnName = "Fecha", DataType = Type.GetType("System.String") },
                new DataColumn() { ColumnName = "Payment", DataType = Type.GetType("System.String") },
                new DataColumn() { ColumnName = "Transaccion", DataType = Type.GetType("System.String") },
                new DataColumn() { ColumnName = "Code", DataType = Type.GetType("System.String") },
                new DataColumn() { ColumnName = "Vesta", DataType = Type.GetType("System.String") },
                new DataColumn() { ColumnName = "IdUsuario", DataType = Type.GetType("System.Int32") },

                new DataColumn() { ColumnName = "Amount", DataType = Type.GetType("System.String") },
                new DataColumn() { ColumnName = "FechaVesta", DataType = Type.GetType("System.String") },
                new DataColumn() { ColumnName = "PaymentStatus", DataType = Type.GetType("System.String") },
                new DataColumn() { ColumnName = "Reason", DataType = Type.GetType("System.String") },

                new DataColumn() { ColumnName = "FechaRS", DataType = Type.GetType("System.String") },
                new DataColumn() { ColumnName = "Celular", DataType = Type.GetType("System.String") },
                new DataColumn() { ColumnName = "Monto", DataType = Type.GetType("System.String") },
                new DataColumn() { ColumnName = "Carrier", DataType = Type.GetType("System.String") },
                new DataColumn() { ColumnName = "Folio", DataType = Type.GetType("System.String") },
            });

        string json = GetReporteConciliacion(fini, ffin);
        repconc rep = JsonConvert.DeserializeObject<repconc>(json);
        DataRow drtmp = null;
        foreach (DataRow dr in rep.recargas.Rows)
        {
            drtmp = dt.NewRow();
            foreach (DataColumn dc in dt.Columns)
            {
                switch (dc.ColumnName.ToLower())
                {
                    case "numero":
                        drtmp["Numero"] = dr["numerorecarga"]; break;
                    case "payment":
                        drtmp["payment"] = dr["PaymentID"]; break;
                    case "transaccion":
                        drtmp["transaccion"] = (rep.recargas.Columns.Contains("rstransactionid") ? dr["rstransactionid"].ToString() : ""); break;
                    case "code":
                        drtmp["code"] = (rep.recargas.Columns.Contains("rsrcode") ? dr["rsrcode"].ToString() : ""); break;
                    case "vesta":
                        drtmp["vesta"] = dr["errvestadetallado"]; break;
                    default:
                        if (rep.recargas.Columns.Contains(dc.ColumnName))
                            drtmp[dc.ColumnName] = dr[dc.ColumnName]; break;
                }
            }

            List<string> ls = new List<string> { "0", "1", "2" };
            if (ls.Contains(dr["err"].ToString()))
            {
                DataRow[] drvesta = (rep.vesta.Rows.Count > 0 ? rep.vesta.Select("paymentid = '" + dr["PaymentID"].ToString() + "'") : new DataRow[0]);
                DataRow[] drrs = (rep.rs.Rows.Count > 0 ? rep.rs.Select("idtransaccion = '" + dr["rstransactionid"].ToString() + "'") : new DataRow[0]);
                if (drvesta.Count() > 0)
                {
                    drtmp["Amount"] = (decimal.Parse(drvesta[0]["amount"].ToString())).ToString("c");
                    drtmp["FechaVesta"] = drvesta[0]["vestadate"].ToString();
                    drtmp["PaymentStatus"] = PaymentStatus(int.Parse(drvesta[0]["paymentstatus"].ToString()));
                    drtmp["Reason"] = drvesta[0]["paymentstatusreason"].ToString();
                }
                if ((dr["err"].ToString() == "0") && (drrs.Count() > 0))
                {
                    drtmp["FechaRS"] = drrs[0]["fecha"].ToString();
                    drtmp["Celular"] = drrs[0]["celular"].ToString();
                    drtmp["Monto"] = (decimal.Parse(drrs[0]["monto"].ToString())).ToString("c");
                    drtmp["Carrier"] = drrs[0]["carrier"].ToString();
                    drtmp["Folio"] = drrs[0]["folio"].ToString();
                }
            }
            dt.Rows.Add(drtmp);
        }
        return (dt);
    }

    public string GetReporteConciliacion(string fini, string ffin)
    {
        DataTable dt1 = SelectSQL(@"select co.operadora, cp.paquete, r.numerorecarga, r.fecha, r.PaymentID, r.errvestadetallado, r.rstransactionid, r.rsrcode, r.err, r.idusuario from recarga r left join catoperadora co on r.idoperadora = co.idoperadora
left join catpaquete cp on r.idpaquete = cp.idpaquete
where r.fecha between str_to_date('" + fini + @"', '%Y%m%d') and str_to_date('" + ffin + @"', '%Y%m%d')
order by r.fecha");

        DataTable dt2 = SelectSQL(@"select v.paymentid, v.amount, v.vestadate, v.paymentstatus, v.paymentstatusreason, v.originalpaymentid from vestadialy v
where v.vestadate between str_to_date('" + fini + @"', '%Y%m%d') and str_to_date('" + ffin + @"', '%Y%m%d')");

        DataTable dt3 = SelectSQL(@"select r.idtransaccion, r.fecha, r.celular, r.monto, r.resultado, r.carrier, r.folio from rsdialy r
where r.fecha between str_to_date('" + fini + @"', '%Y%m%d') and str_to_date('" + ffin + @"', '%Y%m%d')");

        repconc r = new repconc() { recargas = dt1, vesta = dt2, rs = dt3 };

        string json = JsonConvert.SerializeObject(r, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        return (json);
    }

    #endregion

    #region BD
    private MasTicket.catMunicipio LlenaMunic(int idedo, int idmunic)
    {
        catMunicipio p = new catMunicipio();
        DataTable dt = SelectSQL("select * from catmunicipio where idmunicipio = " + idmunic.ToString() + " and idestado = " + idedo.ToString());
        if (dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[0];
            p = new catMunicipio
            {
                idmunicipio = idmunic,
                municipio = dr["municipio"].ToString(),
                idestado = (int)dr["idestado"],
            };
        }
        return (p);
    }

    private MasTicket.catEstado LlenaEstado(int idedo)
    {
        catEstado p = new catEstado();
        DataTable dt = SelectSQL("select * from catestado where idestado = " + idedo.ToString());
        if (dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[0];
            p = new catEstado
            {
                idestado = idedo,
                estado = dr["estado"].ToString(),
                idpais = (int)dr["idpais"],
            };
        }
        return (p);
    }

    private MasTicket.catPais LlenaPais(int idpais)
    {
        catPais p = new catPais();
        DataTable dt = SelectSQL("select * from catpais where idpais = " + idpais.ToString());
        if (dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[0];
            p = new catPais
            {
                idpais = idpais,
                pais = dr["pais"].ToString(),
                img = dr["img"].ToString(),
                codigotel = dr["codigotel"].ToString(),
                paisdefault = (int.Parse(dr["paisdefault"].ToString()) == 1 ? true : false),
                codigopais = dr["codigopais"].ToString(),
            };
        }
        return (p);
    }

    private MasTicket.Usuario LlenaUsr(int idusr)
    {
        string SaltCryptoKey = ConfigurationManager.AppSettings["SaltCryptoKey"];
        string PwdCryptoKey = ConfigurationManager.AppSettings["PwdCryptoKey"];

        Usuario u = new Usuario();
        DataTable dt = SelectSQL("select * from usuario where idusuario = " + idusr.ToString());
        if (dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[0];
            u = new Usuario
            {
                email = (!String.IsNullOrEmpty(dr["email"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["email"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : ""),
                name = (!String.IsNullOrEmpty(dr["name"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["name"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : ""),
                first_name = (!String.IsNullOrEmpty(dr["first_name"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["first_name"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : ""),
                last_name = (!String.IsNullOrEmpty(dr["last_name"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["last_name"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : ""),
                nip = (!String.IsNullOrEmpty(dr["nip"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["nip"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : ""),
                numerocontacto = (!String.IsNullOrEmpty(dr["numerocontacto"].ToString()) ? Crypto.DecryptAes(BackBitConv(dr["numerocontacto"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)) : ""),
                idusuario = idusr,
                gender = dr["gender"].ToString(),
                picture = dr["picture"].ToString(),
                idpais = (int)dr["idpais"],
                fechaalta = (DateTime)dr["fechaalta"]
            };
        }
        return (u);
    }

    private MasTicket.catPaquete LlenaPaquete(int idpaquete)
    {
        catPaquete p = new catPaquete();
        DataTable dt = SelectSQL("select * from catpaquete where idpaquete = " + idpaquete.ToString());
        if (dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[0];
            p = new catPaquete
            {
                idpaquete = idpaquete,
                sku = dr["sku"].ToString(),
                paquete = dr["paquete"].ToString(),
                monto = (decimal)dr["monto"],
                idoperadora = (int)dr["idoperadora"],
                tipo = (int)dr["tipo"],
            };
        }
        return (p);
    }

    private byte[] BackBitConv(string str)
    {
        String[] arr = str.Split('-');
        byte[] array = new byte[arr.Length];
        for (int i = 0; i < arr.Length; i++) array[i] = Convert.ToByte(arr[i], 16);
        return (array);
    }

    public MasTicket.Tarjeta LlenaTarjeta(int idtarjeta)
    {
        string SaltCryptoKey = ConfigurationManager.AppSettings["SaltCryptoKey"];
        string PwdCryptoKey = ConfigurationManager.AppSettings["PwdCryptoKey"];

        Tarjeta t = new Tarjeta();
        DataTable dt = SelectSQL("select * from tarjeta where idtarjeta = " + idtarjeta.ToString());
        if (dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[0];
            t = new Tarjeta
            {
                idtarjeta = idtarjeta,
                idpais = (int)dr["idpais"],
                idemisor = (int)dr["idemisor"],
                titularFN = Crypto.DecryptAes(BackBitConv(dr["titularFN"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)),
                titularLN = Crypto.DecryptAes(BackBitConv(dr["titularLN"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)),
                calleynumero = Crypto.DecryptAes(BackBitConv(dr["calleynumero"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)),
                idciudad = (int)dr["idciudad"],
                codigopostal = Crypto.DecryptAes(BackBitConv(dr["codigopostal"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)),
                idestado = (int)dr["idestado"],
                permtoken = Crypto.DecryptAes(BackBitConv(dr["permtoken"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)),
                Last4 = dr["last4"].ToString(),
                expirationMMYY = Crypto.DecryptAes(BackBitConv(dr["expirationMMYY"].ToString()), PwdCryptoKey, Crypto.String2ByteArray(SaltCryptoKey)),
                //idusuario = (int)dr["idusuario"],
            };
        }
        return (t);
    }

    private List<catPaquete> GetPaquetesRS()
    {
        wsRecargaSell.transact wsr = new wsRecargaSell.transact();
        wsr.Url = ConfigurationManager.AppSettings["wsRecargaSell"];
        string xml = wsr.GetSkuList(ConfigurationManager.AppSettings["userrecsell"], ConfigurationManager.AppSettings["pwdrecsell"]);
        DataSet ds = new DataSet();
        ds.ReadXml(new XmlTextReader(new StringReader(xml)));
        List<product> lspro = ds.Tables[0].AsEnumerable().Select(x => new product { name = x.Field<string>("name"), sku = x.Field<string>("sku"), monto = x.Field<string>("monto") + ".00", info = x.Field<string>("info"), stype = x.Field<string>("stype"), opid = x.Field<string>("opid") }).ToList<product>();
        List<catPaquete> lspaq = lspro.Select(x => new catPaquete { sku = x.sku, paquete = x.name, monto = decimal.Parse(x.monto), idoperadora = int.Parse(x.opid), tipo = int.Parse(x.stype) }).ToList<catPaquete>();
        return (lspaq);
    }
    
    private void ActualizaPaquetes()
    {
        string fechaant = GetConfig(1).ToString();
        DateTime dt = DateTime.MinValue;
        DateTime.TryParseExact(fechaant, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
        if (dt.Date < DateTime.Now.Date)
        {
            List<catPaquete> lspaqNew = GetPaquetesRS();
            string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
            DataSet ds = new DataSet();
            using (var conn = new MySqlConnection(cs))
            {
                string command = "select sku, paquete, monto, tipo, idoperadora from catpaquete";
                using (var cmd = new MySqlCommand(command, conn))
                {
                    MySqlDataAdapter adapt = new MySqlDataAdapter(cmd);
                    conn.Open();
                    adapt.Fill(ds, "cat");
                    List<catPaquete> lspaqOld = ds.Tables["cat"].AsEnumerable().Select(x => new catPaquete { sku = x.Field<string>("sku"), paquete = x.Field<string>("paquete"), monto = x.Field<decimal>("monto"), tipo = x.Field<int>("tipo"), idoperadora = x.Field<int>("idoperadora") }).ToList<catPaquete>();

                    //  nuevos montos - dar de alta
                    var ids2 = new HashSet<string>(lspaqOld.Select(d => d.sku));
                    IEnumerable<catPaquete> nuevos = lspaqNew.Where(x => !ids2.Contains(x.sku));
                    if (nuevos.Count() > 0)
                        foreach (catPaquete cp in nuevos)
                            ExecuteSQL("insert into catpaquete (sku, paquete, monto, tipo, idoperadora) values ('" + cp.sku + "', '" + cp.paquete + "', " + cp.monto.ToString() + ", " + cp.tipo + ", " + cp.idoperadora + ")");
                    //  viejos montos - eliminar
                    var ids = new HashSet<string>(lspaqNew.Select(d => d.sku));
                    IEnumerable<catPaquete> viejos = lspaqOld.Where(x => !ids.Contains(x.sku));
                    if (viejos.Count() > 0)
                        foreach (catPaquete cp in viejos)
                            ExecuteSQL("delete from catpaquete where sku = '" + cp.sku + "'");
                    ExecuteSQL("update config set valor = '" + DateTime.Now.ToString("yyyyMMdd") + "' where idconfig = 1");
                }
            }
        }
    }

    private object GetConfig(int idconfig)
    {
        object o;
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();
        using (var conn = new MySqlConnection(cs))
        {
            string command = "select valor from config where idconfig = " + idconfig.ToString();
            using (var cmd = new MySqlCommand(command, conn))
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter(cmd);
                conn.Open();
                o = cmd.ExecuteScalar();
            }
        }
        return (o);
    }

    private int ExecuteSQL(string sql)
    {
        int res = 0;
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        using (var conn = new MySqlConnection(cs))
        {
            string command = sql;
            using (var cmd = new MySqlCommand(command, conn))
            {
                conn.Open();
                res = cmd.ExecuteNonQuery();
            }
        }
        return (res);
    }

    private DataTable SelectSQL(string sql)
    {
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        DataSet ds = new DataSet();
        using (var conn = new MySqlConnection(cs))
        {
            string command = sql;
            using (var cmd = new MySqlCommand(command, conn))
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter(cmd);
                conn.Open();
                adapt.Fill(ds, "sql");
            }
        }
        return (ds.Tables["sql"]);
    }

    private object SelectCountSQL(string sql)
    {
        object res;
        string cs = ConfigurationManager.ConnectionStrings["ac"].ConnectionString;
        using (var conn = new MySqlConnection(cs))
        {
            string command = sql;
            using (var cmd = new MySqlCommand(command, conn))
            {
                conn.Open();
                res = cmd.ExecuteScalar();
            }
        }
        return (res);
    }
    #endregion

    #region Aux

    private void EnviaMail(string email, char tipo, string liga, Usuario u = null, byte[] reporte = null, Recarga r = null, catPaquete cp = null, string msg = "")
    {
        string FROM = GetConfig(4).ToString();
        string[] stremailsTO = email.Split(';');

        string TO = email; 
		string SUBJECT = "";
		string readText = "";
        if (tipo == 'F'){ // Referidos
            SUBJECT = "Ganaste credito gratis!";
			readText = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emails/referidos.html"));
            readText = readText.Replace("@@nombre@@", u.name);
            readText = readText.Replace("@@msg@@", msg);
		}
        if (tipo == 'E') //error al recargar, se manda a soporte
        {
            //SUBJECT = "Error al recargar de un usuario";
            //readText = "Hubo un error al hacer una recarga, los datos son:\n\n";
            //readText += "<b>Usuario: </b>" + u.name + " (" + u.email + ")\n";
            //readText += "<b>Tel personal: </b>" + u.numerocontacto + "\n";
            //readText += "<b>Recarga: </b>" + r.numerorecarga + " - " + cp.monto.ToString("c") + " - (" + cp.paquete + ")";
            SUBJECT = "Hubo un error en su recarga";
            readText = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emails/errrecarga.html"));
            readText = readText.Replace("@@nombre@@", u.name);
        }
        if (tipo == 'L') {
            SUBJECT = "Alta Nueva AsiCompras";
            readText = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emails/altasUser.html"));
            readText = readText.Replace("@@nombre@@", u.name);
            readText = readText.Replace("@@correo@@", u.email);
            readText = readText.Replace("@@fecha@@", u.fechaalta.ToString("dd/MMM/yyyy HH:mm:ss"));
        }
        if (tipo == 'B'){ //bienvenida
	        SUBJECT = "¡Bienvenido a AsiCompras!";
	        readText = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emails/bienvenida.html"));
			readText = readText.Replace("@@USUARIO@@", u.email);
            readText = readText.Replace("@@NIP@@", u.nip);
          //  readText = readText.Replace("@@VERIF@@", "https://asicompras.com/index.aspx?verifica="+u.verificacion);
        }
		if (tipo == 'N'){ //recordar nip
            SUBJECT = "Recordatorio de NIP en AsiCompras";
            readText = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emails/recuperacionclave.html"));
            readText = readText.Replace("@@USUARIO@@", u.email);
            readText = readText.Replace("@@NIP@@", u.nip);
        }
        if (tipo == 'Q') //bloqueado
        { 
            SUBJECT = "Bloqueo de NIP en AsiCompras";
            readText = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emails/recorda.html"));
            readText = readText.Replace("@@LINK@@", liga);
        }
        if (tipo == 'R') //Enviar reporte conciliacion
        {
            SUBJECT = "Reporte de conciliacion del dia " + DateTime.Now.Date.ToString("dd/MMM/yyyy");
            readText = "Anexo esta el Reporte de conciliacion del dia " + DateTime.Now.Date.ToString("dd/MMM/yyyy") + "</br></br>¡Saludos!";
        }
        if (tipo == 'U') //Enviar reporte usuarios
        {
            SUBJECT = "Reporte de usuarios del dia " + DateTime.Now.Date.ToString("dd/MMM/yyyy");
            readText = "Anexo esta el Reporte de usuarios del dia " + DateTime.Now.Date.ToString("dd/MMM/yyyy") + "</br></br>¡Saludos!";
        }

        String BODY = readText;
        const String SMTP_USERNAME = "AKIAJK63XZEQQPRAVU3Q";
        const String SMTP_PASSWORD = "At+XeAQc5kVQWclfzF7cvyaoxs7Q7ZMh05lNaaURRlSy";
        // Amazon SES SMTP host name. This example uses the US West (Oregon) region.
        const String HOST = "email-smtp.us-west-2.amazonaws.com";
        // The port you will connect to on the Amazon SES SMTP endpoint. We are choosing port 587 because we will use
        // STARTTLS to encrypt the connection.
        const int PORT = 587;

        MailMessage message = new MailMessage();
        message.From = new MailAddress(FROM);
        if (tipo == 'E')
        {
            message.To.Add(u.email);
            foreach (string s in stremailsTO)
                message.Bcc.Add(s);
        }
        else
        {
            foreach (string s in stremailsTO)
                message.To.Add(s);
        }
        message.Subject = SUBJECT; //"Request from " + SessionFactory.CurrentCompany.CompanyName + " to add a new supplier";
        message.IsBodyHtml = true;
        message.Body = BODY;
        if (tipo == 'R') //Enviar reporte conciliacion
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(reporte);
            //ms.Write(reporte, 0, (int)reporte.Length);
            System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType("application/vnd.ms-excel"); //System.Net.Mime.MediaTypeNames.Application.Octet);
            System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(ms, ct);
            attach.ContentDisposition.FileName = "RepConciliacion" + DateTime.Now.Date.ToString("ddMMMyyyy") + ".xls";
            message.Attachments.Add(attach);
        }
        if (tipo == 'U') //Enviar reporte usuarios
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(reporte);
            System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType("application/vnd.ms-excel"); //System.Net.Mime.MediaTypeNames.Application.Octet);
            System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(ms, ct);
            attach.ContentDisposition.FileName = "RepUsuarios" + DateTime.Now.Date.ToString("ddMMMyyyy") + ".xls";
            message.Attachments.Add(attach);
        }

        using (SmtpClient client = new SmtpClient(HOST, PORT))
        {
            client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
            client.EnableSsl = true;
            try
            {
                Console.WriteLine("Attempting to send an email through the Amazon SES SMTP interface...");
                client.Send(message);
                Console.WriteLine("Email sent!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("The email was not sent.");
                Console.WriteLine("Error message: " + ex.Message);
            }
        }

    }
    #endregion

}

#region Clases Aux
public sealed class StringWriterWithEncoding : StringWriter
{
    private readonly Encoding encoding;

    public StringWriterWithEncoding(Encoding encoding)
    {
        this.encoding = encoding;
    }

    public override Encoding Encoding
    {
        get { return encoding; }
    }
}

public class repconc
{
    public DataTable recargas { get; set; }
    public DataTable vesta { get; set; }
    public DataTable rs { get; set; }
}

public class ChargeAccountToTemporaryToken
    {
        public string ChargeAccountNumberToken { get; set; }
        public string PaymentDeviceLast4 { get; set; }
        public int PaymentDeviceTypeCD { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseText { get; set; }
    }

public class GetSessionTags
    {
        public string OrgID { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public string WebSessionID { get; set; }
        public int idrecarga { get; set; }
    }

public class AdditionalAmountsType
{
    public decimal Amount { get; set; }
    public string Type { get; set; }
}

public class ReversePayment
{
    public decimal AvailableRefundAmount { get; set; }
    public string PartnerCustomerKey { get; set; }
    public string PaymentAcquirerName { get; set; }
    public string PaymentID { get; set; }
    public int PaymentStatus { get; set; }
    public string ProxiedMessageName { get; set; }
    public int ResponseCode { get; set; }
    public string ResponseText { get; set; }
    public int ReversalAction { get; set; }
}

public class ChargeSale
{
    public AdditionalAmountsType AdditionalAmounts { get; set; }
    public string AcquirerApprovalCode { get; set; }
    public string AcquirerAVSResponseCode { get; set; }
    public string AcquirerCVNResponseCode { get; set; }
    public string AcquirerResponseCode { get; set; }
    public string AcquirerResponseCodeText { get; set; }
    public decimal AuthorizedAmount { get; set; }
    public int AuthResultCode { get; set; }
    public int AVSResultCode { get; set; }
    public int ChargeAccountFirst6 { get; set; }
    public int ChargeAccountLast4 { get; set; }
    public string ChargePermanentToken { get; set; }
    public int CVNResultCode { get; set; }
    public string PartnerCustomerKey { get; set; }
    public int PartnerDenialReasonCode { get; set; }
    public string PaymentAcquirerName { get; set; }
    public int PaymentDeviceTypeCD { get; set; }
    public string PaymentID { get; set; }
    public int PaymentStatus { get; set; }
    public string ProxiedMessageName { get; set; }
    public int ResponseCode { get; set; }
    public string ResponseText { get; set; }
}

public class product
{
    public string name { get; set; }
    public string sku { get; set; }
    public string monto { get; set; }
    public string info { get; set; }
    public string opid { get; set; }
    public string stype { get; set; }
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
    public int idtarjeta { get; set; }
    public string PaymentID { get; set; }
    public int err { get; set; }
    public int errVs { get; set; }
    public int errRs { get; set; }
    public tresp tresp { get; set; }
}
#endregion