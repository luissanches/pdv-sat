using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Syslaps.Pdv.Core.Dominio.Impressora;
using Syslaps.Pdv.Core.Dominio.Venda;
using Syslaps.Pdv.Cross;

namespace Syslaps.Pdv.UI.Telas.Venda
{
    public class ConsultarVendasMvvm : INotifyPropertyChanged
    {
        private IVendaRepositorio _vendaRepositorio;
        public Action ProcessoInciadoHandler;
        public Action ProcessoFinalizadoHandler;
        public Action<string, MessageBoxImage> AlertHandler;

        public ConsultarVendasMvvm(IVendaRepositorio vendaRepositorio)
        {
            _vendaRepositorio = vendaRepositorio;
            DataInicial = DateTime.Now;
            DataFinal = DateTime.Now.GetLastDateTimeOfDay();
            ListaDeVendas = new List<Entity.Venda>();
            ListaDeTipoPagamentos =
                InstanceManager.ListaDeTipoPagamentos.Select(c => new TipoDePagamentoCheckBoxList {TipoPagamento = c, EstaSelecionado = true})
                    .ToList();
        }

        private DateTime _dataInicial;

        private DateTime _dataFinal;

        private List<Entity.Venda> _listaDeVendas;

        private List<Entity.Venda> _listaDeVendasFiltrada;

        public DateTime DataInicial
        {
            get
            {
                return _dataInicial.GeFirstDateTimeOfDay();
            }

            set
            {
                _dataInicial = value;
                OnPropertyChanged();
            }
        }

        public DateTime DataFinal
        {
            get
            {
                return _dataFinal.GetLastDateTimeOfDay();
            }

            set
            {
                _dataFinal = value;
                OnPropertyChanged();
            }
        }

        public List<Entity.Venda> ListaDeVendas
        {
            get
            {
                return _listaDeVendas;
            }

            set
            {
                _listaDeVendas = value;
            }
        }

        private List<TipoDePagamentoCheckBoxList> _listaDeTipoPagamentos;

        public List<TipoDePagamentoCheckBoxList> ListaDeTipoPagamentos
        {
            get { return _listaDeTipoPagamentos;}
            set { _listaDeTipoPagamentos = value; OnPropertyChanged(); }
        }

        public List<Entity.Venda> ListaDeVendasFiltrada
        {
            get
            {
                return _listaDeVendasFiltrada;
            }

            set
            {
                _listaDeVendasFiltrada = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private ICommand _filtrarVendasCommand;
        public ICommand FiltrarVendasCommand
        {
            get
            {
                return _filtrarVendasCommand ?? (_filtrarVendasCommand = new RelayCommandMvvm(param =>
                {
                    ProcessoFinalizadoHandler?.Invoke();
                     ListaDeVendas = _vendaRepositorio.RecuperarVendasDoPeriodo(DataInicial, DataFinal);
                    var tiposSelecionados = ListaDeTipoPagamentos.Where(x => x.EstaSelecionado);


                    ListaDeVendasFiltrada =
                        ListaDeVendas.Where(
                            x =>
                                x.VendaPagamentoes.ToList()
                                    .Exists(
                                        y =>
                                            tiposSelecionados.ToList()
                                                .Exists(
                                                    z =>
                                                        z.TipoPagamento.CodigoTipoPagamento ==
                                                        y.TipoPagamento_CodigoTipoPagamento))).ToList();

                    ProcessoInciadoHandler?.Invoke();

                }, null));
            }
        }

        private ICommand _imprimirVendasCommand;
        public ICommand ImprimirVendasCommand
        {
            get
            {
                return _imprimirVendasCommand ?? (_imprimirVendasCommand = new RelayCommandMvvm(param =>
                {
                    if (ListaDeVendasFiltrada.Count == 0)
                    {
                        AlertHandler?.Invoke("Nenhum registro para impressão.", MessageBoxImage.Exclamation);
                        return;    
                    }

                    ProcessoFinalizadoHandler?.Invoke();

                    Task.Factory.StartNew(
                        () => ContainerIoc.GetInstance<Cupom>().ImprimirListaVendaCurta(ListaDeVendasFiltrada));



                    ProcessoInciadoHandler?.Invoke();

                }, null));
            }
        }
    }

    public class TipoDePagamentoCheckBoxList
    {
        public Entity.TipoPagamento TipoPagamento { get; set; }
        public bool EstaSelecionado { get; set; }
    }
}