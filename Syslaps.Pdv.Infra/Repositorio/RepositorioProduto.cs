using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Syslaps.Pdv.Core.Dominio.Produto;
using Produto = Syslaps.Pdv.Entity.Produto;

namespace Syslaps.Pdv.Infra.Repositorio
{
    public class RepositorioProduto : RepositorioBase, IProdutoRepositorio
    {
        public List<Produto> RecuperarListaDeProdutosDoPdv()
        {
            return Db.Query<Produto>(
                "select * from produto where Ativo = @Ativo and ExibirNoPdv = @ExibirNoPdv Order By Descricao", new { Ativo = true, ExibirNoPdv = true })
                .ToList();
        }

        public List<Produto> RecuperarListaDeProdutosComProducao()
        {
            return Db.Query<Produto>(
                "select * from produto where Ativo = @Ativo and TemProducao = @TemProducao Order By Descricao", new { Ativo = true, TemProducao = true })
                .ToList();
        }

        public Entity.Produto RecuperarProdutoPorCodigoDeBarras(string codigoDeBarra)
        {
            return Db.QueryFirstOrDefault<Entity.Produto>(
                "select * from produto where Ativo = @Ativo and CodigoDeBarra = @CodigoDeBarra", new { Ativo = true, TemProducao = true, CodigoDeBarra = codigoDeBarra });
        }
    }
}