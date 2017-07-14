using Syslaps.Pdv.Cross;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Syslaps.Pdv.UI.Telas.Alertas
{
    public partial class DadosConsumidor : Window
    {
        private PDV.PontoDeVendaMvvm _mvvm;

        public DadosConsumidor(PDV.PontoDeVendaMvvm mvvm = null)
        {
            _mvvm = mvvm;
            InitializeComponent();
            this.Title = InstanceManager.Parametros.TituloNoConfig;
            TxtCpf.Mask = ConfigurationManager.AppSettings["MascaraCPF"];

            if (!_mvvm.VendaCorrente.VendaCorrente.CpfCnpjCliente.IsNullOrEmpty())
                TxtCpf.Text = _mvvm.VendaCorrente.VendaCorrente.CpfCnpjCliente;

            if (!_mvvm.VendaCorrente.VendaCorrente.NomeCliente.IsNullOrEmpty())
                TxtNome.Text = _mvvm.VendaCorrente.VendaCorrente.NomeCliente;
        }

        private void BtnOk_OnClick(object sender, RoutedEventArgs e)
        {
            if (!TxtCpf.Text.IsCpfOrCnpj())
            {
                MessageBox.Show("CPF ou CNPJ inválido.", InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Error);
                TxtCpf.Focus();
                return;
            }

            if (_mvvm != null)
            {
                _mvvm.VendaCorrente.VendaCorrente.CpfCnpjCliente = !TxtCpf.Text.IsNullOrEmpty() ? TxtCpf.Text : "";
                _mvvm.VendaCorrente.VendaCorrente.NomeCliente = !TxtNome.Text.IsNullOrEmpty() ? TxtNome.Text : "";
                _mvvm.VendaCorrente.VendaCorrente.TipoDocumento = tpCnpj.IsChecked.Value ? "CNPJ" : "CPF";
            }

            this.DialogResult = true;
        }

        private void IniciarNovaVenda_OnLoaded(object sender, RoutedEventArgs e)
        {
            TxtCpf.Focus();
        }

        private void TxtCpf_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TpDocumentChecked(object sender, RoutedEventArgs e)
        {
            if (TxtCpf != null)
            {
                TxtCpf.Mask = (sender as Control).Name == "tpCnpj" ? ConfigurationManager.AppSettings["MascaraCNPJ"] : ConfigurationManager.AppSettings["MascaraCPF"];
                TxtCpf.Focus();
                TxtCpf.SelectAll();
            }
        }
    }
}
