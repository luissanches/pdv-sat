using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Bolaria.WpfUI.UI;

namespace Syslaps.Pdv.UI.Telas.Comanda
{
    public partial class ConsultarComanda : Window
    {
        private readonly ConsultarComandaMvvm _mvvm;
        public ConsultarComanda()
        {
            InitializeComponent();
            _mvvm = ContainerIoc.GetInstance<ConsultarComandaMvvm>();
            _mvvm.ProcessoFinalizadoHandler += this.StopWait;
            _mvvm.ProcessoInciadoHandler += this.StartWait;

            _mvvm.AlertHandler += (string s, MessageBoxImage messageBoxImage) =>
            {
                MessageBox.Show(s, InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, messageBoxImage);
            };
            _mvvm.OnDeleted += () =>
            {
                MessageBox.Show("Pedido excluído com sucesso.", InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Information);
            };

            DataContext = _mvvm;
        }

        private void ConsultarComanda_OnClosing(object sender, CancelEventArgs e)
        {
            UIManager<ConsultarComanda>.Close(true);
        }

        private void ConsultarComanda_OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnImprimir_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnFechar_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
