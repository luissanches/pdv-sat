using System.Collections.Generic;
using Syslaps.Pdv.Core.Dominio.Base;

namespace Syslaps.Pdv.Core.Dominio.Caixa
{
    public interface ICaixaRepositorio : IRepositorioBase
    {
        Entity.Caixa RecuperarCaixaPorNome(string nomeDoCaixa);

        Entity.OperacaoCaixa RecuperarOperacaoCaixa(string codigo);

        List<Entity.OperacaoCaixa> RecuperarOperacoesCaixaPorCodigoDeAbertura(string codigoDaOperacaoDeAbertura);
    }
}