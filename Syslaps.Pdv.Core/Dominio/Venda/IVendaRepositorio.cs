using System;
using System.Collections.Generic;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Entity.Especializadas;

namespace Syslaps.Pdv.Core.Dominio.Venda
{
    public interface IVendaRepositorio : IRepositorioBase
    {
        List<Entity.Venda> RecuperarListaDasVendasDaOperacaoDeAbertura(string codigoDaOperacaoDeAbertura);
        List<Entity.VendaPagamento> RecuperarListaDosPagamentosDaVenda(string codigoDaVenda);
        TotalVendido RecuperarTotalDeVendaDoPeriodo(DateTime dataInicial, DateTime dataFinal);
        List<Entity.Venda> RecuperarVendasDoPeriodo(DateTime dataInicial, DateTime dataFinal);
    }
}