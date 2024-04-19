using Ava.IronPython.Common;
using Ava.IronPython.Models;
using Avalonia.Controls.Notifications;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Ava.IronPython.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        PyScript.Text = "import clr\r\nclr.AddReference(\"CommonLib\")\r\nfrom CommonLib import Cal as ca\r\n\r\nt = ca.Add(1,2)";
    }
    [ObservableProperty]
    TextDocument pyScript = new TextDocument();

    [ObservableProperty]
    ObservableCollection<PyVariable> variableList = new ObservableCollection<PyVariable>();
    public void PlayPy()
    {
        VariableList.Clear();
        try
        {
            List<PyVariable> variables = ScriptExecute.Execute(pyScript.Text);
            foreach (var item in variables)
            {
                VariableList.Add(item);
            }
            WeakReferenceMessenger.Default.Send(new Tuple<int, string>(1, "执行完成！"));
        }
        catch (System.Exception e)
        {
            WeakReferenceMessenger.Default.Send(new Tuple<int, string>(-1, e.Message));
        }

    }
}
