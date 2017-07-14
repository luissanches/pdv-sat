using Syslaps.Pdv.Cross;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Syslaps.Pdv.UI.Telas.Financeiro
{
    public partial class ExportarNotaFiscal : Window
    {
        private ExportarNotaFiscalMvvm _mvvm;

        public ExportarNotaFiscal()
        {
            InitializeComponent();
            _mvvm = ContainerIoc.GetInstance<ExportarNotaFiscalMvvm>();
            DataContext = _mvvm;
        }

        private void BtnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            this.StartWait();
            try
            {
                _mvvm.ConsultarCuponsPorData();
                ExportXml();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Error);
                ExportXml();
            }
            finally
            {
                this.StopWait();
            }
        }

        private void ExportXml()
        {
            if (_mvvm.ListaDeCupomSat.Count > 0)
            {
                var dialog = new FolderBrowserDialog();
                dialog.ShowDialog();
                if (!dialog.SelectedPath.IsNullOrEmpty())
                {
                    var path = dialog.SelectedPath;
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                        Directory.CreateDirectory(path);
                    }

                    if (!path.IsNullOrEmpty())
                    {
                        _mvvm.ListaDeCupomSat.ForEach((item) =>
                        {
                            if (item.ErrorCode2 == "0000")
                            {
                                var fileName = string.Concat(path, "\\SAT_", item.DataOperacao.ToString("yyyymmdd"), "_000_", item.CodigoVenda, ".xml");

                                if (File.Exists(fileName))
                                    File.Delete(fileName);

                                using (StreamWriter sw = File.CreateText(fileName))
                                {
                                    sw.WriteLine(item.Xml);
                                    sw.Close();
                                }
                            }

                        });

                    }

                    var zipPath = string.Concat(path, ".zip");
                    if (File.Exists(zipPath))
                        File.Delete(zipPath);

                    Infra.Compress.ZipDirectory(path, zipPath);

                    System.Windows.MessageBox.Show("Arquivos exportados com sucesso.", InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Nenhuma cupom SAT fiscal neste período.", InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnGerarArquivo_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
