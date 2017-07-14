using System.Windows;
using System.Windows.Input;

namespace Syslaps.Pdv.UI
{
    public static class ExtensionUI
    {
        private static string tempTitleWait;
        public static void StartWait(this Window window, string title = "Aguarde...")
        {
            tempTitleWait = window.Title;
            window.Title = title;
            Mouse.OverrideCursor = Cursors.Wait;
            window.IsEnabled = false;
        }

        public static void StopWait(this Window window)
        {
            window.Title = tempTitleWait;
            Mouse.OverrideCursor = Cursors.Arrow;
            window.IsEnabled = true;
        }
    }
}
