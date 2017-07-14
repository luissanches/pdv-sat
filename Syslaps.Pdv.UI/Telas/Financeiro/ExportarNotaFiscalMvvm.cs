using Syslaps.Pdv.Core.Dominio.SAT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Syslaps.Pdv.UI.Telas.Financeiro
{
    class ExportarNotaFiscalMvvm : INotifyPropertyChanged
    {
        public Action<string> ProcessoInciadoHandler;
        public Action ProcessoFinalizadoHandler;
        public event PropertyChangedEventHandler PropertyChanged;
        private CupomSat _cupomSat;


        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<Entity.CupomFiscalSat> ListaDeCupomSat { get; set; }


        public ExportarNotaFiscalMvvm(CupomSat cupomSat)
        {
            _cupomSat = cupomSat;
            DataInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DataFinal = DataInicial.AddMonths(1).AddDays(-1);
            ListaDeCupomSat = new List<Entity.CupomFiscalSat>();
        }

        public void ConsultarCuponsPorData()
        {
            ProcessoInciadoHandler?.Invoke("Aguarde...");
            ListaDeCupomSat = _cupomSat.RecuperarCuponsDoPeriodo(DataInicial, DataFinal);
            ProcessoFinalizadoHandler?.Invoke();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
