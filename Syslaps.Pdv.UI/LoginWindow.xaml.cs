using System;
using System.Threading.Tasks;
using System.Windows;
using Bolaria.WpfUI.UI;
using Syslaps.Pdv.Core;
using Syslaps.Pdv.Core.Dominio;
using Syslaps.Pdv.Cross;
using Syslaps.Pdv.Entity;
using Syslaps.Pdv.Infra.Repositorio;
using Caixa = Syslaps.Pdv.Core.Dominio.Caixa.Caixa;
using Produto = Syslaps.Pdv.Core.Dominio.Produto.Produto;
using TelaPrincipal = Syslaps.Pdv.UI.Telas.TelaPrincipal;
using Usuario = Syslaps.Pdv.Core.Dominio.Usuario.Usuario;
using System.Configuration;

namespace Syslaps.Pdv.UI
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            this.Title = ConfigurationManager.AppSettings["TituloInicial"];
            lblTitle.Content = ConfigurationManager.AppSettings["TituloInicial"];

            txtUsuario.Focus();


#if DEBUG
            txtUsuario.Text = "admin";
            txtSenha.Password = "123";
            //btnEntrar_Click(null, null);
#endif
        }

        private void btnSair_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Application.Current.Shutdown();
        }

        private async void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.StartWait();
                if (txtUsuario.Text.Length == 0 || txtSenha.Password.Length == 0) return;
                var login = txtUsuario.Text;
                var senha = txtSenha.Password;
                var task = Task.Factory.StartNew(() =>
                {
                    InstanceManager.UsuarioCorrente = ContainerIoc.GetInstance<Usuario>();
                    InstanceManager.UsuarioCorrente.LogarUsuario(login, senha);
                });

                await task;

                if (InstanceManager.UsuarioCorrente.Status == EnumStatusDoResultado.MensagemDeSucesso)
                {
                    ContainerIoc.containerIoc.Configure(ioc =>
                    {
                        ioc.ForConcreteType<Caixa>().Configure.Ctor<string>("nomeDoCaixa").Is("NomeDoCaixa".GetConfigValue()).Ctor<Entity.Usuario>().Is(InstanceManager.UsuarioCorrente.UsuarioLogado);
                    });

                    InstanceManager.ListaDeProdutosDoPdv = ContainerIoc.GetInstance<Produto>().RecuperarListaDeProdutosDoPdvPoTipo(RdoTp2.IsChecked.Value ? 2 : 1);

                    UIManager<TelaPrincipal>.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show(InstanceManager.UsuarioCorrente.Mensagem, InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                InstanceManager.Logger.Error(ex);
                MessageBox.Show(ex.Message, InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK,MessageBoxImage.Error);
            }
            finally
            {
                this.StopWait();
            }
        }

        private void LoginWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                var repositorio = ContainerIoc.GetInstance<RepositorioBase>();
                repositorio.LimparLogDaBase();
                
                InstanceManager.ListaDeTipoPagamentos = repositorio.RecuperarTodos<TipoPagamento>();
                InstanceManager.Parametros = ContainerIoc.GetInstance<Parametros>();
                InstanceManager.Logger.Info("Login Loaded.....");
            });
        }
        
    }
}
