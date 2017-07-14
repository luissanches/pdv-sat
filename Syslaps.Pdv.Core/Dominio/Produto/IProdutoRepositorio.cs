using System;
using System.Collections.Generic;
using Syslaps.Pdv.Core.Dominio.Base;

namespace Syslaps.Pdv.Core.Dominio.Produto
{
    public interface IProdutoRepositorio : IRepositorioBase
    {
        List<Entity.Produto> RecuperarListaDeProdutosDoPdv();
        List<Entity.Produto> RecuperarListaDeProdutosComProducao();
        Entity.Produto RecuperarProdutoPorCodigoDeBarras(string codigoDeBarra);
    }
}