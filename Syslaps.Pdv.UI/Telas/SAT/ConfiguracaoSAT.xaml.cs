using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using Syslaps.Pdv.Cross;
using Syslaps.Pdv.Entity.SAT;
using Syslaps.Pdv.Infra.Repositorio;
using Syslaps.Pdv.Infra.SAT;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;
using System.Threading.Tasks;

namespace Syslaps.Pdv.UI.Telas.SAT
{
    /// <summary>
    /// Interaction logic for ConfiguracaoSAT.xaml
    /// </summary>
    public partial class ConfiguracaoSAT : Window
    {
        public ConfiguracaoSAT()
        {
            InitializeComponent();
            this.Title = InstanceManager.Parametros.TituloNoConfig;
        }

        private string PegarPathArq()
        {
            OpenFileDialog abrir = new OpenFileDialog();
            abrir.InitialDirectory = "";

            abrir.Filter = "txt (*.txt)|*.txt| xml (*.xml)|*.xml";
            abrir.Title = "Selecione um Arquivo XML ou TXT";
            abrir.CheckFileExists = true;
            abrir.CheckPathExists = true;
            abrir.FilterIndex = 2;

            if (abrir.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {


                return abrir.FileName;


            }
            else
            {
                return "Cancelado";
            }

        }

        private string LerArqTxt(string NomeArq)
        {

            try
            {
                StreamReader arq = new StreamReader(NomeArq);


                NomeArq = arq.ReadToEnd();


                arq.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERRO: " + ex.ToString(), "Erro");
                return "";
            }

            return ConverterToUTF8(NomeArq);

        }

        private string ConverterToUTF8(string dados)  // sempre mandar os dados para o sat em UT8
        {
            byte[] utf16Bytes = Encoding.Unicode.GetBytes(dados);
            byte[] utf8Bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16Bytes);

            return Encoding.Default.GetString(utf8Bytes);
        }

        private void BtnStatus_Click(object sender, RoutedEventArgs e)
        {
            this.StartWait("Aguarde. Comunicando com SAT...");
            var tsat = Task.Run(() =>
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        StatusValidate();
                    });
                    
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Concat("Falha ao se comunicar com SAT.\nMensagem: ", ex.Message), InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
            tsat.Wait(5000);
            this.StopWait();
            //MessageBox.Show("Não foi possível estabelecer comunicação com SAT.");
        }

        private void StatusValidate()
        {
            SatModelEnum modeloSat =
                     (SatModelEnum)Enum.Parse(typeof(SatModelEnum), InstanceManager.Parametros.ModeloSat);
            var sat =
                new Syslaps.Pdv.Core.Dominio.SAT.Sat(
                    SatBase.Create(InstanceManager.Parametros.CodigoSat, modeloSat), new RepositorioBase(), InstanceManager.Parametros);

            var response = sat.VerificarStatus();
            var response2 = sat.VerificarDisponibilidade();

            TxtResutadoStatus.Text = $@"Status: {response.ErrorMessage}
SAT Number: {InstanceManager.Parametros.NumeroSat}
Serial Number: {response.SerialNumber}
Software Version: {response.SoftwareVersion}
Certificate ExpirationDate: {response.CertificateExpirationDate}
Date: {response.DateTime}
LanStatus: {response.LanStatus}
LastCfeSent: {response.LastCfeSent}
LastComunicationDate: {response.LastComunicationDate}
LastTransmissionDate: {response.LastTransmissionDate}
Memory: {response.MemoryUsed} de {response.MemoryTotal}
LayoutVersion: {response.LayoutVersion}
Diponibilidade: {response2.RawResponse}
";
        }

        private void BtnSendXml_Click(object sender, RoutedEventArgs e)
        {
            this.StartWait("Aguarde. Comunicando com SAT...");
            var tsat = Task.Run(() =>
            {
                try
                {
                    var cupomXml = LerArqTxt(PegarPathArq());
                    if (!cupomXml.IsNullOrEmpty())
                    {
                        SatModelEnum modeloSat =
                             (SatModelEnum)Enum.Parse(typeof(SatModelEnum), InstanceManager.Parametros.ModeloSat);
                        var sat =
                            new Syslaps.Pdv.Core.Dominio.SAT.Sat(
                                SatBase.Create(InstanceManager.Parametros.CodigoSat, modeloSat), new RepositorioBase(), InstanceManager.Parametros);

                        var retSat = sat.EnviarVenda(cupomXml);
                        MessageBox.Show(retSat.RawResponse);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Concat("Falha ao se comunicar com SAT.\nMensagem: ", ex.Message), InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
            tsat.Wait(10000);
            this.StopWait();
        }
    }
}
