using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Syslaps.Pdv.Core.Dominio;
using Syslaps.Pdv.Cross;
using Syslaps.Pdv.Entity;

namespace Syslaps.Pdv.UI.Telas.Pedido
{
    public class IncluirProdutoComandaMvvm : INotifyPropertyChanged
    {
        public Action OnProcessoInciado;
        public Action OnProcessoFinalizado;
        public Action OnPedidoRegistrado;
        public Action<string, MessageBoxImage> OnAlert;
        public event PropertyChangedEventHandler PropertyChanged;

        public IncluirProdutoComandaMvvm(Core.Dominio.Pedido.Pedido pedidoDominio)
        {
            _pedidoDominio = pedidoDominio;
            FiltrarComEsteTexto = "";
            ListaPedidoProdutos = new List<PedidoProduto>();
        }

        private readonly Core.Dominio.Pedido.Pedido _pedidoDominio;
        private readonly Core.Dominio.Producao.Producao _producaoDominio;
        private List<Produto> _listaDeProdutosDoFiltrada;
        private List<PedidoProduto> _listaDeProdutosDoPedido;
        private string _filtrarComEsteTexto;
        private decimal _varlorDoPedido;

        public List<PedidoProduto> ListaPedidoProdutos
        {
            get
            {
                return _listaDeProdutosDoPedido;
            }
            set
            {
                _listaDeProdutosDoPedido = value;
                OnPropertyChanged();
            }
        }

        public List<Produto> ListaDeProdutosFiltrada
        {
            get { return _listaDeProdutosDoFiltrada; }
            set
            {
                _listaDeProdutosDoFiltrada = value;
                OnPropertyChanged();
            }
        }

        public DateTime DataEntrega { get; set; }

        public string HoraEntrega { get; set; }

        public Entity.Pedido PedidoCorrente
        {
            get { return _pedidoDominio.PedidoCorrente; }
            set
            {
                _pedidoDominio.PedidoCorrente = value;

                DataEntrega = value.DataEntrega;
                HoraEntrega = value.DataEntrega.ToString("HH:mm");
                OnPropertyChanged("DataEntrega");
                OnPropertyChanged("HoraEntrega");


                ListaPedidoProdutos = _pedidoDominio.PedidoCorrente.PedidoProduto.Reverse().ToList();
                AtualizarValorDoPedido();
                OnPropertyChanged();
            }
        }

        public decimal ValorDoPedido
        {
            get { return _varlorDoPedido; }
            set
            {
                _varlorDoPedido = value;
                OnPropertyChanged();
            }
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ICommand _registrarPedidoCommand;
        

        public ICommand RegistrarPedidoCommand
        {
            get
            {
                return _registrarPedidoCommand ?? (_registrarPedidoCommand = new RelayCommandMvvm(param =>
                {
                    OnProcessoInciado?.Invoke();

                    _pedidoDominio.PedidoCorrente.Valor = this.ValorDoPedido;
                    _pedidoDominio.PedidoCorrente.DataEntrega = DateTime.ParseExact(string.Concat(DataEntrega.ToString("dd/MM/yyyy"), " ", HoraEntrega), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                    _pedidoDominio.RegistrarPedido();

                    if (_pedidoDominio.Status != EnumStatusDoResultado.MensagemDeSucesso)
                    {
                        OnAlert?.Invoke(_pedidoDominio.Mensagem, MessageBoxImage.Error);
                    }
                    else
                    {
                        OnPedidoRegistrado?.Invoke();
                    }


                    





                    OnProcessoFinalizado?.Invoke();

                }, null));
            }
        }

        public void AtualizarValorDoPedido()
        {
            this.ValorDoPedido = ListaPedidoProdutos.Sum(x => x.Produto.PrecoVenda * x.Quantidade);
        }

        public void AtualizarQuantidadeDoPedidoProduto(PedidoProduto pedidoProduto)
        {
            _pedidoDominio.AtualizarQuantidadeDoProdutoDoPedido(pedidoProduto);
            ListaPedidoProdutos = _pedidoDominio.PedidoCorrente.PedidoProduto.Reverse().ToList();
            AtualizarValorDoPedido();
        }

        public void AdicionarPedidoProduto(PedidoProduto pedidoProduto)
        {
            _pedidoDominio.AdicionarProdutoNoPedido(pedidoProduto);
            ListaPedidoProdutos = _pedidoDominio.PedidoCorrente.PedidoProduto.Reverse().ToList();
            AtualizarValorDoPedido();
        }

        public void RemoverVendaProduto(PedidoProduto pedidoProduto)
        {
            _pedidoDominio.RemoverProdutoDoPedido(pedidoProduto);
            ListaPedidoProdutos = _pedidoDominio.PedidoCorrente.PedidoProduto.Reverse().ToList();
            AtualizarValorDoPedido();
        }

        public string FiltrarComEsteTexto
        {
            get { return _filtrarComEsteTexto; }
            set
            {
                _filtrarComEsteTexto = value;
                Task.Factory.StartNew(() =>
                {
                    if (value.Length > 0)
                    {
                        ListaDeProdutosFiltrada =
                            InstanceManager.ListaDeProdutosDoPdv.Where(
                                x => x.DescricaoBusca.Contains(value.ToComparableString()) && x.TemProducao).ToList();
                    }
                    else
                    {
                        ListaDeProdutosFiltrada =
                            InstanceManager.ListaDeProdutosDoPdv.Where(x => x.TemProducao).ToList();
                    }
                });
            }
        }
    }
}
