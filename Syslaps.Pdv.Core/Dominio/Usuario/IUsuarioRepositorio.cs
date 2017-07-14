using Syslaps.Pdv.Core.Dominio.Base;

namespace Syslaps.Pdv.Core.Dominio.Usuario
{
    public interface IUsuarioRepositorio : IRepositorioBase
    {
        Entity.Usuario RecuperarUsuario(string nome, string senha);
    }
}