using Xamarin.Forms;
using System;
using System.Windows.Input;

namespace MasTicket
{
	public partial class OpcionButton : ContentView
	{
		public event EventHandler Clicked;

		public static readonly BindableProperty CommandProperty =
			BindableProperty.Create("Command", typeof(ICommand), typeof(Button), null);

		public ICommand Command
		{
			get
			{
				return (ICommand)GetValue(CommandProperty);
			}
			set
			{
				SetValue(CommandProperty, value);
			}
		}

		public static readonly BindableProperty ButtonBackgroundColorProperty =
			BindableProperty.Create("ButtonBackgroundColor", typeof(Color), typeof(OpcionButton), Color.Transparent);

		public Color ButtonBackgroundColor
		{
			get
			{
				return (Color)GetValue(ButtonBackgroundColorProperty);
			}
			set
			{
				SetValue(ButtonBackgroundColorProperty, value);
			}
		}

		public static readonly BindableProperty TituloProperty =
			BindableProperty.Create("Titulo", typeof(string), typeof(OpcionButton), null);
		public string Titulo
		{
			get
			{
				return (string)GetValue(TituloProperty);
			}
			set
			{
				SetValue(TituloProperty, value);
			}
		}
		public static readonly BindableProperty DescProperty =
			BindableProperty.Create("Desc", typeof(string), typeof(OpcionButton), null);
		public string Desc
		{
			get
			{
				return (string)GetValue(DescProperty);
			}
			set
			{
				SetValue(DescProperty, value);
			}
		}

		public static readonly BindableProperty SourceProperty =
			BindableProperty.Create("Source", typeof(FileImageSource), typeof(OpcionButton), null);

		public FileImageSource Source
		{
			get
			{
				return (FileImageSource)GetValue(SourceProperty);
			}
			set
			{
				SetValue(SourceProperty, value);
			}
		}

		public OpcionButton()
		{
			InitializeComponent();
			root.BindingContext = this;
		}

		async void HandleClick(object sender, EventArgs e)
		{
			Clicked.Invoke(this, e);

			await root.ScaleTo(1.2, 100);
			await root.ScaleTo(1, 100);
		}
	}
}
