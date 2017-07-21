using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MasTicket
{
    public class nipViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand ClickedCommand { get; private set; }
        //public bool EsCorrecto { get; private set; }
        private string _nip = string.Empty;
        public string CambioNip { get { return (_nip); } }
        
        public nipViewModel()
        {
            ClickedCommand = new Command<string>(ChecaNip);
        }

        void ChecaNip(string n)
        {
            if (_nip.Trim().Length == 4)
                _nip = "";
            _nip += n;
            OnPropertyChanged("CambioNip");
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}