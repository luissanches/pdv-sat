using Bolaria.WpfUI.UI;
using Syslaps.Pdv.Core.Dominio.Impressora;
using Syslaps.Pdv.Cross;
using Syslaps.Pdv.Entity;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Caixa = Syslaps.Pdv.Core.Dominio.Caixa.Caixa;
using Produto = Syslaps.Pdv.Entity.Produto;
using System.Linq;
using System.Diagnostics;
using System.Configuration;
using System.IO;

namespace Syslaps.Pdv.UI.Telas.PDV
{
    public partial class PontoDeVenda : Window
    {
        private readonly PontoDeVendaMvvm _mvvm;

        public PontoDeVenda()
        {
            InitializeComponent();
            this.Title = InstanceManager.Parametros.TituloNoConfig;
            if (InstanceManager.CaixaCorrente == null)
            {
                InstanceManager.CaixaCorrente = ContainerIoc.GetInstance<Caixa>();
                ContainerIoc.containerIoc.Configure(ioc =>
                {
                    ioc.ForConcreteType<Core.Dominio.Venda.Venda>().Configure.Ctor<Entity.Caixa>().Is(InstanceManager.CaixaCorrente.CaixaCorrente);
                });
            }

            _mvvm = ContainerIoc.GetInstance<PontoDeVendaMvvm>();
            DataContext = _mvvm;
            _mvvm.PodeInicializarVenda = _mvvm.CaixaEstaAberto;
        }

        private void BtnAbrirFecharCaixa_OnClick(object sender, RoutedEventArgs e)
        {
            if (InstanceManager.Parametros.GavetaAutomatica)
                InstanceManager.Cupom.AbrirGaveta();
            var aberturaFechamento = _mvvm.CaixaEstaAberto ? "Fechamento" : "Abertura";
            var msgBox = new Alertas.InputBox($"{aberturaFechamento} do Caixa", $"Insira o valor de {aberturaFechamento}:");
            if (msgBox.ShowDialog().Value)
            {
                var valorDaOperacao = msgBox.TxtValue.Text.ToDecimal();
                if (_mvvm.CaixaEstaAberto)
                {
                    _mvvm.CaixaCorrente.FecharCaixa(valorDaOperacao);
                }
                else
                {
                    _mvvm.CaixaCorrente.AbrirCaixa(valorDaOperacao);
                }
            }
        }

