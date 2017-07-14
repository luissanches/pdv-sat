using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Syslaps.Pdv.Core.Dominio.Producao;
using Syslaps.Pdv.Entity;

namespace Syslaps.Pdv.Infra.Repositorio
{
    public class RepositorioProducao : RepositorioBase, IProducaoRepositorio
    {
        public List<ProdutoProducao> RecuperarProducaoPorData(DateTime dataDaProducao)
        {
            return Db.Query<ProdutoProducao>("select * from ProdutoProducao where DataProducao = @DataProducao",
                new { DataProducao = dataDaProducao }).ToList();
        }

        public ProdutoProducao RecuperarProducaoDoDiaDeUmProduto(string codigoDeBarraDoProduto, DateTime dataDaProducao)
        {
            var data = new DateTime(dataDaProducao.Year, dataDaProducao.Month, dataDaProducao.Day);
            var entity = 
                Db.QuerySingleOrDefault<ProdutoProducao>(
                    "select * from ProdutoProducao where DataProducao = @DataProducao and Produto_CodigoDeBarra = @Produto_CodigoDeBarra",
                    new ProdutoProducao() {DataProducao = data, Produto_CodigoDeBarra = codigoDeBarraDoProduto});

            return entity;
        }

        public dynamic RecuperarConsultaProducao(DateTime dataInicio, DateTime dataFim)
        {
            return Db.QueryFirst<dynamic>(
                @"select sum(pp.QuantidadeProduzida) QtdeProduzida, sum(pp.QuantidadeDescartadaInteira) QtdeDecartadaInteira, sum(pp.QuantidadeDescartadaParcial) QtdeDecartadaParcial, sum((p.PrecoVenda * pp.QuantidadeDescartadaInteira)) ValorEstimado
                    from ProdutoProducao pp inner
                    join Produto p on pp.Produto_CodigoDeBarra = p.CodigoDeBarra
                    where pp.DataProducao BETWEEN @DataInicio and @DataFim", new { DataInicio = dataInicio, DataFim = dataFim });
        }
    }
}