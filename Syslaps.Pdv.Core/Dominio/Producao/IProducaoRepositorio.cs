using System;
using System.Collections.Generic;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Entity;

namespace Syslaps.Pdv.Core.Dominio.Producao
{
    public interface IProducaoRepositorio : IRepositorioBase
    {
        List<ProdutoProducao> RecuperarProducaoPorData(DateTime dataDaProducao);

        ProdutoProducao RecuperarProducaoDoDiaDeUmProduto(string codigoDeBarraDoProduto, DateTime dataDaProducao);

        dynamic RecuperarConsultaProducao(DateTime dataInicio, DateTime dataFim);
    }
}