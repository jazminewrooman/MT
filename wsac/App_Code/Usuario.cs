using System;
using System.Collections.Generic;
using System.Text;

namespace MasTicket
{
    public class Usuario
    {
        public Usuario()
        {

        }

        public int idusuario { get; set; }
        public string numerocontacto { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string gender { get; set; }
        public string picture { get; set; }
        public bool registrado { get; set; }
        public int idpais { get; set; }
        public string nip { get; set; }
        public DateTime fechaalta { get; set; }
    }
}
