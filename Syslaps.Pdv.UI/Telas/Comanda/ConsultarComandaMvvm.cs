using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Syslaps.Pdv.Core.Dominio.Impressora;

namespace Syslaps.Pdv.UI.Telas.Comanda
{
    public class ConsultarComandaMvvm : INotifyPropertyChanged
    {
        public Action<string> ProcessoInciadoHandler;
        public Action ProcessoFinalizadoHandler;
        public Action<string, MessageBoxImage> AlertHandler;
        public Action OnDeleted;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Core.Dominio.Comanda.Comanda _comandaDominio;
        private readonly Core.Dominio.Impressora.Cupom _cupomDominio;
        private List<Entity.Comanda> _listaDePedidos;

        public Window ParentWindow { get; set; }

        public ConsultarComandaMvvm(Core.Dominio.Comanda.Comanda comandaDominio, Cupom cupomDominio)
        {
            _comandaDominio = comandaDominio;
            _cupomDominio = cupomDominio;
            ListaDeComandasAbertas = _comandaDominio.RecuperarListaDeComandasAbertas();
        }


        public List<Entity.Comanda> ListaDeComandasAbertas
        {
            get { return _listaDePedidos; }
            set
            {
                _listaDePedidos = value;
                OnPropertyChanged();
            }
        }

        public void ImprimirPedido(Entity.Pedido pedido)
        {
            ProcessoInciadoHandler?.Invoke( "Aguarde...");
            _cupomDominio.ImprimirPedido(pedido);
            _cupomDominio.AcionarGuilhotina();
            ProcessoFinalizadoHandler?.Invoke();
        }

        public void ExcluirComanda(Entity.Comanda comanda)
        {
            ProcessoInciadoHandler?.Invoke("Aguarde...");
            _comandaDominio.ExcluirComanda();
            OnDeleted?.Invoke();
            ProcessoFinalizadoHandler?.Invoke();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