        private void BtnInicializarVenda_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _mvvm.VendaCorrente = ContainerIoc.GetInstance<Core.Dominio.Venda.Venda>();
                (new Alertas.DadosConsumidor(_mvvm)).ShowDialog();
                this.StartWait();
                _mvvm.PodeInicializarVenda = false;
                BtnAbrirFecharCaixa.IsEnabled = false;
                BtnCancelarVenda.Visibility = Visibility.Visible;
                BtnIncluirCpf.Visibility = Visibility.Visible;
                TxtNrComanda.Visibility = Visibility.Visible;
                LblNrComanda.Visibility = Visibility.Visible;
                PainelProdutosDaVenda.Width = new GridLength(480);
                StpTotal.Visibility = Visibility.Visible;
                _mvvm.NotifyPropertyChanged("ValorTotalDaVenda");
                TxtProduto.Text = "";
                SendKey(Key.Space);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.StopWait();
                TxtProduto.Focus();
            }
        }

        private void BtnFechar_OnClick(object sender, RoutedEventArgs e)
        {
            UIManager<PontoDeVenda>.Hide();
        }

        private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text[0]) && e.Text != ",")
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private async void ButtonAdicionarProduto_OnClick(object sender, RoutedEventArgs e)
        {
            this.StartWait();
            try
            {
                if (_mvvm.VendaCorrente != null)
                {
                    var produto = (Produto)(sender as Button).DataContext;
                    var qtde = (((sender as Button).Parent as StackPanel).Children[1] as TextBox).Text.ToDecimal();
                    await Task.Factory.StartNew(() =>
                    {
                        _mvvm.AdicionarVendaProduto(produto, qtde);
                    });

                    TxtProduto.SelectAll();
                }
                else
                {
                    MessageBox.Show("Venda não inicializada.", InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                InstanceManager.Logger.Error(ex);
            }
            finally
            {
                this.StopWait();
                TxtProduto.SelectAll();
            }
        }

        private void PontoDeVenda_OnLoaded(object sender, RoutedEventArgs e)
        {
            TxtProduto.Focus();
        }

        private void ButtonRemoverProduto_OnClick(object sender, RoutedEventArgs e)
        {
            if (_mvvm.VendaCorrente != null)
            {
                this.StartWait();
                try
                {
                    var vendaProduto = (VendaProduto)(sender as Button).DataContext;
                    _mvvm.RemoverVendaProduto(vendaProduto);
                }
                finally
                {
                    this.StopWait();
                }
            }
            else
            {
                MessageBox.Show("Venda não inicializada.", InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TxtVendaProdutoValor_OnLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var qtde = (((sender as TextBox).Parent as WrapPanel).Children[1] as TextBox).Text.ToDecimal();
                var valor = (((sender as TextBox).Parent as WrapPanel).Children[3] as TextBox).Text.ToDecimal();
                var vendaProduto = (VendaProduto)(sender as TextBox).DataContext;
                _mvvm.AtualizarVendaProduto(vendaProduto, valor, qtde);
            }
            catch (Exception ex)
            {
                if (ex.HResult != -2147467262)
                    MessageBox.Show(string.Concat("Falha na operação.\nMensagem: ", ex.Message), InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void BtnCancelarVenda_OnClick(object sender, RoutedEventArgs e)
        {
            if (
                MessageBox.Show("Deseja Cancelar a Venda?", InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                FecharVenda();
            }
        }

        private void FecharVenda()
        {
            this.StartWait();
            try
            {
                _mvvm.PodeInicializarVenda = true;
                _mvvm.NumeroComanda = null;
                BtnAbrirFecharCaixa.IsEnabled = true;
                BtnCancelarVenda.Visibility = Visibility.Collapsed;
                BtnIncluirCpf.Visibility = Visibility.Collapsed;
                TxtNrComanda.Visibility = Visibility.Collapsed;
                LblNrComanda.Visibility = Visibility.Collapsed;
                PainelProdutosDaVenda.Width = new GridLength(0);
                StpTotal.Visibility = Visibility.Collapsed;
                _mvvm.NotifyPropertyChanged("ValorTotalDaVenda");
                _mvvm.FinalizarVenda();
                TxtProduto.Text = "";
                TxtNrComanda.Text = "";
                BtnInicializarVenda.Focus();
            }
            finally
            {
                this.StopWait();
            }
        }

        private void BtnEfetuarPagamento_OnClick(object sender, RoutedEventArgs e)
        {
            var msgBox = new Alertas.Pagamentos(_mvvm);
            if (msgBox.ShowDialog().Value)
            {
                var hasMoney = _mvvm.ListaDeVendaPagamentos.Count(x => x.TipoPagamento.Nome.Contains("Dinheiro")) > 0;
                if (InstanceManager.Parametros.GavetaAutomatica && hasMoney)
                {
                    InstanceManager.Cupom.AbrirGaveta();
                }

                var msgCupom = "Venda Finalizada. Deseja imprimir Cupom?";
                if(_mvvm.VendaCorrente.VendaCorrente.ValorTroco > 0)
                {
                    msgCupom = $"Venda Finalizada. Troco: {_mvvm.VendaCorrente.VendaCorrente.ValorTroco.ToString("C")}. Deseja imprimir Cupom ?";
                }

                if (MessageBox.Show(msgCupom, InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.YesNo,
                    MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    this.StartWait("Aguarde. Imprimindo Cupom...");
                    var tprint = Task.Run(() =>
                    {
                        try
                        {
                            _mvvm.VendaCorrente.VendaCorrente.CupomFiscalImpresso = true;
                            _mvvm.RegistrarCupomFiscalSat();
                            ContainerIoc.GetInstance<Cupom>().ImprimirVenda(_mvvm.VendaCorrente.VendaCorrente);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Concat("Falha.\nMensagem: ", ex.Message), InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    });

                    tprint.Wait(10000);
                    this.StopWait();
                }
                else
                {
                    this.StartWait("Aguarde. Comunicando com SAT...");
                    var tsat = Task.Run(() =>
                    {
                        try
                        {
                            _mvvm.RegistrarCupomFiscalSat();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Concat("Falha ao se comunicar com SAT.\nMensagem: ", ex.Message), InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    });
                    tsat.Wait(10000);
                    this.StopWait();
                }

                try
                {
                    FecharVenda();
                    SincronizarVenda();
                }
                finally
                {
                    this.StopWait();
                }
            }
        }

        private void SincronizarVenda()
        {
            var pathBat = ConfigurationManager.AppSettings["CaminhoFisicoDoBatSincronizador"];
            if (pathBat != null && File.Exists(pathBat))
            {
                var processInfo = new ProcessStartInfo("cmd.exe", "/c " + pathBat);
                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;
                processInfo.WorkingDirectory = Path.GetDirectoryName(pathBat);
                var process = Process.Start(processInfo);
                //process.WaitForExit();
                process.Close();
                process.Dispose();
            }

            
        }

        private void PontoDeVenda_OnKeyDown(object sender, KeyEventArgs e)
        {
            Key k = e.SystemKey == Key.None ? e.Key : e.SystemKey;

            switch (k)
            {
                case Key.F10:
                    {
                        if (_mvvm.VendaPossuiProdutos && !_mvvm.PodeInicializarVenda)
                        {
                            BtnEfetuarPagamento_OnClick(null, null);
                        }
                        else if (_mvvm.PodeInicializarVenda)
                        {
                            BtnInicializarVenda_OnClick(null, null);
                        }
                        break;
                    }

                case Key.F9:
                    {
                        if (_mvvm.VendaPossuiProdutos && !_mvvm.PodeInicializarVenda)
                            BtnSalvarComanda_OnClick(null, null);
                        break;
                    }
                case Key.F8:
                    {
                        BtnIncluirCpf_Click(null, null);
                        break;
                    }

                case Key.Escape:
                    {
                        if (!_mvvm.PodeInicializarVenda)
                            BtnCancelarVenda_OnClick(null, null);
                        break;
                    }
            }

            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
            {
                switch (e.Key)
                {
                    case Key.P:
                        ImprimirComanda();
                        break;
                    case Key.D1:
                        AutoAdd(0);
                        break;
                    case Key.D2:
                        AutoAdd(1);
                        break;
                    case Key.D3:
                        AutoAdd(2);
                        break;
                    case Key.D4:
                        AutoAdd(3);
                        break;
                    case Key.D5:
                        AutoAdd(4);
                        break;
                    case Key.D6:
                        AutoAdd(5);
                        break;
                    case Key.D7:
                        AutoAdd(6);
                        break;
                    case Key.D8:
                        AutoAdd(7);
                        break;
                    case Key.D9:
                        AutoAdd(8);
                        break;
                    case Key.D0:
                        AutoAdd(9);
                        break;
                }

            }
        }

        private void PontoDeVenda_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            UIManager<PontoDeVenda>.Hide();
        }

        private void BtnSalvarComanda_OnClick(object sender, RoutedEventArgs e)
        {
            if (_mvvm.NumeroComanda.IsNullOrEmpty())
            {
                MessageBox.Show("Necessário digitar o número da comanda.", InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtNrComanda.Focus();
            }
            else
            {
                _mvvm.SalvarComanda();
                MessageBox.Show("Comanda salva com sucesso.", InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Information);
                FecharVenda();
            }

        }

        private void TxtNrComanda_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CarregarComanda();
            }

        }

        private void TxtNrComanda_OnLostFocus(object sender, RoutedEventArgs e)
        {
            //CarregarComanda();
        }

        private void CarregarComanda()
        {
            _mvvm.CarregarComanda(TxtNrComanda.Text);
            TxtProduto.Focus();
        }


        private void ImprimirComanda()
        {
            if (MessageBox.Show("Deseja imprimir Comanda?",
                InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.YesNo,
                MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                ContainerIoc.GetInstance<Cupom>().ImprimirComanda(_mvvm.VendaCorrente.VendaCorrente);
            }
        }

        private void AutoAdd(int indx)
        {
            if (indx <= LstProdutosFiltrados.Items.Count)
            {
                LstProdutosFiltrados.SelectedIndex = indx;
                ListBoxItem myListBoxItem =
                    (ListBoxItem)
                        (LstProdutosFiltrados.ItemContainerGenerator.ContainerFromIndex(
                            LstProdutosFiltrados.SelectedIndex));
                ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);
                DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
                Button target = (Button)myDataTemplate.FindName("BtnAddProduto", myContentPresenter);
                ButtonAdicionarProduto_OnClick(target, null);
            }
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
    where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void TxtVendaProdutoQuantidade_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var qtde = (((sender as TextBox).Parent as WrapPanel).Children[1] as TextBox).Text.ToDecimal();
                var valor = (((sender as TextBox).Parent as WrapPanel).Children[3] as TextBox).Text.ToDecimal();
                var vendaProduto = (VendaProduto)(sender as TextBox).DataContext;
                _mvvm.AtualizarVendaProduto(vendaProduto, valor, qtde);
            }
        }

        private void BtnIncluirCpf_Click(object sender, RoutedEventArgs e)
        {
            if (!_mvvm.PodeInicializarVenda)
                (new Alertas.DadosConsumidor(_mvvm)).ShowDialog();
        }

        public static void SendKey(Key key)
        {
            if (Keyboard.PrimaryDevice != null)
            {
                if (Keyboard.PrimaryDevice.ActiveSource != null)
                {
                    var e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key)
                    {
                        RoutedEvent = Keyboard.KeyDownEvent
                    };
                    InputManager.Current.ProcessInput(e);

                    // Note: Based on your requirements you may also need to fire events for:
                    // RoutedEvent = Keyboard.PreviewKeyDownEvent
                    // RoutedEvent = Keyboard.KeyUpEvent
                    // RoutedEvent = Keyboard.PreviewKeyUpEvent
                }
            }
        }
    }
}
