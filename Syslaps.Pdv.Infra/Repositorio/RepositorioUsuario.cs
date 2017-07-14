using Dapper;
using Syslaps.Pdv.Core.Dominio.Usuario;
using Usuario = Syslaps.Pdv.Entity.Usuario;

namespace Syslaps.Pdv.Infra.Repositorio
{
    public sealed class RepositorioUsuario : RepositorioBase, IUsuarioRepositorio
    {

        public Usuario RecuperarUsuario(string nome, string senha)
        {
            return Db.QuerySingleOrDefault<Usuario>("select * from Usuario Where Nome = @Nome and Senha = @Senha", new  { Nome = nome, Senha = senha });
        }

        
    }
}
