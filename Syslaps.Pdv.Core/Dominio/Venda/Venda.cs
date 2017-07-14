using System;
using System.Collections.Generic;
using System.Linq;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Core.Dominio.Impressora;
using Syslaps.Pdv.Entity;
using Syslaps.Pdv.Entity.Especializadas;

namespace Syslaps.Pdv.Core.Dominio.Venda
{
    public class Venda : ModeloBase
    {
        private readonly Caixa.Caixa _caixaAberto;
        private readonly IVendaRepositorio _repositorio;
        private readonly Cupom _cupom;
        private readonly Parametros _parametros;
        public Entity.Venda VendaCorrente { get; private set; }

        public Venda(Caixa.Caixa caixaAberto, IVendaRepositorio repositorio, Cupom cupom, Parametros parametros)
        {
            this._caixaAberto = caixaAberto;
            this._repositorio = repositorio;
            _cupom = cupom;
            _parametros = parametros;
            VendaCorrente = new Entity.Venda
            {
                CodigoVenda = GerarCodigoUnico(),
                DataVenda = DateTime.Now,
                OperacaoCaixa_CodigoOperacaoCaixa = caixaAberto.CaixaCorrente.CodigoOperacaoDeAbertura,
                Usuario_CodigoUsuario = this._caixaAberto.UsuarioCorrente.CodigoUsuario,
                DataRecebimento = DateTime.Now,
                ValorTotalRecebimento = 0,
                CFOP = 5101
            };
            //this._repositorio.Inserir(VendaCorrente);
        }

        public void IdentificarCliente(string nomeCliente, string cpfCnpj, TipoDocumento tipoDocumento = TipoDocumento.CPF)
        {
            VendaCorrente.NomeCliente = nomeCliente;
            VendaCorrente.CpfCnpjCliente = cpfCnpj;
            VendaCorrente.TipoDocumento = tipoDocumento.ToString();
        }

        public void AdicionarProdutoNaVenda(Entity.Produto produto, decimal quantidade, decimal valorUnitarioDaVenda)
        {
            var vendaProduto = new VendaProduto
            {
                Quantidade = quantidade,
                CodigoVendaProduto = GerarCodigoUnico(),
                Venda_CodigoVenda = VendaCorrente.CodigoVenda,
                ValorDoProdutoComDesconto = valorUnitarioDaVenda,
                ValorDoProduto = produto.PrecoVenda,
                Produto = produto,
                Produto_CodigoDeBarra = produto.CodigoDeBarra,
                CodigoParaCupom = produto.CodigoParaCupom,
                DescricaoProduto = produto.Descricao,
                Venda = VendaCorrente
            };


            var produtoNaVenda = VendaCorrente.VendaProdutoes.ToList().FirstOrDefault(
              x => x.Produto_CodigoDeBarra == produto.CodigoDeBarra);

            if (produtoNaVenda != null)
            {
                produtoNaVenda.Quantidade += quantidade;
                vendaProduto = produtoNaVenda;
            }
            else
            {
                VendaCorrente.VendaProdutoes.Add(vendaProduto);
            }

            vendaProduto.ValorTotalVendaProduto = vendaProduto.Quantidade * vendaProduto.ValorDoProdutoComDesconto;
            vendaProduto.ValorDoDesconto = vendaProduto.ValorDoProduto - vendaProduto.ValorDoProdutoComDesconto;
            vendaProduto.ValorTotalDoDesconto = vendaProduto.Quantidade * vendaProduto.ValorDoDesconto;
            VendaCorrente.ValorTotalVenda = VendaCorrente.VendaProdutoes.Sum(x => x.ValorTotalVendaProduto);
            VendaCorrente.ValorTotalDescontoVenda = VendaCorrente.VendaProdutoes.Sum(x => x.ValorTotalDoDesconto);
        }

        public void RemoverProdutoDaVenda(VendaProduto vendaProduto)
        {
            VendaCorrente.VendaProdutoes.Remove(vendaProduto);
            VendaCorrente.ValorTotalVenda = VendaCorrente.VendaProdutoes.Sum(x => x.ValorTotalVendaProduto);
            VendaCorrente.ValorTotalDescontoVenda = VendaCorrente.VendaProdutoes.Sum(x => x.ValorTotalDoDesconto);
        }

