using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Syslaps.Pdv.UI.Telas.Alertas
{
    public partial class InputBox : Window
    {

        private void BtnOk_OnClick(object sender, RoutedEventArgs e)
        {
            if (TxtValue.Text.Length > 0)
            {
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("O valor deve ser preenchido.", InstanceManager.Parametros.TituloDasMensagens, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            
        }

        public InputBox(string title = "Caixa de Valor", string description = "Insira o valor:")
        {
            InitializeComponent();
            Title = title;
            LblDescription.Content = description;

        }


        private void TxtValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text[0]) && e.Text != ",")
                e.Handled = true;
            else
            {
                e.Handled = false;
            }
        }

        private void InputBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            TxtValue.Focus();
        }
    }
}
