using Ava.IronPython.Common;
using Ava.IronPython.Models;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static IronPython.Modules.PythonIterTools;

namespace Ava.IronPython.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        PyScript.Text = "import clr\r\nclr.AddReference(\"CommonLib\")\r\nfrom CommonLib import Cal as ca\r\n\r\nt = ca.Add(1,2)";
        pyScript.TextLengthChanged += PyScript_TextLengthChanged;
        PyScript_TextLengthChanged(null, null);
        WeakReferenceMessenger.Default.Register<TextArea>(this, OnBreakPointsKeyDown);
    }

    [ObservableProperty]
    TextDocument pyScript = new TextDocument();

    [ObservableProperty]
    ObservableCollection<PyVariable> variableList = new ObservableCollection<PyVariable>();

    [ObservableProperty]
    ObservableCollection<BreakPoint> breakPoints = new ObservableCollection<BreakPoint>();

    [ObservableProperty]
    bool isDebuging;
    public void PlayPy()
    {
        try
        {
            ScriptScope? scope = null;
            List<PyVariable> variables = ScriptExecute.Execute(pyScript.Text, ref scope);
            UpdateVariables(variables);
            WeakReferenceMessenger.Default.Send(new Tuple<int, string>(1, "执行完成！"));
        }
        catch (Exception e)
        {
            WeakReferenceMessenger.Default.Send(new Tuple<int, string>(-1, e.Message));
        }
    }

    /// <summary>
    /// 更新变量列表
    /// </summary>
    /// <param name="variables">变量列表</param>
    private void UpdateVariables(List<PyVariable> variables)
    {
        VariableList.Clear();
        foreach (var item in variables)
        {
            VariableList.Add(item);
        }
    }

    string[] scriptArray;
    int currentRow = 0;
    ScriptScope? currentScope;
    public void DebugPy()
    {
        scriptArray = PyScript.Text.Split('\n');
        currentScope = null;
        IsDebuging = true;
        for (int i = 0; i < scriptArray.Length; i++)
        {
            foreach (var item in BreakPoints)
            {
                item.IsHit = false;
            }

            currentRow = i;
            string line = scriptArray[i];
            if (BreakPoints[i].Enable)// 当前断点启用，将停止并标记
            {
                BreakPoints[i].IsHit = true;
                break;
            }
            else// 当前断点未启用，执行本句
            {
                try
                {
                    List<PyVariable> variables = ScriptExecute.Execute(line, ref currentScope);
                    UpdateVariables(variables);
                    if (currentRow >= scriptArray.Length - 1)
                    {
                        IsDebuging = false;
                    }
                }
                catch (Exception e)
                {
                    WeakReferenceMessenger.Default.Send(new Tuple<int, string>(-1, e.Message));
                    IsDebuging = false;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 继续运行到下一个断点
    /// </summary>
    public void ContinueDebug()
    {
        for (int i = currentRow; i < scriptArray.Length; i++)
        {
            foreach (var item in BreakPoints)
            {
                item.IsHit = false;
            }

            string line = scriptArray[i];
            if (BreakPoints[i].Enable && currentRow != i)// 当前断点启用，将停止并标记
            {
                BreakPoints[i].IsHit = true;
                currentRow = i;
                break;
            }
            else// 当前断点未启用，执行本句
            {
                try
                {
                    List<PyVariable> variables = ScriptExecute.Execute(line, ref currentScope);
                    UpdateVariables(variables);
                    currentRow = i;
                    if (currentRow >= scriptArray.Length - 1)
                    {
                        IsDebuging = false;
                    }
                }
                catch (Exception e)
                {
                    WeakReferenceMessenger.Default.Send(new Tuple<int, string>(-1, e.Message));
                    IsDebuging = false;
                    return;
                }

            }
            currentRow = i;
        }
    }

    /// <summary>
    /// 单行继续运行
    /// </summary>
    public void StepOver()
    {
        foreach (var item in BreakPoints)
        {
            item.IsHit = false;
        }

        try
        {
            string line = scriptArray[currentRow];
            List<PyVariable> variables = ScriptExecute.Execute(line, ref currentScope);
            UpdateVariables(variables);
            currentRow++;
            if (currentRow < scriptArray.Length)
            {
                BreakPoints[currentRow].IsHit = true;
            }
            else
            {
                IsDebuging = false;
            }
        }
        catch (Exception e)
        {
            WeakReferenceMessenger.Default.Send(new Tuple<int, string>(-1, e.Message));
            IsDebuging = false;
        }
    }

    public void ViewKeyDown(object? sender)
    {
        var keyEvent = (sender as KeyEventArgs);
        if (keyEvent == null)
        {
            return;
        }
        if (keyEvent.Key == Key.F5 && keyEvent.KeyModifiers == KeyModifiers.Control && !IsDebuging)
        {
            DebugPy();
            return;
        }
        if (keyEvent.Key == Key.F5 && !IsDebuging)
        {
            PlayPy();
            return;
        }
        if (keyEvent.Key == Key.F5 && IsDebuging)
        {
            ContinueDebug();
            return;
        }
        if (keyEvent.Key == Key.F10 && IsDebuging)
        {
            StepOver();
            return;
        }
    }

    private void PyScript_TextLengthChanged(object? sender, EventArgs? e)
    {
        var count = PyScript.Text.Count(x => x == '\n');
        var temp = BreakPoints;
        BreakPoints = new ObservableCollection<BreakPoint>();
        for (int i = 0; i < count + 1; i++)
        {
            var point = new BreakPoint() { Row = i };
            if (temp.Count > i)
            {
                point.Enable = temp[i].Enable;
            }
            BreakPoints?.Add(point);
        }

        temp = null;
    }

    /// <summary>
    /// 切换断点启用
    /// </summary>
    private void OnBreakPointsKeyDown(object recipient, TextArea message)
    {
        var currentRow = message.TextView.HighlightedLine;
        if (BreakPoints.Count >= currentRow - 1)
        {
            BreakPoints[currentRow - 1].Enable = !BreakPoints[currentRow - 1].Enable;
        }
    }
}
