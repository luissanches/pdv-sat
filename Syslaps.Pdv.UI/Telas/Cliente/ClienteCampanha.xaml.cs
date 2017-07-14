using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Syslaps.Pdv.Core.Dominio.Cliente;

namespace Syslaps.Pdv.UI.Telas.Cliente
{
    public partial class ClienteCampanha : Window
    {
        private readonly ClienteCampanhaMvvm _mvvm;
        public ClienteCampanha()
        {
            InitializeComponent();
            _mvvm = new ClienteCampanhaMvvm();
            _mvvm.ClienteRegistradoHandler += ClienteRegistradoHandler;
            _mvvm.ErroAoRegistrarClienteHandler += ErroAoRegistrarClienteHandler;
            DataContext = _mvvm;
        }

        private void ErroAoRegistrarClienteHandler(string s)
        {
            MessageBox.Show(s, InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Information);
            
        }

        private void ClienteRegistradoHandler()
        {
            MessageBox.Show("Cliente registrado com sucesso.", InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Information);
            CmbTipoCampanha.SelectedIndex = 0;
            TxtCpf.Focus();
        }

        private void BtnOk_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ClienteCampanha_OnLoaded(object sender, RoutedEventArgs e)
        {
            TxtCpf.Focus();
        }

        private void TxtCpf_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text[0]) && e.Text != "." && e.Text != "-")
                e.Handled = true;
            else
            {
                e.Handled = false;
            }
        }

        private void TxtTelefone_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text[0]) && e.Text != "(" && e.Text != ")" && e.Text != "(" && e.Text != "-")
                e.Handled = true;
            else
            {
                e.Handled = false;
            }
        }

        private void TxtCpf_OnLostFocus(object sender, RoutedEventArgs e)
        {
            _mvvm.ClienteCampanha.NomeCampanha = _mvvm.TipoCampanhaSelecionada.ToString();
            _mvvm.ClienteCampanha.CpfCnpj = TxtCpf.Text;
            var clienteCampanha = ContainerIoc.GetInstance<Core.Dominio.Cliente.ClienteCampanha>().RecuperarcClienteNaCampanha(_mvvm.ClienteCampanha);
            if(clienteCampanha == null)return;
            _mvvm.ClienteCampanha = clienteCampanha;
            string message = $"Este cliente foi cadastrado em: {_mvvm.ClienteCampanha.DataCadastro.ToString("dd/MM/yyyy HH:mm")}";
            message += $"\nNome: {_mvvm.ClienteCampanha.NomeCliente}";
            MessageBox.Show(message, InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
