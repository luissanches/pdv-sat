using System;
using System.Linq;
using System.Net;

namespace Syslaps.Pdv.Cross
{
    public class Utils
    {
        public string RecuperarIp()
        {
            var firstOrDefault = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            return firstOrDefault?.ToString() ?? string.Empty;
        }

        public string GerarCodigoUnico()
        {
            return Guid.NewGuid().ToString("N");
        }

    }
}
