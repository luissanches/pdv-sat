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
using Syslaps.Pdv.UI.Telas.Producao;

namespace Syslaps.Pdv.UI.Telas.Pedido
{
    /// <summary>
    /// Interaction logic for Pedido.xaml
    /// </summary>
    public partial class Pedido : Window
    {
        private readonly PedidoMvvm _mvvm;
        private IncluirPedido _produtoPedido;
        public Pedido()
        {
            InitializeComponent();
            _mvvm = ContainerIoc.GetInstance<PedidoMvvm>();
            _mvvm.ProcessoFinalizadoHandler += ProcessoFinalizado;
            _mvvm.ProcessoInciadoHandler += ProcessoInicializado;
            _mvvm.AlertHandler += AlertHandler;
            _mvvm.OnDeleted += OnDeleted;
            DataContext = _mvvm;

        }

        private void OnDeleted()
        {
            PedidoAtualizado();
            MessageBox.Show("Pedido excluído com sucesso.", InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AlertHandler(string s, MessageBoxImage messageBoxImage)
        {
            MessageBox.Show(s, InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, messageBoxImage);
        }
        private void ProcessoInicializado()
        {
            this.StartWait();
        }

        private void ProcessoFinalizado()
        {
            this.StopWait();
        }

        private void Pedidos_OnLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnImprimir_OnClick(object sender, RoutedEventArgs e)
        {
            _mvvm.ImprimirPedido((Entity.Pedido)(sender as Button).DataContext);
        }

        private void BtnIncluir_OnClick(object sender, RoutedEventArgs e)
        {
            _produtoPedido = (IncluirPedido)UIManager<IncluirPedido>.Show(this);
            _produtoPedido.OnPedidoAtualizado += PedidoAtualizado;
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _produtoPedido = (IncluirPedido)UIManager<IncluirPedido>.Show(this);
            _produtoPedido.PedidoCorrente = (Entity.Pedido)(sender as Expander).DataContext;
            _produtoPedido.OnPedidoAtualizado += PedidoAtualizado;
        }

        private void PedidoAtualizado()
        {
            _mvvm.DataDaEntrega = DtEntrega.SelectedDate.Value;
        }

        private void Pedido_OnClosing(object sender, CancelEventArgs e)
        {
            UIManager<Pedido>.Close(true);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnImprimirTodos_OnClick(object sender, RoutedEventArgs e)
        {
            _mvvm.ImprimirTodosOsPedidos();
        }

        private void OnDeletePedido(object sender, RoutedEventArgs e)
        {
            _mvvm.ExcluirPedido((Entity.Pedido)(sender as Button).DataContext);
        }
    }
}
