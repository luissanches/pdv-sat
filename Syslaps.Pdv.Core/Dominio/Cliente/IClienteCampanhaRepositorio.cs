using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Entity;

namespace Syslaps.Pdv.Core.Dominio.Cliente
{
    public interface IClienteCampanhaRepositorio: IRepositorioBase
    {
        Entity.ClienteCampanha RecuperarcClienteNaCampanha(Entity.ClienteCampanha clienteCampanha);
    }
}