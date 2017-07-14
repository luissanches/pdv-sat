using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using Syslaps.Pdv.Core.Dominio.Cliente;

namespace Syslaps.Pdv.UI.Telas.Cliente
{
    public class ClienteCampanhaMvvm : INotifyPropertyChanged
    {
        public Action ClienteRegistradoHandler;
        public Action<string> ErroAoRegistrarClienteHandler;

        public ClienteCampanhaMvvm()
        {
            ClienteCampanha = new Entity.ClienteCampanha();
        }

        private Entity.ClienteCampanha _clienteCampanha;

        public Entity.ClienteCampanha ClienteCampanha
        {
            get { return _clienteCampanha;}
            set { _clienteCampanha = value; OnPropertyChanged(); }
        }


        public Dictionary<EnumTipoCampanha, string> TipoCampanha => new Dictionary<EnumTipoCampanha, string>
        {
            {EnumTipoCampanha.VemExperimentar, "Vem Experimantar"}
        };


        private EnumTipoCampanha _tipoCampanhaSelecionada;

        public EnumTipoCampanha TipoCampanhaSelecionada
        {
            get { return _tipoCampanhaSelecionada; }
            set
            {
                _tipoCampanhaSelecionada = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private ICommand _registrarClienteCommand;
        public ICommand RegistrarClienteCommand
        {
            get
            {
                return _registrarClienteCommand ?? (_registrarClienteCommand = new RelayCommandMvvm(param =>
                {
                    if (string.IsNullOrEmpty(ClienteCampanha.CpfCnpj))
                    {
                        ErroAoRegistrarClienteHandler?.Invoke("CPF é obrigatório.");
                        return;
                    }

                    ContainerIoc.GetInstance<Core.Dominio.Cliente.ClienteCampanha>()
                            .RegistrarClienteNaCampanha(ClienteCampanha, TipoCampanhaSelecionada);

                    ClienteCampanha = new Entity.ClienteCampanha();
                    ClienteRegistradoHandler?.Invoke();
                }, null));
            }
        }
    }
}