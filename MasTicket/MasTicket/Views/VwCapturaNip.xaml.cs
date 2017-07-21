using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace MasTicket
{
    public partial class VwCapturaNip : ContentView
    {
        
        public void SetFocus()
        {
            edtPwd1.Focus();
        }

        public event EventHandler<NippedEventArgs> Nipped;
        protected virtual void OnNipped(NippedEventArgs e)
        {
            var handler = Nipped;
            if (handler != null) handler(this, e);
        }

        private static void OnNipPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var popup = (VwCapturaNip)bindable;
            popup.Nip = (string)newValue;
        }
        public static readonly BindableProperty NipProperty = BindableProperty.Create("Nip", typeof(string), typeof(VwCapturaNip), "", propertyChanged: OnNipPropertyChanged);
        public string Nip
        {
            get { return (string)base.GetValue(NipProperty); }
            private set
            {
                base.SetValue(NipProperty, value);
            }
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(VwCapturaNip), null);
        public string Text
        {
            get { return (string)base.GetValue(TextProperty); }
            private set
            {
                base.SetValue(TextProperty, value);
            }
        }

        public VwCapturaNip()
        {
            InitializeComponent();

            grdMain.BindingContext = this;

            edtPwd1.Completed += (s, e) =>
            {
                edtPwd2.Focus();
            };
            edtPwd2.Completed += (s, e) =>
            {
                edtPwd3.Focus();
            };
            edtPwd3.Completed += (s, e) =>
            {
                edtPwd4.Focus();
            };
            //edtPwd4.Completed += (s, e) =>
            //{
            //    Nip = edtPwd1.Text + edtPwd2.Text + edtPwd3.Text + edtPwd4.Text;
            //    OnNipped(new NippedEventArgs() { nip = Nip });
            //};
        }

        protected void CambiaTxt(object e, TextChangedEventArgs ea)
        {
            Nip = edtPwd1.Text + edtPwd2.Text + edtPwd3.Text + edtPwd4.Text;
            OnNipped(new NippedEventArgs() { nip = Nip });
        }
    }

    public class NippedEventArgs : EventArgs
    {
        public string nip { get; set; }
    }
}
