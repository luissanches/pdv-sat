using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Syslaps.Pdv.Core.Dominio.Base;
using Syslaps.Pdv.Cross;
using Caixa = Syslaps.Pdv.Core.Dominio.Caixa.Caixa;
using Syslaps.Pdv.Core.Dominio.Impressora;
using System.Configuration;

namespace Syslaps.Pdv.UI
{
    public partial class App : Application
    {
        
        protected override void OnStartup(StartupEventArgs e)
        {
            InstanceManager.Logger = ContainerIoc.GetInstance<IInfraLogger>().Log();
            log4net.Config.XmlConfigurator.Configure();
            ContainerIoc.SetDefaultConstructorParameter<Caixa, string>("nomeDoCaixa".GetConfigValue(), "nomeDoCaixa");
            // Select the text in a TextBox when it receives focus.
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.PreviewMouseLeftButtonDownEvent,
                new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotKeyboardFocusEvent,
                new RoutedEventHandler(SelectAllText));
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.MouseDoubleClickEvent,
                new RoutedEventHandler(SelectAllText));

            InstanceManager.Cupom = ContainerIoc.GetInstance<Cupom>();

            CultureInfo culture = new CultureInfo(ConfigurationManager.AppSettings["Cultura"]);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            base.OnStartup(e);
        }
        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            // Find the TextBox
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent != null)
            {
                var textBox = (TextBox)parent;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }
        private void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
                textBox.SelectAll();
        }


    }
}
