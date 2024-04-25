using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging;
using System;

namespace Ava.IronPython.Views
{
    public partial class OutputView : UserControl
    {
        public OutputView()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<Tuple<int, string>>(this, OnMessageNotify);
        }

        private void OnMessageNotify(object recipient, Tuple<int, string> message)
        {
            switch (message.Item1)
            {
                case -1:
                    LogBox.Text += $"{DateTime.Now} [Error] : {message.Item2}\r\n";
                    break;
                case 1:
                    LogBox.Text += $"{DateTime.Now} [Info] : {message.Item2}\r\n";
                    break;
                default:
                    break;
            }

            LogBox.CaretIndex = LogBox.Text.Length;
        }
    }
}
