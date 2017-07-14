using System;
using System.Windows;
using Bolaria.WpfUI.UI;
using Syslaps.Pdv.UI.Telas.Cliente;
using Syslaps.Pdv.UI.Telas.PDV;
using Syslaps.Pdv.UI.Telas.Venda;
using System.Windows.Input;
using Syslaps.Pdv.UI.Telas.Financeiro;

namespace Syslaps.Pdv.UI.Telas
{
    public partial class TelaPrincipal : Window
    {
        public TelaPrincipal()
        {
            InitializeComponent();
            txtTitle.Text = InstanceManager.Parametros.TituloNoConfig;
            this.Title = InstanceManager.Parametros.TituloNoConfig;
            BtnSat.Visibility = InstanceManager.Parametros.SatHabilitado ? Visibility.Visible : Visibility.Hidden;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnCaixa_Click(object sender, RoutedEventArgs e)
        {
            UIManager<PontoDeVenda>.Show(this);
        }

        private void BtnProducao_Click(object sender, RoutedEventArgs e)
        {
            UIManager<Producao.Producao>.Show(this);
        }

        private void BtnClientesCampanha_OnClick(object sender, RoutedEventArgs e)
        {
            (new ClienteCampanha()).ShowDialog();
        }

        private void BtnImprimirVendas_OnClick(object sender, RoutedEventArgs e)
        {
            (new ConsultarVendas()).ShowDialog();
        }

        private void BtnImportarProdutos_OnClick(object sender, RoutedEventArgs e)
        {
            this.StartWait();
            try
            {
                (new Alertas.ExecutarProcessoImportarProduto()).ShowDialog();
            }
            finally
            {
                this.StopWait();
            }
        }


        private void TelaPrincipal_OnLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnPedidos_OnClick(object sender, RoutedEventArgs e)
        {
            UIManager<Pedido.Pedido>.Show(this);
        }

        private void BtnSat_OnClick(object sender, RoutedEventArgs e)
        {
            (new SAT.ConfiguracaoSAT()).ShowDialog();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
            {
                switch (e.Key)
                {
                    case Key.S:
                        BtnSat_OnClick(null, null);
                        return;
                }
            }
        }

        private void BtnFinanceiro_Click(object sender, RoutedEventArgs e)
        {
            (new ExportarNotaFiscal()).ShowDialog();
        }
    }
}
