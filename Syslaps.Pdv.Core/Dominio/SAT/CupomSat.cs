using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syslaps.Pdv.Core.Dominio.SAT
{
    public class CupomSat
    {
        private readonly ICupomSatRepositorio _repositorio;


        public CupomSat(ICupomSatRepositorio repositorio)
        {
            this._repositorio = repositorio;
        }

        public List<Entity.CupomFiscalSat> RecuperarCuponsDoPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            var dtStart = new DateTime(dataInicial.Year, dataInicial.Month, dataInicial.Day, 0, 0, 0);
            var dtEnd = new DateTime(dataFinal.Year, dataFinal.Month, dataFinal.Day, 23, 59, 59);
            return _repositorio.RecuperarCuponsDoPeriodo(dtStart, dtEnd);
        }
    }
}
