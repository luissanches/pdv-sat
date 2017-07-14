using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Syslaps.Pdv.Core.Dominio.Venda;
using Syslaps.Pdv.Entity;
using Syslaps.Pdv.Entity.Especializadas;
using Venda = Syslaps.Pdv.Entity.Venda;

namespace Syslaps.Pdv.Infra.Repositorio
{
    public class RepositorioVenda : RepositorioBase, IVendaRepositorio
    {
        public List<Venda> RecuperarListaDasVendasDaOperacaoDeAbertura(string codigoDaOperacaoDeAbertura)
        {
            return Db.Query<Venda>("select * from Venda Where OperacaoCaixa_CodigoOperacaoCaixa = @OperacaoCaixa_CodigoOperacaoCaixa", new { OperacaoCaixa_CodigoOperacaoCaixa = codigoDaOperacaoDeAbertura }).ToList();
        }

        public List<VendaPagamento> RecuperarListaDosPagamentosDaVenda(string codigoDaVenda)
        {
            return Db.Query<VendaPagamento>("select * from VendaPagamento Where Venda_CodigoVenda = @Venda_CodigoVenda", new { Venda_CodigoVenda = codigoDaVenda }).ToList();
        }

        public TotalVendido RecuperarTotalDeVendaDoPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            return Db.Query<TotalVendido>("select count(*) Quantidade, sum(valortotalvenda) ValorTotal from venda where datavenda between @DataInicial and @DataFinal", new { DataInicial = dataInicial, DataFinal = dataFinal }).SingleOrDefault();
        }

        public List<Venda> RecuperarVendasDoPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            var vendas = Db.Query<Venda>( "select * from Venda where datavenda between @DataInicial and @DataFinal order by datavenda",
                    new {DataInicial = dataInicial, DataFinal = dataFinal}).ToList();

            vendas.ForEach(item =>
            {
                item.VendaPagamentoes = Db.Query<VendaPagamento>("select * from vendapagamento where Venda_CodigoVenda = @Venda_CodigoVenda",
                new { Venda_CodigoVenda = item.CodigoVenda }).ToList();

                item.VendaPagamentoes.ToList().ForEach(pagamento =>
                {
                    pagamento.TipoPagamento =
                        Db.Query<Entity.TipoPagamento>(
                            "select * from tipopagamento where codigotipopagamento = @codigotipopagamento",
                            new {codigotipopagamento = pagamento.TipoPagamento_CodigoTipoPagamento}).SingleOrDefault();
                });


                item.VendaProdutoes =
                    Db.Query<VendaProduto>("select * from VendaProduto where Venda_CodigoVenda = @Venda_CodigoVenda",
                        new {Venda_CodigoVenda = item.CodigoVenda}).ToList();


            });

            return vendas;
        }
    }
}