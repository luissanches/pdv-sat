using Syslaps.Pdv.Core.Dominio.Base;
using System;
using System.Collections.Generic;

namespace Syslaps.Pdv.Core.Dominio.SAT
{
    public interface ICupomSatRepositorio : IRepositorioBase
    {
        List<Entity.CupomFiscalSat> RecuperarCuponsDoPeriodo(DateTime dataInicial, DateTime dataFinal);
    }
}
