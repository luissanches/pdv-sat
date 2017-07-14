using System;
using System.Threading.Tasks;
using System.Windows;
using Bolaria.WpfUI.UI;

namespace Syslaps.Pdv.UI.Telas.Producao
{
    public partial class Producao : Window
    {
        private ProducaoMvvm mvvm;
        public Producao()
        {
            InitializeComponent();
            this.Title = InstanceManager.Parametros.TituloNoConfig;
            mvvm = ContainerIoc.GetInstance<ProducaoMvvm>();
            mvvm.DataDeProducao = DateTime.Now;
            DataContext = mvvm;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UIManager<Producao>.Close(true);
        }

        private void BtnConsultarProducao_OnClick(object sender, RoutedEventArgs e)
        {
            (new ConsultarProducao()).ShowDialog();
        }

        private void Producao_OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
