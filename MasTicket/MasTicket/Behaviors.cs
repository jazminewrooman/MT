using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;

using Xamarin.Forms;
using CreditCardValidator;

namespace MasTicket
{
	public static class check
	{
		public static bool ValidaNip(string nip)
		{
			bool ret = false; int inip = 0;
			string emailRegex = @"^(?!(.)\1{3})\d{4}$";
			if (!Regex.IsMatch(nip, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
				ret = false;
			else {
				emailRegex = @"^(?!(.)\1{2})\d{4}$";
				if (!Regex.IsMatch(nip, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
					ret = false;
				else {
					emailRegex = @"^(?!(.)\1{3})(?!19|20)(?!0123|1234|2345|3456|4567|5678|6789|7890|0987|9876|8765|7654|6543|5432|4321|3210)\d{4}$";
					if (!Regex.IsMatch(nip, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
						ret = false;
					else {
						if (int.TryParse(nip, out inip) && nip.Length == 4)
							ret = true;
					}
				}
			}
			return (ret);
		}

		public static string SafeSqlLiteral(string inputSQL)
		{
			//return inputSQL.Replace("'", "''");

			HashSet<char> removeChars = new HashSet<char>("?&^$#!()+-,:;<>’\'-*");
			StringBuilder result = new StringBuilder(inputSQL.Length);
			foreach (char c in inputSQL)
				if (!removeChars.Contains(c))
					result.Append(c);
			return result.ToString();
		}
	}

    public class CharValidationBehavior : Behavior<Entry>
    {
		public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create("MaxLength", typeof(int), typeof(MaxLengthValidator), 0);
		public static readonly BindableProperty NeedLengthProperty = BindableProperty.Create("NeedLength", typeof(bool), typeof(MaxLengthValidator), false);
		public int MaxLength
		{
			get { return (int)GetValue(MaxLengthProperty); }
			set { SetValue(MaxLengthProperty, value); }
		}

		protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }
        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }
        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
			if (MaxLength == 0)
				MaxLength = 1;
            MethodInfo baseEntrySendCompleted = null;
            int tmp = 0; bool isValid = false;
			if (args.NewTextValue.Length > MaxLength)
            {
				if (int.TryParse(args.NewTextValue.Substring(0, MaxLength), out tmp))
                    isValid = true;
                else
                    isValid = false;
            }
            else
            {
                if (int.TryParse(args.NewTextValue, out tmp))
                    isValid = true;
                else
                    isValid = false;
            }
			if (args.NewTextValue.Length == MaxLength)
			{
				((Entry)sender).Text = !isValid ? "" : tmp.ToString();
				if (isValid)
				{
/*#if __IOS__*/
					((IEntryController)sender).SendCompleted();
/*#endif
#if __ANDROID__
					Type baseEntry = sender.GetType();
					if (baseEntrySendCompleted == null)
						baseEntrySendCompleted = baseEntry.GetMethod("SendCompleted", BindingFlags.NonPublic | BindingFlags.Instance);
					baseEntrySendCompleted.Invoke(sender, null);
#endif*/
				}
			}
        }
    }
    public class NipValidationBehavior : Behavior<VwCapturaNip>
    {
        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(EmailValidationBehavior), false);
        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;
        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }

        protected override void OnAttachedTo(VwCapturaNip entry)
        {
            entry.Nipped += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }
        protected override void OnDetachingFrom(VwCapturaNip entry)
        {
            entry.Nipped -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }
        void OnEntryTextChanged(object sender, NippedEventArgs args)
        {
            int tmp = 0;
            if (args.nip.Length == 4)
            {
                if (int.TryParse(args.nip, out tmp))
                    IsValid = true;
                else
                    IsValid = false;
            }
            else
                IsValid = false;
                
            
        }
    }
    public class EmptyValidationBehavior : Behavior<Entry>
    {
        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(EmptyValidationBehavior), false);
        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;
        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }
        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }
        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            IsValid = !String.IsNullOrEmpty(args.NewTextValue);  //Double.TryParse (args.NewTextValue, out result);
            //((Entry)sender).BackgroundColor = IsValid ? Color.Default : Color.Red;
        }
    }
    public class DecimalValidationBehavior : Behavior<Entry>
    {
		static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(DecimalValidationBehavior), false);
		public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;
		public bool IsValid
		{
			get { return (bool)base.GetValue(IsValidProperty); }
			private set { base.SetValue(IsValidPropertyKey, value); }
		}

		static readonly BindablePropertyKey ValorPropertyKey = BindableProperty.CreateReadOnly("Valor", typeof(decimal), typeof(DecimalValidationBehavior), 0.0M);
		public static readonly BindableProperty ValorProperty = ValorPropertyKey.BindableProperty;
		public decimal Valor
		{
			get { return (decimal)base.GetValue(ValorProperty); }
			private set { base.SetValue(ValorPropertyKey, value); }
		}

		protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }
        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }
        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            decimal result;
            IsValid = decimal.TryParse(args.NewTextValue, out result);
			Valor = (IsValid) ? result : 0;
			((Entry)sender).Text = (IsValid ? result.ToString() : ""); //.BackgroundColor = (isValid && result > 0) ? Color.Default : Color.Red;
		}
    }
    public class IntValidationBehavior : Behavior<Entry>
    {
        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(IntValidationBehavior), false);
        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;
        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }

        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }
        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }
        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            int result;
			IsValid = int.TryParse(args.NewTextValue, out result);

			//((Entry)sender).TextChanged -= OnEntryTextChanged;
			((Entry)sender).Text = (IsValid ? args.NewTextValue : ""); //.BackgroundColor = (isValid && result > 0) ? Color.Default : Color.Red;
			//((Entry)sender).TextChanged += OnEntryTextChanged;
		}
    }
    public class EmailValidationBehavior : Behavior<Entry>
    {
        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(EmailValidationBehavior), false);

        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }
        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }
        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
            IsValid = (Regex.IsMatch(args.NewTextValue, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
            //((Entry)sender).BackgroundColor = isValid ? Color.Default : Color.Red;
        }
    }
    public class PhoneValidationBehavior : Behavior<Entry>
    {
        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(PhoneValidationBehavior), false);

        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }
        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }
        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            const string phoneRegex = @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}";


			string str = args.NewTextValue;
			str = Regex.Replace(str, @"\s+", "");
			//str = str.Trim();
			str = str.Replace("(", "");
			str = str.Replace(")", "");
			str = str.Replace("-", "");
			if (str.StartsWith("+"))
				str = str.Substring(1, str.Length - 1);
			//((Entry)sender).Text = str;

            if (str.Length > 0 && str.Length > 10)
                str = str.Substring(str.Length - 10, 10);

			((Entry)sender).TextChanged -= OnEntryTextChanged;
			((Entry)sender).Text = str;
			((Entry)sender).TextChanged += OnEntryTextChanged;

			IsValid = (str.Length == 10) && !String.IsNullOrEmpty(str) && (Regex.IsMatch(str, phoneRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));

			//((Entry)sender).BackgroundColor = (IsValid) ? Color.Default : Color.Red;
			//((Entry)sender).Style = IsValid ? (App.Current.Resources["EntryValido"] as Style) : (App.Current.Resources["EntryInvalido"] as Style);
		}
    }
	public class CardValidationBehavior : Behavior<Entry>
	{
        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(CardValidationBehavior), false);
		public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;
		public bool IsValid
		{
			get { return (bool)base.GetValue(IsValidProperty); }
			private set { base.SetValue(IsValidPropertyKey, value); }
		}
		protected override void OnAttachedTo(Entry entry)
		{
			entry.TextChanged += OnEntryTextChanged;
			base.OnAttachedTo(entry);
		}
		protected override void OnDetachingFrom(Entry entry)
		{
			entry.TextChanged -= OnEntryTextChanged;
			base.OnDetachingFrom(entry);
		}
		void OnEntryTextChanged(object sender, TextChangedEventArgs args)
		{
			CreditCardDetector det = null;
			if (!String.IsNullOrEmpty(args.NewTextValue))
			{
				det = new CreditCardDetector(args.NewTextValue);
				IsValid = det.IsValid();
				((Entry)sender).Text = det.CardNumber;
			}
			else
				IsValid = false;
		}
	}

	public class MaxLengthValidator : Behavior<Entry>
    {
        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(MaxLengthValidator), false);

        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }

        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create("MaxLength", typeof(int), typeof(MaxLengthValidator), 0);
        public static readonly BindableProperty NeedLengthProperty = BindableProperty.Create("NeedLength", typeof(bool), typeof(MaxLengthValidator), false);
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }
        public bool NeedLength
        {
            get { return (bool)GetValue(NeedLengthProperty); }
            set { SetValue(NeedLengthProperty, value); }
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += bindable_TextChanged;
            base.OnAttachedTo(bindable);
        }

        private void bindable_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (MaxLength != null && MaxLength.HasValue)
            if (e.NewTextValue.Length > 0 && e.NewTextValue.Length > MaxLength)
                ((Entry)sender).Text = e.NewTextValue.Substring(0, MaxLength);

            IsValid = (NeedLength && e.NewTextValue.Length < MaxLength) ? false : true;
            //((Entry)sender).BackgroundColor = (IsValid) ? Color.Default : Color.Red;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= bindable_TextChanged;
            base.OnDetachingFrom(bindable);
        }
    }
    public class EmptyPickerValidationBehavior : Behavior<Picker>
    {
        protected override void OnAttachedTo(Picker entry)
        {
            entry.SelectedIndexChanged += OnPickerChanged;
            base.OnAttachedTo(entry);
        }
        protected override void OnDetachingFrom(Picker entry)
        {
            entry.SelectedIndexChanged -= OnPickerChanged;
            base.OnDetachingFrom(entry);
        }
        void OnPickerChanged(object sender, EventArgs args)
        {
            Picker pk = (sender as Picker);
            bool isValid = false;
            if (pk.SelectedIndex != null && pk.SelectedIndex != -1)
            { // !String.IsNullOrEmpty (args.NewTextValue);  //Double.TryParse (args.NewTextValue, out result);
                if (pk.Items[pk.SelectedIndex].ToString() == "0")
                    isValid = false;
                else
                    isValid = true;
            }
            else
                isValid = false;
            ((Picker)sender).BackgroundColor = isValid ? Color.Default : Color.Red;
        }
    }

	public class CleanString
	{
		//by MSDN http://msdn.microsoft.com/en-us/library/844skk0h(v=vs.71).aspx
		public static string UseRegex(string strIn)
		{
			//return Regex.Replace(strIn, @"[^\w\.@-]", "");

			//return new String(strIn.Where(Char.IsLetterOrDigit).ToArray());

			HashSet<char> removeChars = new HashSet<char>("?&^$#!()+-,:;<>’\'-*");
			StringBuilder result = new StringBuilder(strIn.Length);
			foreach (char c in strIn)
				if (!removeChars.Contains(c))
					result.Append(c);
			return result.ToString();
		}
	}
}
