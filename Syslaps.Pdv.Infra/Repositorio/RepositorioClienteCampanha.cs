using System.Linq;
using Dapper;
using Syslaps.Pdv.Core.Dominio.Cliente;
using Syslaps.Pdv.Entity;

namespace Syslaps.Pdv.Infra.Repositorio
{
    public class RepositorioClienteCampanha : RepositorioBase, IClienteCampanhaRepositorio
    {
        public Entity.ClienteCampanha RecuperarcClienteNaCampanha(Entity.ClienteCampanha clienteCampanha)
        {
            return Db.Query<Entity.ClienteCampanha>("select * from ClienteCampanha Where CpfCnpj = @CpfCnpj and NomeCampanha = @NomeCampanha", new Entity.ClienteCampanha { CpfCnpj = clienteCampanha.CpfCnpj, NomeCampanha = clienteCampanha.NomeCampanha}).FirstOrDefault();
        }
    }
}