        public void AdicionarPagamento(Entity.TipoPagamento tipoPagamento, decimal valorDoPagamento)
        {
            if (VendaCorrente.ValorTotalVenda <= 0)
            {
                AdicionarMensagem("Venda com valor igual ou menor que zero.",
                    EnumStatusDoResultado.RegraDeNegocioInvalida);
            }
            else
            {
                var vendaPagamento = new VendaPagamento
                {
                    CodigoVendaPagamento = GerarCodigoUnico(),
                    TipoPagamento_CodigoTipoPagamento = tipoPagamento.CodigoTipoPagamento,
                    Venda_CodigoVenda = VendaCorrente.CodigoVenda,
                    TipoPagamento = tipoPagamento,
                    ValorPagamento = valorDoPagamento
                };

                VendaCorrente.VendaPagamentoes.Add(vendaPagamento);
                VendaCorrente.ValorTotalRecebimento += valorDoPagamento - (tipoPagamento.PercentualDesconto / 100 * valorDoPagamento);
                VendaCorrente.DataRecebimento = DateTime.Today.AddDays(tipoPagamento.DiasParaPagamento);

                if (VendaCorrente.VendaPagamentoes.Sum(x => x.ValorPagamento) - VendaCorrente.ValorTotalVenda < 0)
                    VendaCorrente.ValorTroco = 0;
                else
                {
                    VendaCorrente.ValorTroco = VendaCorrente.VendaPagamentoes.Sum(x => x.ValorPagamento) - VendaCorrente.ValorTotalVenda;
                    VendaCorrente.ValorTotalRecebimento = VendaCorrente.ValorTotalRecebimento - VendaCorrente.ValorTroco;
                }
            }
        }

        public void RemoverPagamentoDaVenda(VendaPagamento vendaPagamento)
        {
            VendaCorrente.VendaPagamentoes.Remove(vendaPagamento);
            var tipoPagamentoDoPagamento = _caixaAberto.TipoDoPagamento.ListaDeTiposDePagamento.FirstOrDefault(x => x.CodigoTipoPagamento == vendaPagamento.TipoPagamento_CodigoTipoPagamento);
            if (tipoPagamentoDoPagamento != null)
            {
                VendaCorrente.ValorTotalRecebimento -= vendaPagamento.ValorPagamento -
                                                       (tipoPagamentoDoPagamento.PercentualDesconto / 100 *
                                                        vendaPagamento.ValorPagamento);
                VendaCorrente.DataRecebimento = DateTime.Now;
                if (VendaCorrente.VendaPagamentoes.Sum(x => x.ValorPagamento) - VendaCorrente.ValorTotalVenda < 0)
                    VendaCorrente.ValorTroco = 0;
                else
                    VendaCorrente.ValorTroco = VendaCorrente.VendaPagamentoes.Sum(x => x.ValorPagamento) - VendaCorrente.ValorTotalVenda;
            }
        }

        public void ExcluirVenda()
        {
            VendaCorrente.VendaProdutoes.ToList().ForEach((item) =>
            {
                _repositorio.Excluir(item);
            });

            VendaCorrente.VendaPagamentoes.ToList().ForEach((item) =>
            {
                _repositorio.Excluir(item);
            });

            _repositorio.Excluir(VendaCorrente);
        }

        public void RegistrarVenda()
        {
            _repositorio.Inserir(VendaCorrente);   

            VendaCorrente.VendaProdutoes.ToList().ForEach((item) =>
            {
                _repositorio.Inserir(item);   
            });

            VendaCorrente.VendaPagamentoes.ToList().ForEach((item) =>
            {
                _repositorio.Inserir(item);
            });
        }

        public TotalVendido RecuperarTotalDeVendaDoPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            return _repositorio.RecuperarTotalDeVendaDoPeriodo(dataInicial, dataFinal);
        }

        public List<Entity.Venda> RecuperarVendasDoPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            return _repositorio.RecuperarVendasDoPeriodo(dataInicial, dataFinal);
        }

    }
}
