using System;
using System.ComponentModel;
using System.Windows;
using Bolaria.WpfUI.UI;
using Syslaps.Pdv.Core.Dominio.Produto;
using Syslaps.Pdv.Cross;

namespace Syslaps.Pdv.UI.Telas.Producao
{
    public partial class ConsultarProducao : Window
    {
        public ConsultarProducao()
        {
            InitializeComponent();
            DtInicio.SelectedDate = DateTime.Now.GetFirstDateTimeOfMonth();
            DtFim.SelectedDate = DateTime.Now;
            BtnFiltrar_OnClick(null, null);
        }

        private void ConsultarProducao_OnClosing(object sender, CancelEventArgs e)
        {
            UIManager<ConsultarProducao>.Close(true);
        }

        private void BtnOk_OnClick(object sender, RoutedEventArgs e)
        {
           this.Close();
        }

        private void BtnFiltrar_OnClick(object sender, RoutedEventArgs e)
        {
            this.StartWait();
            try
            {
                if (!DtInicio.SelectedDate.HasValue)
                {
                    MessageBox.Show("Data inicial deve ser preenchida.", InstanceManager.Parametros.TituloDasMensagens,
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (!DtFim.SelectedDate.HasValue)
                {
                    MessageBox.Show("Data final deve ser preenchida.", InstanceManager.Parametros.TituloDasMensagens,
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (DtFim.SelectedDate.Value < DtInicio.SelectedDate.Value)
                {
                    MessageBox.Show("Data final deve ser maior que a data inicial.",
                        InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var dominio = ContainerIoc.GetInstance<Core.Dominio.Producao.Producao>();
                var mvvm = dominio.RecuperarConsultaProducao(DtInicio.SelectedDate.Value, DtFim.SelectedDate.Value);
                DataContext = mvvm;
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

        private void ConsultarProducao_OnLoaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
