using System;
using System.Collections;
using System.Collections.ObjectModel;

using MasTicket.Chat;
using Xamarin.Forms;

namespace MasTicket
{
    public class ChatPage : ContentPage
    {
		ObservableCollection<Messages> lstMsg = new ObservableCollection<Messages>();

        public ChatPage()
        {
            Title = "Chat";
            Icon = "chat.png";

            var headerLabel = new Label();
            headerLabel.Font = Font.BoldSystemFontOfSize(24);
            headerLabel.TextColor = Device.OnPlatform(Color.Green, Color.Yellow, Color.Yellow);
            headerLabel.SetBinding(Label.TextProperty, new Binding("Subject", stringFormat:"  {0}"));

            var sendButton = new Button();
            sendButton.Text = " Send ";
            sendButton.VerticalOptions = LayoutOptions.EndAndExpand;
            sendButton.SetBinding(Button.CommandProperty, new Binding("SendMessageCommand"));
            if (Device.OS == TargetPlatform.WinPhone)
            {
                sendButton.BackgroundColor = Color.Green;
                sendButton.BorderColor = Color.Green;
                sendButton.TextColor = Color.White; 
            }

            var inputBox = new Entry();
            inputBox.HorizontalOptions = LayoutOptions.FillAndExpand;
            inputBox.Keyboard = Keyboard.Chat;
            inputBox.Placeholder = "Type a message...";
            inputBox.HeightRequest = 30;
            inputBox.SetBinding(Entry.TextProperty, new Binding("InputText", BindingMode.TwoWay));

			lstMsg.Add(new Messages() { AuthorName = "Asi Compras", Text = "Hola", Timestamp = DateTime.Now, IsMine = false });
			lstMsg.Add(new Messages() { AuthorName = App.usr.name, Text = "Hola", Timestamp = DateTime.Now, IsMine = true });

			var messageList = new ChatListView();
            messageList.VerticalOptions = LayoutOptions.FillAndExpand;
			messageList.ItemsSource = lstMsg;
            messageList.ItemTemplate = new DataTemplate(CreateMessageCell);
            
            Content = new StackLayout
                {
                    Padding = Device.OnPlatform(new Thickness(6,6,6,6), new Thickness(0), new Thickness(0)),
                    Children =
                        {
                            new StackLayout
                                {
                                    Children = {inputBox, sendButton},
                                    Orientation = StackOrientation.Horizontal,
                                    Padding = new Thickness(0, Device.OnPlatform(0, 20, 0),0,0),
                                },
                            //headerLabel,
                            messageList,
                        }
                };
        }

        private Cell CreateMessageCell()
        {
            var timestampLabel = new Label();
            timestampLabel.SetBinding(Label.TextProperty, new Binding("Timestamp", stringFormat: "[{0:HH:mm}]"));
            timestampLabel.TextColor = Color.Silver;
            timestampLabel.Font = Font.SystemFontOfSize(14);

            var authorLabel = new Label();
            authorLabel.SetBinding(Label.TextProperty, new Binding("AuthorName", stringFormat: "{0}: "));
            authorLabel.TextColor = Device.OnPlatform(Color.Blue, Color.Yellow, Color.Yellow);
            authorLabel.Font = Font.SystemFontOfSize(14);

            var messageLabel = new Label();
            messageLabel.SetBinding(Label.TextProperty, new Binding("Text"));
            messageLabel.Font = Font.SystemFontOfSize(14);

            var stack = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children = {authorLabel, messageLabel}
                };

            if (Device.Idiom == TargetIdiom.Tablet)
            {
                stack.Children.Insert(0, timestampLabel);
            }

            var view = new MessageViewCell
                {
                    View = stack
                };
            return view;
        }
    }

	public class Messages
	{
		public DateTime Timestamp { get; set; }
		public string AuthorName { get; set; }
		public string Text { get; set; }
		public bool IsMine { get; set; }
	}
}
