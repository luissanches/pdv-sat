using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap.Pipeline;
using Syslaps.Pdv.Core;
using Syslaps.Pdv.Core.Dominio;
using Syslaps.Pdv.Core.Dominio.Caixa;
using Syslaps.Pdv.Core.Dominio.Produto;
using Syslaps.Pdv.Core.Dominio.Usuario;
using Syslaps.Pdv.Cross;

namespace Syslaps.Pdv.Test.Model
{
    [TestClass]
    public class Venda
    {
        [TestMethod]
        public void DevoEfetuarVenda()
        {

            var usuario = ContainerIoc.GetInstance<Usuario>();
            usuario.LogarUsuario("admin", "123");
            Assert.IsTrue(usuario.Status == EnumStatusDoResultado.MensagemDeSucesso);
            
            if (usuario.Status == EnumStatusDoResultado.MensagemDeSucesso)
            {
                ContainerIoc.containerIoc.Configure(ioc =>
                {
                    ioc.ForConcreteType<Caixa>().Configure.Ctor<string>("nomeDoCaixa").Is("NomeDoCaixa".GetConfigValue()).Ctor<Entity.Usuario>().Is(usuario.UsuarioLogado);
                });
            }
            var caixa = ContainerIoc.GetInstance<Caixa>();
            ContainerIoc.SetDefaultConstructorParameter<Core.Dominio.Venda.Venda, Caixa>(caixa, "caixaAberto");

            caixa.AbrirCaixa(100);


            var produto = ContainerIoc.GetInstance<Produto>();
            var listaDeProdutos = produto.RecuperarListaDeProdutosDoPdv();

            var venda1 = ContainerIoc.GetInstance<Core.Dominio.Venda.Venda>();
            venda1.AdicionarProdutoNaVenda(listaDeProdutos[0], 10, listaDeProdutos[0].PrecoVenda);
            venda1.AdicionarProdutoNaVenda(listaDeProdutos[1], 5, listaDeProdutos[1].PrecoVenda);
            venda1.AdicionarProdutoNaVenda(listaDeProdutos[2], 5, listaDeProdutos[2].PrecoVenda);
            venda1.AdicionarProdutoNaVenda(listaDeProdutos[3], 5, listaDeProdutos[3].PrecoVenda - 1);
            venda1.AdicionarPagamento(caixa.TipoDoPagamento.Dinheiro, 100);

            var venda2 = ContainerIoc.GetInstance<Core.Dominio.Venda.Venda>();
            venda2.AdicionarProdutoNaVenda(listaDeProdutos[0], 10, listaDeProdutos[0].PrecoVenda);
            venda2.AdicionarProdutoNaVenda(listaDeProdutos[1], 5, listaDeProdutos[1].PrecoVenda);
            venda2.AdicionarProdutoNaVenda(listaDeProdutos[2], 5, listaDeProdutos[2].PrecoVenda);
            venda2.AdicionarProdutoNaVenda(listaDeProdutos[3], 5, listaDeProdutos[3].PrecoVenda - 1);
            venda2.AdicionarPagamento(caixa.TipoDoPagamento.DebitoRede, venda2.VendaCorrente.ValorTotalVenda);

            var venda3 = ContainerIoc.GetInstance<Core.Dominio.Venda.Venda>();
            venda3.AdicionarProdutoNaVenda(listaDeProdutos[12], 10, listaDeProdutos[12].PrecoVenda);
            venda3.AdicionarProdutoNaVenda(listaDeProdutos[44], 5, listaDeProdutos[44].PrecoVenda);
            venda3.AdicionarProdutoNaVenda(listaDeProdutos[33], 5, listaDeProdutos[33].PrecoVenda);
            venda3.AdicionarProdutoNaVenda(listaDeProdutos[66], 5, listaDeProdutos[66].PrecoVenda - 1);
            venda3.AdicionarPagamento(caixa.TipoDoPagamento.CreditoRede, venda3.VendaCorrente.ValorTotalVenda);

            var venda4 = ContainerIoc.GetInstance<Core.Dominio.Venda.Venda>();
            venda4.AdicionarProdutoNaVenda(listaDeProdutos[12], 10, listaDeProdutos[12].PrecoVenda);
            venda4.AdicionarProdutoNaVenda(listaDeProdutos[44], 5, listaDeProdutos[44].PrecoVenda);
            venda4.AdicionarProdutoNaVenda(listaDeProdutos[88], 5, listaDeProdutos[88].PrecoVenda);
            venda4.AdicionarProdutoNaVenda(listaDeProdutos[66], 5, listaDeProdutos[66].PrecoVenda - 1);
            venda4.AdicionarPagamento(caixa.TipoDoPagamento.Tiket, venda4.VendaCorrente.ValorTotalVenda);

            caixa.EfetuarSangria(80.5m);
            caixa.AdicionarReforco(200);

            caixa.FecharCaixa(175);
        }
    }
}
