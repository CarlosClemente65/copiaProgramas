using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace copiaProgramas
{
    public  class ConfiguracionServidor
    {
        public string Nombre { get; set; } // Ejemplo: "geco72", "geco04"
        public WinSCP.Protocol Protocolo { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string HostKey { get; set; }
        public string PrivateKey { get; set; }

        public ConfiguracionServidor(string nombre, WinSCP.Protocol protocolo, string hostName, string userName, string hostKey, string privateKey)
        {
            Nombre = nombre;
            Protocolo = protocolo;
            HostName = hostName;
            UserName = userName;
            HostKey = hostKey;
            PrivateKey = privateKey;
        }
    }
}
