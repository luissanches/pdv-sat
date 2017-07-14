using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapper.FastCrud;
using Syslaps.Pdv.Core.Dominio.Caixa;
using Syslaps.Pdv.Entity;
using Caixa = Syslaps.Pdv.Entity.Caixa;

namespace Syslaps.Pdv.Infra.Repositorio
{
    public sealed class RepositorioCaixa : RepositorioBase, ICaixaRepositorio
    {
        public Caixa RecuperarCaixaPorNome(string nomeDoCaixa)
        {
            return Db.QuerySingleOrDefault<Caixa>("select * from Caixa where Nome = @Nome", new Entity.Caixa() { Nome = nomeDoCaixa });
        }

        public OperacaoCaixa RecuperarOperacaoCaixa(string codigo)
        {
            return Db.Get(new OperacaoCaixa {CodigoOperacaoCaixa = codigo});
        }

        public List<OperacaoCaixa> RecuperarOperacoesCaixaPorCodigoDeAbertura(string codigoDaOperacaoDeAbertura)
        {
            return Db.Query<OperacaoCaixa>("select * from OperacaoCaixa where CodigoOperacaoCaixaAbertura = @CodigoOperacaoCaixaAbertura or CodigoOperacaoCaixa = @CodigoOperacaoCaixa", new OperacaoCaixa { CodigoOperacaoCaixaAbertura = codigoDaOperacaoDeAbertura, CodigoOperacaoCaixa = codigoDaOperacaoDeAbertura }).ToList();
        }

    }
}