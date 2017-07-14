using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Syslaps.Pdv.Core.Dominio.Pedido;
using Syslaps.Pdv.Core.Dominio.Produto;
using Syslaps.Pdv.Entity;
using Pedido = Syslaps.Pdv.Entity.Pedido;

namespace Syslaps.Pdv.Infra.Repositorio
{
    public class RepositorioPedidoProduto : RepositorioBase, IPedidoProdutoRepositorio
    {
        public void ExcluirProdutosDoPedido(Pedido pedido)
        {
            Db.Execute("delete from pedidoproduto where pedido_codigopedido = @CodigoPedido", new { CodigoPedido  = pedido.CodigoPedido});
        }
    }
}
