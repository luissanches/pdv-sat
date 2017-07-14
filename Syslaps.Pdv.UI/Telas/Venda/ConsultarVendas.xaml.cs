using System.Windows;

namespace Syslaps.Pdv.UI.Telas.Venda
{
    public partial class ConsultarVendas : Window
    {
        private ConsultarVendasMvvm _mvvm;
        public ConsultarVendas()
        {
            InitializeComponent();
            _mvvm = ContainerIoc.GetInstance<ConsultarVendasMvvm>();
            _mvvm.ProcessoFinalizadoHandler += ProcessoFinalizado;
            _mvvm.ProcessoInciadoHandler += FiltroFinalizado;
            _mvvm.AlertHandler += AlertHandler;
            DataContext = _mvvm;
        }

        private void AlertHandler(string s, MessageBoxImage messageBoxImage)
        {
            MessageBox.Show(s, InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, messageBoxImage);
        }

        private void ProcessoFinalizado()
        {
            this.StartWait();
        }

        private void FiltroFinalizado()
        {
            this.StopWait();
        }

        private void BtnOk_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ConsultarVendas_OnLoaded(object sender, RoutedEventArgs e)
        {
            BtnFiltrar.Focus();
        }

    }
}
