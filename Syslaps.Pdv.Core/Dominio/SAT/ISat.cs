using Syslaps.Pdv.Entity.SAT;

namespace Syslaps.Pdv.Core.Dominio.SAT
{
    public interface ISat
    {
        SendResponse Send(string xml);
        SatResponse CheckAvailability();
        GetStatusResponse GetStatus();
    }
}
