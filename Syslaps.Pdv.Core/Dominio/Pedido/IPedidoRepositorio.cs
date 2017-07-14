using System;
using System.Collections.Generic;
using Syslaps.Pdv.Core.Dominio.Base;

namespace Syslaps.Pdv.Core.Dominio.Pedido
{
    public interface IPedidoRepositorio: IRepositorioBase
    {
        List<Entity.Pedido> RecuperarPedidosPorDataDaEntrega(DateTime dataInicio, DateTime dataFim);
    }
}