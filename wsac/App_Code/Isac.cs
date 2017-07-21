using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

[ServiceContract]
[XmlSerializerFormat]
public interface Isac
{
    /*[OperationContract]
    Task DEV();
    [OperationContract]
    Task Encry();*/

    [OperationContract]
    string GetUser(int idusr, string email, string nip);

    [OperationContract]
    string GetAdmin(int idusr, string email, string nip);

    [OperationContract]
    string GetCatalogo(int idcatalogo, string where);

    [OperationContract]
    decimal GetMontoMax();

    [OperationContract]
    int GetMonederoHab();

    [OperationContract]
    void EnviaMailRecordatorio(int idusuario);

    [OperationContract]
    Task<int> AltaRecargaProg(MasTicket.RecargaProg r);

    [OperationContract]
    Task<string> AltaRecarga(MasTicket.Recarga r);

    [OperationContract]
    Task<string> AltaRecargaMonedero(MasTicket.RecargaMonedero r);

    [OperationContract]
    Task<string> AltaRecargaViaWallet(MasTicket.Recarga r);

    [OperationContract]
    int AltaUsr(MasTicket.Usuario u);

    [OperationContract]
    int Mod_Usuario(MasTicket.Usuario u);

    [OperationContract]
    int Mod_administrador(MasTicket.administrador u);

    [OperationContract]
    bool Delete_administrador(int idadministrador);

    [OperationContract]
    int AltaAdmin(MasTicket.administrador a);

    //[OperationContract]
    //int AltaTarjeta(MasTicket.Tarjeta t);

    [OperationContract]
    bool EliminaTarjeta(int idtarjeta);

    [OperationContract]
    bool EliminaRecProg(int idrecprog);

    [OperationContract]
    string GetCatalogoPersonalizado(string consulta);

    //[OperationContract]
    //Task<string> CargaVesta(MasTicket.Recarga r, MasTicket.RecargaMonedero rm, string WebSessionID);

    [OperationContract]
    //Task<string> CargaVesta1aVez(MasTicket.Recarga r, MasTicket.RecargaMonedero rm, MasTicket.Tarjeta t);
    Task<string> CargaVesta1aVez(MasTicket.Recarga r, MasTicket.RecargaMonedero rm, string WebSessionID, MasTicket.Tarjeta t, string cvv);

    [OperationContract]
    Task<string> CargaVesta2aVez(MasTicket.Recarga r, MasTicket.RecargaMonedero rm, string WebSessionID);

    //[OperationContract]
    //MasTicket.Tarjeta LlenaTarjeta(int idtarjeta);

    [OperationContract]
    string GetReporteConciliacion(string fini, string ffin);

    [OperationContract]
    byte[] ExportaRepConc(string fini, string ffin);

    [OperationContract]
    void EnviaReporteConciliacion(string fini, string ffin);

    [OperationContract]
    void EnviaReporteUsuarios();

    [OperationContract]
    string GetUsers();


}
