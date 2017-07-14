using System.Windows;

namespace Syslaps.Pdv.UI.Telas.Base
{
    public class BaseWindow : Window
    {
        public State CurrentState { get; private set; }
    
        protected new void Close()
        {
            CurrentState = State.Closed;
            base.Close();
        }
        protected new void Show()
        {
            CurrentState = State.Opened;
            base.Show();
        }
        protected new void Hide()
        {
            CurrentState = State.Hidden;
            base.Hide();
        }
        public enum State
        {
            Opened,
            Hidden,
            Closed
        }
    }
}
