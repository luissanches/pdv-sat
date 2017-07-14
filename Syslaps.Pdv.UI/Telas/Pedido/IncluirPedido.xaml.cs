using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Bolaria.WpfUI.UI;
using Syslaps.Pdv.Cross;
using Syslaps.Pdv.Entity;

namespace Syslaps.Pdv.UI.Telas.Pedido
{
    public partial class IncluirPedido : Window
    {
        private readonly IncluirPedidoMvvm _mvvm;
        private Entity.Pedido _pedido;
        public Action OnPedidoAtualizado;


        public IncluirPedido()
        {
            InitializeComponent();
            _mvvm = ContainerIoc.GetInstance<IncluirPedidoMvvm>();
            _mvvm.OnProcessoFinalizado += ProcessoFinalizado;
            _mvvm.OnProcessoInciado += ProcessoInicializado;
            _mvvm.OnPedidoRegistrado += PedidoRegistrado;
            _mvvm.OnAlert += AlertHandler;

            if(PedidoCorrente == null)
                PedidoCorrente = new Entity.Pedido { DataEntrega = DateTime.Now, DataPedido = DateTime.Now, Situacao = "aguardando"};


            DataContext = _mvvm;

        }

        private void PedidoRegistrado()
        {
            OnPedidoAtualizado?.Invoke();
            MessageBox.Show("Pedido registrado com sucesso.", InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        public Entity.Pedido PedidoCorrente
        {
            get { return _pedido; }
            set
            {
                _mvvm.PedidoCorrente = value;
                _pedido = value;
            }
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

        private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text[0]) && e.Text != ",")
                e.Handled = true;
            else
            {
                e.Handled = false;
            }
        }


        private void IncluirPedido_OnLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void OnAdicionarProdutoNoPedido(object sender, RoutedEventArgs e)
        {
            this.StartWait();
            try
            {
                var produto = (Entity.Produto) (sender as Button).DataContext;
                var qtde = (((sender as Button).Parent as StackPanel).Children[1] as TextBox).Text.ToLong().Value;
                _mvvm.AdicionarPedidoProduto(new PedidoProduto { Pedido = _mvvm.PedidoCorrente, Pedido_CodigoPedido = _mvvm.PedidoCorrente.CodigoPedido, Produto_CodigoDeBarra = produto.CodigoDeBarra, Quantidade = qtde, Produto = produto});
            }
            finally
            {
                this.StopWait();
                TxtProduto.Focus();
            }
        }

        private void OnRemoverProdutoDoPedido(object sender, RoutedEventArgs e)
        {
            this.StartWait();
            try
            {
                var pedidoProduto = (PedidoProduto)(sender as Button).DataContext;
                _mvvm.RemoverVendaProduto(pedidoProduto);
            }
            finally
            {
                this.StopWait();
            }
        }

        private void IncluirPedido_OnClosing(object sender, CancelEventArgs e)
        {
            UIManager<IncluirPedido>.Close(true);
            
        }

        

        private void TxtVendaProdutoQuantidade_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var pedidoProduto = (PedidoProduto)(sender as TextBox).DataContext;
            pedidoProduto.Quantidade = (((sender as TextBox).Parent as WrapPanel).Children[1] as TextBox).Text.ToInt();
            _mvvm.AtualizarQuantidadeDoPedidoProduto(pedidoProduto);
        }


        private void BtnCancelar_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}

