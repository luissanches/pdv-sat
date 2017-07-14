using Dapper;
using Syslaps.Pdv.Core.Dominio.SAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Syslaps.Pdv.Infra.Repositorio
{
    public class RepositorioCupomSat : RepositorioBase, ICupomSatRepositorio
    {
        public List<Entity.CupomFiscalSat> RecuperarCuponsDoPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            return Db.Query<Entity.CupomFiscalSat>("select * from CupomFiscalSat where DataOperacao between @DataInicial and @DataFinal", new { DataInicial = dataInicial, DataFinal = dataFinal }).ToList();
        }

    }
}