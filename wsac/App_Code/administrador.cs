using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for administrador
/// </summary>

namespace MasTicket
{
    public class administrador
    {
        public administrador()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public int idadministrador { get; set; }
        public string email { get; set;}
        public string name { get; set; }
        public string firs_name { get; set; }
        public string last_name { get; set; }
        public DateTime fechaalta { get; set; }
        public string nip { get; set; }
        public string numerocontacto { get; set; }
    }
}