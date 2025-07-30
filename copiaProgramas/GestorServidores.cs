using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace copiaProgramas
{
    public class GestorServidores
    {
        private List<ConfiguracionServidor> servidores = new List<ConfiguracionServidor>();

        public GestorServidores()
        {
            // Geco72
            servidores.Add(new ConfiguracionServidor(
                nombre: "geco72",
                protocolo: WinSCP.Protocol.Sftp,
                hostName: "172.31.5.149",
                userName: "centos",
                hostKey: "ssh-ed25519 255 ypCFfhJskB3YSCzQzF5iHV0eaWxlBIvMeM5kRl4N46o=",
                privateKey: @"C:\Oficina_ds\Diagram\Accesos portatil\conexiones VPN\Credenciales SSH\aws_diagram_irlanda.ppk"
            ));

            // Geco04
            servidores.Add(new ConfiguracionServidor(
                nombre: "geco04",
                protocolo: WinSCP.Protocol.Sftp,
                hostName: "172.31.26.21",
                userName: "centos",
                hostKey: "ssh-ed25519 255 EED2o6CV3I8GXE2qqXPEopvallRrpWb8MY2hqmJshGM=",
                privateKey: @"C:\Oficina_ds\Diagram\Accesos portatil\conexiones VPN\Credenciales SSH\aws_diagram_irlanda.ppk"
            ));
        }

        public ConfiguracionServidor ObtenerConfiguracion(string nombre)
        {
            return servidores.FirstOrDefault(s => s.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }
    }
}
