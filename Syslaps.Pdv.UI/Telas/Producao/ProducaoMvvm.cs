using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Syslaps.Pdv.Core.Dominio.Producao;
using Syslaps.Pdv.Core.Dominio.Produto;
using Syslaps.Pdv.Cross;
using Syslaps.Pdv.Entity;

namespace Syslaps.Pdv.UI.Telas.Producao
{
    public class ProducaoMvvm : INotifyPropertyChanged
    {
        private readonly IProducaoRepositorio _respositorioRepositorio;

        public ProducaoMvvm(IProducaoRepositorio respositorioRepositorio)
        {
            this._respositorioRepositorio = respositorioRepositorio;
        }

        private string _filtrarComEsteTexto;
        private List<ProducaoMvvmAgregador> _listaDeProdutoProducoesFiltrada;
        private DateTime _dataDeProducao;

        public List<ProducaoMvvmAgregador> ListaDeProdutoProducoes { get; private set; }

        public List<ProducaoMvvmAgregador> ListaDeProdutoProducoesFiltrada
        {
            get { return _listaDeProdutoProducoesFiltrada; }
            set
            {
                _listaDeProdutoProducoesFiltrada = value;
                OnPropertyChanged();
            }
        }

        public DateTime DataDeProducao
        {
            get { return _dataDeProducao; }
            set
            {
                _dataDeProducao = new DateTime(value.Year, value.Month, value.Day);
                CarregarListaDeProducao();
                OnPropertyChanged();
            }
        }

        public string FiltrarComEsteTexto
        {
            get { return _filtrarComEsteTexto; }
            set
            {
                _filtrarComEsteTexto = value;

                Task.Factory.StartNew(() =>
                {
                    ListaDeProdutoProducoesFiltrada =
                        ListaDeProdutoProducoes.Where(x => x.Produto.DescricaoBusca.Contains(value.ToComparableString())).ToList();
                    OnPropertyChanged();
                });


            }
        }

        private void CarregarListaDeProducao()
        {
            var produtosComProducao = InstanceManager.ListaDeProdutosDoPdv.Where(x => x.TemProducao);
            ListaDeProdutoProducoes =
                produtosComProducao.Select(
                    x =>
                        new ProducaoMvvmAgregador()
                        {
                            Produto = x,
                            ProdutoProducao =
                                _respositorioRepositorio.RecuperarProducaoDoDiaDeUmProduto(x.CodigoDeBarra,
                                    _dataDeProducao) ??
                                new ProdutoProducao()
                                {
                                    Produto = x,
                                    Produto_CodigoDeBarra = x.CodigoDeBarra,
                                    DataProducao = _dataDeProducao
                                }
                        }).ToList();
            ListaDeProdutoProducoesFiltrada = ListaDeProdutoProducoes;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ProducaoMvvmAgregador
    {
        private ICommand lostFocusCommand;
        public Entity.Produto Produto { get; set; }
        public Entity.ProdutoProducao ProdutoProducao { get; set; }
        public ICommand LostFocusCommand
        {
            get {
                return lostFocusCommand ?? (lostFocusCommand = new RelayCommandMvvm(param => this.LostTextBoxFocus(), null));
            }
        }
        public async void LostTextBoxFocus()
        {
            var task = Task<Core.Dominio.Producao.Producao>.Factory.StartNew(() =>
            {
                var producao = ContainerIoc.GetInstance<Core.Dominio.Producao.Producao>();
                producao.RegistrarProdutoProducao(ProdutoProducao);
                return producao;
            });

            await task;
        }
    }
}
