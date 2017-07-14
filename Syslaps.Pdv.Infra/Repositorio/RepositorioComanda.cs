using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Syslaps.Pdv.Core.Dominio.Comanda;
using Syslaps.Pdv.Entity;
using Comanda = Syslaps.Pdv.Entity.Comanda;

namespace Syslaps.Pdv.Infra.Repositorio
{
    public class RepositorioComanda : RepositorioBase, IComandaRepositorio
    {

        public List<Entity.Comanda> RecuperarListaDeComandasAbertas()
        {
            return Db.Query<Entity.Comanda>(
                    "Select * From Comanda Where Situacao = @Situcao",
                    new { Situcao = "aberta" }).ToList();
        }

        public void ExcluirPorCodigoDaComanda(string codigoDaComanda)
        {
            Db.Execute("Delete From ComandaProduto Where Comanda_CodigoComanda = @Comanda_CodigoComanda",
               new { Comanda_CodigoComanda = codigoDaComanda });

            Db.Execute("Delete From Comanda Where CodigoComanda = @CodigoComanda",
                new { CodigoComanda = codigoDaComanda });
        }

        public Comanda RecuperarComanda(string codigoDaComanda)
        {
            var comanda = Db.QueryFirstOrDefault<Entity.Comanda>(
                    "Select * From Comanda Where CodigoComanda = @CodigoComanda and Situacao = @Situacao",
                    new { CodigoComanda = codigoDaComanda, Situacao = SituacaoComanda.aberta.ToString() });

            if(comanda!=null)
            comanda.ComandaProdutoes = Db.Query<Entity.ComandaProduto>(
                    "Select * From ComandaProduto Where Comanda_CodigoComanda = @Comanda_CodigoComanda",
                    new { Comanda_CodigoComanda = codigoDaComanda }).ToList();

            return comanda;
        }
    }
}
