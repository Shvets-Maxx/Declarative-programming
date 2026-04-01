using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Lab2Avalonia;

public partial class MainWindow : Window
{
    private readonly string _defaultSavePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "lab2_text_avalonia.txt");

    public MainWindow()
    {
        InitializeComponent();

        BtnOpen.Click += BtnOpen_Click;
        BtnClear.Click += BtnClear_Click;
        BtnCopy.Click += BtnCopy_Click;
        BtnPaste.Click += BtnPaste_Click;
        BtnOk.Click += BtnOk_Click;
    }

    private async void BtnOpen_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            AllowMultiple = false
        };
        var result = await dialog.ShowAsync(this);
        if (result != null && result.Length > 0)
        {
            try
            {
                TxtDocument.Text = await File.ReadAllTextAsync(result[0]);
            }
            catch (Exception ex)
            {
                await ShowMessageAsync($"Помилка відкриття: {ex.Message}");
            }
        }
    }

    private void BtnClear_Click(object? sender, RoutedEventArgs e)
    {
        TxtDocument.Text = string.Empty;
    }

    private async void BtnCopy_Click(object? sender, RoutedEventArgs e)
    {
        // Використовуємо Clipboard через TopLevel, бо this.Clipboard може бути null
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
            return;

        var textToCopy = !string.IsNullOrEmpty(TxtDocument.SelectedText)
            ? TxtDocument.SelectedText
            : TxtDocument.Text;

        if (!string.IsNullOrEmpty(textToCopy))
        {
            await topLevel.Clipboard.SetTextAsync(textToCopy);
        }
    }

    private async void BtnPaste_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
            return;

        var text = await topLevel.Clipboard.GetTextAsync();
        if (!string.IsNullOrEmpty(text))
        {
            var caret = TxtDocument.CaretIndex;
            TxtDocument.Text = TxtDocument.Text?.Insert(caret, text);
            TxtDocument.CaretIndex = caret + text.Length;
        }
    }

    private async void BtnOk_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtDocument.Text))
        {
            await ShowMessageAsync("Немає тексту для збереження.");
            return;
        }

        try
        {
            await File.WriteAllTextAsync(_defaultSavePath, TxtDocument.Text);
            await ShowMessageAsync($"Файл збережено: {_defaultSavePath}");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync($"Помилка збереження: {ex.Message}");
        }
    }

    private async System.Threading.Tasks.Task ShowMessageAsync(string message)
    {
        var dialog = new Window
        {
            Width = 300,
            Height = 150,
            Title = "Повідомлення",
            Content = new TextBlock
            {
                Text = message,
                Margin = new Thickness(20),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            }
        };
        await dialog.ShowDialog(this);
    }
}