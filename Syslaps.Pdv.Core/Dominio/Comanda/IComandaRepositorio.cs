using System.Collections.Generic;
using Syslaps.Pdv.Core.Dominio.Base;

namespace Syslaps.Pdv.Core.Dominio.Comanda
{
    public interface IComandaRepositorio : IRepositorioBase
    {
        List<Entity.Comanda> RecuperarListaDeComandasAbertas();

        void ExcluirPorCodigoDaComanda(string codigoDaComanda);

        Entity.Comanda RecuperarComanda(string codigoDaComanda);
    }
}
