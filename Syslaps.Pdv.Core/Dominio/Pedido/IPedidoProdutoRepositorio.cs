using System;
using System.Collections.Generic;
using Syslaps.Pdv.Core.Dominio.Base;

namespace Syslaps.Pdv.Core.Dominio.Pedido
{
    public interface IPedidoProdutoRepositorio : IRepositorioBase
    {
        void ExcluirProdutosDoPedido(Entity.Pedido pedido);
    }
}