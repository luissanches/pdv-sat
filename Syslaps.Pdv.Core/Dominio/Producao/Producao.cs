using System;
using System.Collections.Generic;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Cross;
using Syslaps.Pdv.Entity;

namespace Syslaps.Pdv.Core.Dominio.Producao
{
    public class Producao : ModeloBase
    {
        private readonly IProducaoRepositorio producaoRepositorio;

        public Producao(IProducaoRepositorio producaoRepositorio)
        {
            this.producaoRepositorio = producaoRepositorio;
        }

        public ProdutoProducao CriarAlterarProducaoDeProduto(string codigoDeBarra, DateTime dataDaProducao, int qtdeProduzida, int qtdeDescartadaInteira, int qtdeDescartadaParcial)
        {
            var data = new DateTime(dataDaProducao.Year, dataDaProducao.Month, dataDaProducao.Day);
            var produtoProducaoExistente = RecuperarProducaoDoDiaDeUmProduto(codigoDeBarra, data);

            if (produtoProducaoExistente != null)
            {
                produtoProducaoExistente.QuantidadeProduzida = qtdeProduzida;
                produtoProducaoExistente.QuantidadeDescartadaInteira = qtdeDescartadaInteira;
                produtoProducaoExistente.QuantidadeDescartadaParcial = qtdeDescartadaParcial;

                producaoRepositorio.Atualizar(produtoProducaoExistente);
                return produtoProducaoExistente;
            }

            var produtoProducao = new ProdutoProducao
            {
                CodigoProdutoProducao = GerarCodigoUnico(),
                Produto_CodigoDeBarra = codigoDeBarra,
                DataProducao = data,
                QuantidadeProduzida = qtdeProduzida,
                QuantidadeDescartadaInteira = qtdeDescartadaInteira,
                QuantidadeDescartadaParcial = qtdeDescartadaParcial
            };

            producaoRepositorio.Inserir(produtoProducao);
            AdicionarMensagem("Produção Registrada com Sucesso.");
            return produtoProducao;
        }

        public void RegistrarProducao(List<ProdutoProducao> listaDeProdutoProducaos)
        {
            try
            {
                listaDeProdutoProducaos.ForEach(RegistrarProdutoProducao);

                AdicionarMensagem("Produção Registrada com Sucesso.");
            }
            catch (Exception ex)
            {
                AdicionarMensagem(ex.Message, EnumStatusDoResultado.ErroGerenciado);
            }
        }

        public void RegistrarProdutoProducao(ProdutoProducao produtoProducao)
        {
            if (produtoProducao.CodigoProdutoProducao.IsNullOrEmpty())
            {
                if (produtoProducao.QuantidadeProduzida > 0 || produtoProducao.QuantidadeDescartadaInteira > 0 ||
                    produtoProducao.QuantidadeDescartadaParcial > 0)
                {
                    produtoProducao.CodigoProdutoProducao = GerarCodigoUnico();
                    producaoRepositorio.Inserir(produtoProducao);
                }
            }
            else if (produtoProducao.QuantidadeProduzida == 0 && produtoProducao.QuantidadeDescartadaInteira == 0 &&
                     produtoProducao.QuantidadeDescartadaParcial == 0)
                producaoRepositorio.Excluir(produtoProducao);
            else
                producaoRepositorio.Atualizar(produtoProducao);
        }


        public List<ProdutoProducao> RecuperarProducaoDoDia(DateTime dataDaProducao)
        {
            return producaoRepositorio.RecuperarProducaoPorData(dataDaProducao);
        }

        public ProdutoProducao RecuperarProducaoDoDiaDeUmProduto(string codigoDeBarraDoProduto, DateTime dataDaProducao)
        {
            return producaoRepositorio.RecuperarProducaoDoDiaDeUmProduto(codigoDeBarraDoProduto, dataDaProducao);
        }

        public dynamic RecuperarConsultaProducao(DateTime dataInicio, DateTime dataFim)
        {
            return producaoRepositorio.RecuperarConsultaProducao(dataInicio, dataFim);
        }
    }
}
