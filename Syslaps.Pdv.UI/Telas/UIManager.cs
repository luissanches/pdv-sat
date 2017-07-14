using System;
using System.Collections.Generic;
using System.Windows;

namespace Bolaria.WpfUI.UI
{
    public static class UIManager<TWindow> where TWindow : Window
    {
        static UIManager()
        {
            containers = new List<UIContainer>();
        }

        private static List<UIContainer> containers;

        public static Window Show(Window owner = null)
        {
            foreach (var container in containers)
            {
                if (!(container.CurrentWindow is TWindow)) continue;

                switch (container.CurrentState)
                {
                    case UIContainerState.Hidden:
                        container.CurrentState = UIContainerState.Opened;
                        container.CurrentWindow.Show();
                        return container.CurrentWindow;
                    case UIContainerState.Opened:
                        container.CurrentWindow.WindowState = WindowState.Maximized;
                        container.CurrentWindow.Focus();
                        return container.CurrentWindow;
                }
            }

            var window = (Window)Activator.CreateInstance(typeof(TWindow));

            if (owner != null)
                window.Owner = owner;

            containers.Add(new UIContainer() { CurrentState = UIContainerState.Opened, CurrentWindow = window });
            window.Show();

            return window;

        }

        public static void Hide()
        {
            containers.ForEach(container =>
            {
                if (container.CurrentWindow is TWindow)
                {
                    container.CurrentState = UIContainerState.Hidden;
                    container.CurrentWindow.Hide();
                }
            });
        }

        public static void Close(bool jaEstaFechada = false)
        {
            var clone = new List<UIContainer>(containers);
            clone.ForEach(container =>
            {
                if (container.CurrentWindow is TWindow)
                {
                    if (!jaEstaFechada)
                        container.CurrentWindow.Close();

                    containers.Remove(container);
                }
            });
        }
    }

    public class UIContainer
    {
        public Window CurrentWindow { get; set; }
        public UIContainerState CurrentState { get; set; }
    }

    public enum UIContainerState
    {
        Opened,
        Hidden,
        Closed
    }

}
