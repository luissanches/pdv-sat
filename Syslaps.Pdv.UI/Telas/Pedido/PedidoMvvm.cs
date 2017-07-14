using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Syslaps.Pdv.Core.Dominio.Impressora;
using Syslaps.Pdv.Core.Dominio.Pedido;

namespace Syslaps.Pdv.UI.Telas.Pedido
{
    public class PedidoMvvm : INotifyPropertyChanged
    {
        public Action ProcessoInciadoHandler;
        public Action ProcessoFinalizadoHandler;
        public Action<string, MessageBoxImage> AlertHandler;
        public Action OnDeleted;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Core.Dominio.Pedido.Pedido _pedidoDominio;
        private readonly Core.Dominio.Impressora.Cupom _cupomDominio;
        private DateTime _dataDaEntrega;
        private List<Entity.Pedido> _listaDePedidos;

        public PedidoMvvm(Core.Dominio.Pedido.Pedido pedidoDominio, Cupom cupomDominio)
        {
            _pedidoDominio = pedidoDominio;
            _cupomDominio = cupomDominio;
            DataDaEntrega = DateTime.Now;
        }

        public DateTime DataDaEntrega
        {
            get { return _dataDaEntrega; }
            set
            {
                _dataDaEntrega = value;
                ProcessoInciadoHandler?.Invoke();
                ListaDePedidos = CarregarListaDePedido();
                ProcessoFinalizadoHandler?.Invoke();
                OnPropertyChanged();
            }
        }

        public List<Entity.Pedido> ListaDePedidos
        {
            get { return _listaDePedidos; }
            set
            {
                _listaDePedidos = value;
                OnPropertyChanged();
            }
        }

        private List<Entity.Pedido> CarregarListaDePedido()
        {
            return _pedidoDominio.RecuperarListaDePedidosPorDataDeEntrega(_dataDaEntrega);
        }

        public void ImprimirPedido(Entity.Pedido pedido)
        {
            ProcessoInciadoHandler?.Invoke();
            _cupomDominio.ImprimirPedido(pedido);
            _cupomDominio.AcionarGuilhotina();
            ProcessoFinalizadoHandler?.Invoke();
        }

        public void ExcluirPedido(Entity.Pedido pedido)
        {
            ProcessoInciadoHandler?.Invoke();
            _pedidoDominio.ExcluirPedido(pedido);
            OnDeleted?.Invoke();
            ProcessoFinalizadoHandler?.Invoke();
        }

        public void ImprimirTodosOsPedidos()
        {
            ProcessoInciadoHandler?.Invoke();
            ListaDePedidos.ForEach(pedido =>
            {
                _cupomDominio.ImprimirPedido(pedido);
            });
            _cupomDominio.AcionarGuilhotina();
            ProcessoFinalizadoHandler?.Invoke();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
