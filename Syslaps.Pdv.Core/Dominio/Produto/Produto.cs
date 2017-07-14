using System;
using System.Collections.Generic;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Entity;

namespace Syslaps.Pdv.Core.Dominio.Produto
{
    public class Produto : ModeloBase
    {
        private readonly IProdutoRepositorio repositorio;

        public Produto(IProdutoRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public List<Entity.Produto> RecuperarListaDeProdutosDoPdvPoTipo(int tpProduto)
        {
            if(tpProduto == 2)
            {
                var listaDeProdutos = repositorio.RecuperarListaDeProdutosDoPdv();
                listaDeProdutos.ForEach((item) =>
                {
                    item.PrecoVenda = item.PrecoVenda2;
                });

                return listaDeProdutos;
            }

            return repositorio.RecuperarListaDeProdutosDoPdv();
        }

        public List<Entity.Produto> RecuperarListaDeProdutosDoPdv()
        {
            return repositorio.RecuperarListaDeProdutosDoPdv();
        }

        public List<Entity.Produto> RecuperarListaDeProdutosComProducao()
        {
            return repositorio.RecuperarListaDeProdutosComProducao();
        }
    }
}
