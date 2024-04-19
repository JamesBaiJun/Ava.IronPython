using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging;
using System;
namespace Ava.IronPython.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var topLevel = TopLevel.GetTopLevel(this);
        _manager = new WindowNotificationManager(topLevel) { MaxItems = 3 };
        WeakReferenceMessenger.Default.Register<Tuple<int, string>>(this, OnMessageNotify);
    }

    private void OnMessageNotify(object recipient, Tuple<int, string> message)
    {
        switch (message.Item1)
        {
            case -1:
                _manager?.Show(message.Item2 + "\r\n" + DateTime.Now.ToString(), NotificationType.Error);
                break;
            case 1:
                _manager?.Show(message.Item2 + "\r\n" + DateTime.Now.ToString(), NotificationType.Success);
                break;
            default:
                break;
        }
    }

    private WindowNotificationManager? _manager;

}
