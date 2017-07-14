using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Syslaps.Pdv.Core;
using Syslaps.Pdv.Core.Dominio.Produto;

namespace Syslaps.Pdv.UI.Telas.Alertas
{
    public partial class ExecutarProcessoImportarProduto : Window
    {
        public class ExecutarProcessoImportarProdutoMvvm : INotifyPropertyChanged
        {

            private string _nomeDoItemProcessado;

            private int _qtdeItensParaProcessar;

            private int _indiceDoItemProcessado;

            public string NomeDoItemProcessado
            {
                get
                {
                    return _nomeDoItemProcessado;
                }

                set
                {
                    _nomeDoItemProcessado = value;
                    OnPropertyChanged();
                }
            }

            public int QtdeItensParaProcessar
            {
                get
                {
                    return _qtdeItensParaProcessar;
                }

                set
                {
                    _qtdeItensParaProcessar = value;
                    OnPropertyChanged();
                }
            }

            public int IndiceDoItemProcessado
            {
                get
                {
                    return _indiceDoItemProcessado;
                }

                set
                {
                    _indiceDoItemProcessado = value;
                    OnPropertyChanged();
                }
            }

            public string TituloDoProcesso { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ExecutarProcessoImportarProdutoMvvm _mvvm;

        public ExecutarProcessoImportarProduto()
        {
            InitializeComponent();
            _mvvm = new ExecutarProcessoImportarProdutoMvvm { TituloDoProcesso = "Importando produdos da planilha...", IndiceDoItemProcessado = 0, QtdeItensParaProcessar = 100 };
            DataContext = _mvvm;
        }

        private void ProdutoImportadoHandler(int indice, int total, string produto)
        {
            _mvvm.IndiceDoItemProcessado = indice;
            _mvvm.QtdeItensParaProcessar = total;
            _mvvm.NomeDoItemProcessado = $"Produto Importado: {produto}";
        }

        private void ExecutarProcesso_OnLoaded(object sender, RoutedEventArgs e)
        {
            var bootstrap = ContainerIoc.GetInstance<Bootstrap>();
            bootstrap.ProdutoImportadoHandler += ProdutoImportadoHandler;
            bootstrap.ImportacaoConcluidaHandler += (ex) =>
            {
                if (Dispatcher.CheckAccess())
                {
                    InstanceManager.ListaDeProdutosDoPdv = ContainerIoc.GetInstance<Produto>().RecuperarListaDeProdutosDoPdv();
                    AlertarErro(ex);
                    DialogResult = true;
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        InstanceManager.ListaDeProdutosDoPdv = ContainerIoc.GetInstance<Produto>().RecuperarListaDeProdutosDoPdv();
                        AlertarErro(ex);
                        DialogResult = true;
                    });
                }
            };
            Task.Factory.StartNew(() =>
            {
                bootstrap.ImportarProdutos();
            });

        }

        private void AlertarErro(Exception ex)
        {
            if (ex != null)
            {
                MessageBox.Show(ex.Message, InstanceManager.Parametros.TituloDasMensagens,
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
        }
    }
}
