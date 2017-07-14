using System;
using System.Collections.Generic;
using System.Linq;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Cross;
using Syslaps.Pdv.Entity;

namespace Syslaps.Pdv.Core.Dominio.Comanda
{
    public class Comanda : Base.ModeloBase
    {
        private readonly IComandaRepositorio _comandaRepositorio;
        private readonly IInfraLogger _logger;
        private readonly IRepositorioBase _repositorio;


        public Comanda(IComandaRepositorio comandaRepositorio, IInfraLogger logger, IRepositorioBase repositorio)
        {
            ComandaCorrente = new Entity.Comanda();
            ComandaCorrente.ComandaProdutoes = new List<ComandaProduto>();
            _comandaRepositorio = comandaRepositorio;
            _logger = logger;
            _repositorio = repositorio;
        }

        public Entity.Comanda ComandaCorrente { get; set; }

        public void AdicionarAlterarProdutoNaComanda(decimal quantidade, Entity.Produto produto)
        {
            var produtoNaComanda = ComandaCorrente.ComandaProdutoes.ToList().FirstOrDefault(
               x => x.Produto_CodigoDeBarra == produto.CodigoDeBarra);

            if (produtoNaComanda != null)
            {
                    produtoNaComanda.Quantidade += quantidade;
            }
            else
                ComandaCorrente.ComandaProdutoes.Add(new ComandaProduto { Produto_CodigoDeBarra = produto.CodigoDeBarra, Quantidade = quantidade });
        }

        public void RemoverProdutoDaComanda(Entity.Produto produto)
        {
            var produtoNaComanda = ComandaCorrente.ComandaProdutoes.FirstOrDefault(
                x => x.Produto_CodigoDeBarra == produto.CodigoDeBarra);

            if (produtoNaComanda != null)
                ComandaCorrente.ComandaProdutoes.Remove(produtoNaComanda);
        }

        public void RegistrarComanda(string codigoComanda)
        {
            try
            {
                ComandaCorrente.CodigoComanda = codigoComanda;
                ComandaCorrente.Situacao = SituacaoComanda.aberta.ToString();
                ComandaCorrente.Sincronizado = false;
                _comandaRepositorio.Inserir(ComandaCorrente);

                ComandaCorrente.ComandaProdutoes.ToList().ForEach((item) =>
                {
                    item.CodigoComandaProduto = this.GerarCodigoUnico();
                    item.Comanda_CodigoComanda = ComandaCorrente.CodigoComanda;
                    item.Sincronizado = false;
                    _repositorio.Inserir(item);
                });
                AdicionarMensagem("Comanda Registrada com Sucesso");
            }
            catch (Exception ex)
            {
                _logger.Log().Error(ex);
            }

        }

        public void FecharComanda()
        {
            ComandaCorrente.Situacao = SituacaoComanda.fechada.ToString();
            _comandaRepositorio.Atualizar(ComandaCorrente);
        }

        public void ExcluirComanda()
        {
            if (!ComandaCorrente.CodigoComanda.IsNullOrEmpty())
            {
                _comandaRepositorio.ExcluirPorCodigoDaComanda(ComandaCorrente.CodigoComanda);
                ComandaCorrente = null;
                ComandaCorrente = new Entity.Comanda();
                ComandaCorrente.ComandaProdutoes = new List<ComandaProduto>();
            }
        }

        public List<Entity.Comanda> RecuperarListaDeComandasAbertas()
        {
            return _comandaRepositorio.RecuperarListaDeComandasAbertas();
        }

        public Entity.Comanda RecuperarComanda(string codigoDaComanda)
        {
            ComandaCorrente = _comandaRepositorio.RecuperarComanda(codigoDaComanda);

            if (ComandaCorrente == null)
            {
                ComandaCorrente = new Entity.Comanda();
                ComandaCorrente.ComandaProdutoes = new List<ComandaProduto>();
            }

            return ComandaCorrente;
        }
    }
}
