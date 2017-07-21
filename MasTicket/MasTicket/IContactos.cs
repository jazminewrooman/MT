using System;
using System.Collections.Generic;
using System.Text;

namespace MasTicket
{
    public interface IContactos
    {
        List<Contacto> GetLista();
		string GetIP();
		string GetOS();
    }

    public class Contacto
    {
        public Contacto() { }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string PhotoUri { get; set; }
        public byte[] Photo { get; set; }
        public string Fecha { get; set; }
        public string Carrier { get; set; }
        public int idoperadora { get; set; }
    }
}
