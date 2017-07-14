using System;
using System.Collections.Generic;
using System.Linq;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Cross;
using Syslaps.Pdv.Entity;

namespace Syslaps.Pdv.Core.Dominio.Pedido
{
    public class Pedido : ModeloBase
    {
        private readonly IPedidoRepositorio _pedidoRepositorio;
        private readonly IPedidoProdutoRepositorio _pedidoProdutoRepositorio;
        private readonly IInfraLogger _logger;
        private readonly Producao.Producao _producaoDominio;


        public Pedido(IPedidoRepositorio pedidoRepositorio, IPedidoProdutoRepositorio pedidoProdutoRepositorio, IInfraLogger logger, Producao.Producao producaoDominio)
        {
            this._pedidoRepositorio = pedidoRepositorio;
            _pedidoProdutoRepositorio = pedidoProdutoRepositorio;
            _logger = logger;
            _producaoDominio = producaoDominio;
        }

        public Entity.Pedido PedidoCorrente { get; set; }

        public void AdicionarProdutoNoPedido(PedidoProduto pedidoProduto)
        {
            var produtoNoPedido = PedidoCorrente.PedidoProduto.FirstOrDefault(
                x => x.Produto.CodigoDeBarra == pedidoProduto.Produto.CodigoDeBarra);

            if (produtoNoPedido != null)
            {
                produtoNoPedido.Quantidade += pedidoProduto.Quantidade;
                produtoNoPedido.IsVisible = true;
            }
            else
                PedidoCorrente.PedidoProduto.Add(pedidoProduto);
        }

        public void AtualizarQuantidadeDoProdutoDoPedido(PedidoProduto pedidoProduto)
        {
            var produtoNoPedido = PedidoCorrente.PedidoProduto.FirstOrDefault(
                x => x.Produto.CodigoDeBarra == pedidoProduto.Produto.CodigoDeBarra);

            if (produtoNoPedido != null)
            {
                produtoNoPedido.Quantidade = pedidoProduto.Quantidade;
            }
        }

        public void RemoverProdutoDoPedido(PedidoProduto pedidoProduto)
        {
            if (!pedidoProduto.CodigoPedidoProduto.IsNullOrEmpty())
            {
                pedidoProduto.IsVisible = false;
            }
            else
                PedidoCorrente.PedidoProduto.Remove(pedidoProduto);

        }

        public void RegistrarPedido()
        {
            try
            {
                var resultadoValidacao = PedidoCorrente.Validate();

                resultadoValidacao.ForEach(item =>
                {
                    AdicionarMensagem(item.ErrorMessage, EnumStatusDoResultado.RegraDeNegocioInvalida);
                });


                if (PedidoCorrente.Telefone.IsNullOrEmpty())
                {
                    AdicionarMensagem("Telefone Obrigatório.\r\n", EnumStatusDoResultado.RegraDeNegocioInvalida);
                }

                if (PedidoCorrente.Telefone.IsNullOrEmpty())
                {
                    AdicionarMensagem("Telefone Obrigatório.\r\n", EnumStatusDoResultado.RegraDeNegocioInvalida);
                }

                if (PedidoCorrente.DataEntrega<DateTime.Today)
                {
                    AdicionarMensagem("A data de Entrega do Pedido deve ser Maior que a Atual.\r\n", EnumStatusDoResultado.RegraDeNegocioInvalida);
                }

                if (Status != EnumStatusDoResultado.MensagemDeSucesso)
                {
                    return;
                }

                _pedidoProdutoRepositorio.ExcluirProdutosDoPedido(PedidoCorrente);
                _pedidoRepositorio.Excluir(PedidoCorrente);

                PedidoCorrente.CodigoPedido = GerarCodigoUnico();
                _pedidoRepositorio.Inserir(PedidoCorrente);
                PedidoCorrente.PedidoProduto.ToList().ForEach(item =>
                {
                    if (item.IsVisible)
                    {
                        item.CodigoPedidoProduto = GerarCodigoUnico();
                        item.Pedido_CodigoPedido = PedidoCorrente.CodigoPedido;
                        _pedidoProdutoRepositorio.Inserir(item);
                    }
                });

                AtualizarProducao();

                AdicionarMensagem("Produtos Importados com Sucesso");
            }
            catch (Exception ex)
            {
                _logger.Log().Error(ex);
            }

        }

        public void ExcluirPedido(Entity.Pedido pedido)
        {
            pedido.PedidoProduto.ToList().ForEach(produtoPedido =>
            {
                var produtoProducao = _producaoDominio.RecuperarProducaoDoDiaDeUmProduto(produtoPedido.Produto_CodigoDeBarra,
                    pedido.DataEntrega);

                if (produtoProducao != null)
                {
                    if (produtoProducao.QuantidadeProduzida >= produtoPedido.Quantidade)
                    {
                        produtoProducao.QuantidadeProduzida -= produtoPedido.Quantidade;
                    }
                    else
                    {
                        produtoProducao.QuantidadeProduzida = 0;
                    }


                    _producaoDominio.CriarAlterarProducaoDeProduto(produtoProducao.Produto_CodigoDeBarra,
                            pedido.DataEntrega,
                            produtoProducao.QuantidadeProduzida.ToInt(),
                            produtoProducao.QuantidadeDescartadaInteira.ToInt(),
                            produtoProducao.QuantidadeDescartadaParcial.ToInt());
                   
                }
                else
                {
                    _producaoDominio.CriarAlterarProducaoDeProduto(produtoPedido.Produto_CodigoDeBarra, pedido.DataEntrega,
                        produtoPedido.Quantidade.ToInt(), 0, 0);
                }

            });

            _pedidoProdutoRepositorio.ExcluirProdutosDoPedido(pedido);
            _pedidoRepositorio.Excluir(pedido);
        }

        private void AtualizarProducao()
        {
            PedidoCorrente.PedidoProduto.ToList().ForEach(produtoPedido =>
            {
                var produtoProducao = _producaoDominio.RecuperarProducaoDoDiaDeUmProduto(produtoPedido.Produto_CodigoDeBarra,
                    PedidoCorrente.DataEntrega);

                if (produtoProducao != null)
                {
                    if(!produtoPedido.IsVisible)
                        produtoProducao.QuantidadeProduzida -= produtoPedido.Quantidade;
                    else
                        produtoProducao.QuantidadeProduzida += produtoPedido.Quantidade;

                    _producaoDominio.CriarAlterarProducaoDeProduto(produtoProducao.Produto_CodigoDeBarra, PedidoCorrente.DataEntrega,
                        produtoProducao.QuantidadeProduzida.ToInt(), produtoProducao.QuantidadeDescartadaInteira.ToInt(),
                        produtoProducao.QuantidadeDescartadaParcial.ToInt());
                }
                else
                {
                    _producaoDominio.CriarAlterarProducaoDeProduto(produtoPedido.Produto_CodigoDeBarra, PedidoCorrente.DataEntrega,
                        produtoPedido.Quantidade.ToInt(), 0,0);
                }

            });
        }

        public List<Entity.Pedido> RecuperarListaDePedidosPorDataDeEntrega(DateTime dataDaEntrega)
        {
            var dataInicio = new DateTime(dataDaEntrega.Year, dataDaEntrega.Month, dataDaEntrega.Day);
            var dataFim = new DateTime(dataDaEntrega.Year, dataDaEntrega.Month, dataDaEntrega.Day, 23, 59, 59);
            return _pedidoRepositorio.RecuperarPedidosPorDataDaEntrega(dataInicio, dataFim);
        }
    }
}
