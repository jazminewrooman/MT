using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

/// <summary>
/// Descripción breve de wscatalogos
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
// [System.Web.Script.Services.ScriptService]
public class wscatalogos : System.Web.Services.WebService
{
    private sac s;

    public wscatalogos()
    {
        s = new sac();

    }

    [WebMethod]
    public decimal GetMontoMax()
    {
        return (s.GetMontoMax());
    }

    [WebMethod]
    public int GetMonederoHab()
    {
        return (s.GetMonederoHab());
    }

    [WebMethod]
    public string GetAdmin(int idusr, string email, string nip)
    {
        return (s.GetAdmin(idusr, email, nip));
    }

    [WebMethod]
    public string GetUser(int idusr, string email, string nip)
    {
        return (s.GetUser(idusr, email, nip));
    }

    [WebMethod]
    public string GetCatalogo(int idcatalogo, string where)
    {
        return (s.GetCatalogo(idcatalogo, where));
    }

    [WebMethod]
    public string GetCodigoReferidoUsr(int idusr)
    {
        return (s.GetCodigoReferidoUsr(idusr));
    }

    [WebMethod]
    public int ExisteCodigoReferidoUsr(string codigo)
    {
        return (s.ExisteCodigoReferidoUsr(codigo));
    }

    [WebMethod]
    public string GetMsgReferidosPUsr(int idusr)
    {
        return s.GetMsgReferidosPUsr(idusr);
    }

	[WebMethod]
	public string GetMsgReferidosPRef(int idref)
	{
		return s.GetMsgReferidosPRef(idref);
	}


}