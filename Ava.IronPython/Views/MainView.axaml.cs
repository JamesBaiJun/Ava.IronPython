using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Folding;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;
using Avalonia.Diagnostics;
using AvaloniaEdit;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.Messaging;

namespace Ava.IronPython.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        Loaded += MainView_Loaded;

    }

    TextMate.Installation _textMateInstallation;
    RegistryOptions _registryOptions;
    ScrollViewer editorScrollViewer { get; set; }
    private void MainView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var _textEditor = this.FindControl<TextEditor>("Editor");
        editorScrollViewer = _textEditor.GetTemplateChildren().ToList()[1] as ScrollViewer;
        editorScrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
        _registryOptions = new RegistryOptions(ThemeName.DimmedMonokai);
        _textMateInstallation = _textEditor.InstallTextMate(_registryOptions);

        Language pyLanguage = _registryOptions.GetLanguageByExtension(".py");
        _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(pyLanguage.Id));
    }

    private int _currentTheme = (int)ThemeName.DimmedMonokai;
    private void ButtonTheme_Click(object sender, RoutedEventArgs e)
    {
        _currentTheme = (_currentTheme + 1) % Enum.GetNames(typeof(ThemeName)).Length;
        _textMateInstallation.SetTheme(_registryOptions.LoadTheme(
            (ThemeName)_currentTheme));
    }

    private void ScrollViewer_ScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        BreakItems.Height = (sender as ScrollViewer).Extent.Height;
        BreakScrollViewer.Offset = (sender as ScrollViewer).Offset;
    }

    private void TextEditor_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.F9)
        {
            return;
        }
        WeakReferenceMessenger.Default.Send((sender as TextEditor).TextArea);
    }
}
