using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Syslaps.Pdv.Core.Dominio.Pedido;
using Syslaps.Pdv.Core.Dominio.Produto;
using Syslaps.Pdv.Entity;

namespace Syslaps.Pdv.Infra.Repositorio
{
    public class RepositorioPedido : RepositorioBase, IPedidoRepositorio
    {
        private IProdutoRepositorio _produtoRepositorio;

        public RepositorioPedido(IProdutoRepositorio produtoRepositorio)
        {
            _produtoRepositorio = produtoRepositorio;
        }

        public List<Entity.Pedido> RecuperarPedidosPorDataDaEntrega(DateTime dataInicio, DateTime dataFim)
        {

            var list = Db.Query<Entity.Pedido>("select * from Pedido where DataEntrega between @DataPedidoInicio and @DataPedidoFim",
                new { DataPedidoInicio = dataInicio, DataPedidoFim = dataFim });

            return list.Select(c => new Entity.Pedido
            {
                CodigoPedido   = c.CodigoPedido,
                NomeCliente = c.NomeCliente,
                DataEntrega = c.DataEntrega,
                DataPedido = c.DataPedido,
                Situacao = c.Situacao,
                Observacao = c.Observacao,
                Telefone = c.Telefone,
                Valor = c.Valor,
                PedidoProduto = RecuperarListaDeProdutosDoPedido(c.CodigoPedido)
            }).ToList();
        }

        public List<PedidoProduto> RecuperarListaDeProdutosDoPedido(string codigoPedido)
        {
            var list = Db.Query<PedidoProduto>(
                    "select * from pedidoproduto where Pedido_CodigoPedido = @CodigoPedido",
                    new { CodigoPedido = codigoPedido });

            return list.Select(c=>new PedidoProduto
            {
                CodigoPedidoProduto = c.CodigoPedidoProduto,
                Quantidade = c.Quantidade,
                Pedido_CodigoPedido = c.Pedido_CodigoPedido,
                Produto_CodigoDeBarra = c.Produto_CodigoDeBarra,
                Produto = _produtoRepositorio.RecuperarProdutoPorCodigoDeBarras(c.Produto_CodigoDeBarra)
            }).ToList();
        }
    }
}
