using Syslaps.Pdv.Core;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Core.Dominio.Caixa;
using Syslaps.Pdv.Core.Dominio.Cliente;
using Syslaps.Pdv.Core.Dominio.Comanda;
using Syslaps.Pdv.Core.Dominio.Impressora;
using Syslaps.Pdv.Core.Dominio.Pedido;
using Syslaps.Pdv.Core.Dominio.Producao;
using Syslaps.Pdv.Core.Dominio.Produto;
using Syslaps.Pdv.Core.Dominio.SAT;
using Syslaps.Pdv.Core.Dominio.Usuario;
using Syslaps.Pdv.Core.Dominio.Venda;
using Syslaps.Pdv.Infra;
using Syslaps.Pdv.Infra.Impressora.T20;
using Syslaps.Pdv.Infra.Repositorio;

namespace Syslaps.Pdv.UI
{
    public class ContainerIoc : StructureMap.Container
    {
        private static ContainerIoc _currentContainerIoc;
        public static ContainerIoc containerIoc => _currentContainerIoc ?? (_currentContainerIoc = new ContainerIoc());


        private ContainerIoc()
        {
            Configure(ioc =>
            {
                // infra
                ioc.For<IUsuarioRepositorio>().Use<RepositorioUsuario>().Singleton();
                ioc.For<ICaixaRepositorio>().Use<RepositorioCaixa>().Singleton();
                ioc.For<IProducaoRepositorio>().Use<RepositorioProducao>();
                ioc.For<IProdutoRepositorio>().Use<RepositorioProduto>();
                ioc.For<IVendaRepositorio>().Use<RepositorioVenda>();
                ioc.For<ICupomSatRepositorio>().Use<RepositorioCupomSat>();
                ioc.For<IRepositorioBase>().Use<RepositorioBase>();
                ioc.For<IEmail>().Use<Email>().Singleton();
                ioc.For<IImpressora>().Use<Impressora>().Singleton();
                ioc.For<IInfraLogger>().Use<Logger>().Singleton();
                ioc.For<Parametros>().Use<Parametros>().Singleton();
                ioc.For<ClienteCampanha>().Use<ClienteCampanha>().Singleton();
                ioc.For<IRepositorioCriarBaseDeDados>().Use<RepositorioCriarBaseDeDados>().Singleton();
                ioc.For<IClienteCampanhaRepositorio>().Use<RepositorioClienteCampanha>().Singleton();
                ioc.For<IPedidoRepositorio>().Use<RepositorioPedido>().Singleton();
                ioc.For<IPedidoProdutoRepositorio>().Use<RepositorioPedidoProduto>().Singleton();
                ioc.For<IComandaRepositorio>().Use<RepositorioComanda>().Singleton();

            });
        }

        public static void SetDefaultConstructorParameter<TConcretType, TInstance>(TInstance instance, string parameterName)
        {
            containerIoc.Configure(ioc =>
            {
                ioc.ForConcreteType<TConcretType>().Configure.Ctor<TInstance>(parameterName).Is(instance);
            });
        }

        public new static T GetInstance<T>()
        {
            return (T)containerIoc.GetInstance(typeof(T));
        }
    }
}
