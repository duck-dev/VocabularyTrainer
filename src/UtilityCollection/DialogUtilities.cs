using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using VocabularyTrainer.Views;

namespace VocabularyTrainer.UtilityCollection;

public static partial class Utilities
{
    private static MainWindow MainWindowInstance 
        => MainWindow.Instance ?? throw new InvalidOperationException("MainWindow.Instance is null and can't be used as a parent window for the dialog.");
    
    internal static async Task<string?> InvokeOpenFolderDialog(string title, string? directory = null)
    {
        var fileDialog = new OpenFolderDialog
        {
            Title = title, 
            Directory = directory
        };
        
        string? result = await fileDialog.ShowAsync(MainWindowInstance);
        return result;
    }

    internal static async Task<string[]?> InvokeOpenFileDialog(string title, bool allowMultiple, string? directory = null, 
        string? initialFileName = null, List<FileDialogFilter>? filters = null)
    {
        var dialog = new OpenFileDialog
        {
            Title = title, 
            AllowMultiple = allowMultiple, 
            Directory = directory, 
            InitialFileName = initialFileName, 
            Filters = filters
        };
        
        string[]? result = await dialog.ShowAsync(MainWindowInstance);
        return result;
    }

    internal static async Task<string?> InvokeSaveFileDialog(string title, string initialFileName, string extension, 
        string? directory = null, List<FileDialogFilter>? filters = null)
    {
        var dialog = new SaveFileDialog
        {
            Title = title,
            InitialFileName = initialFileName,
            DefaultExtension = extension,
            Directory = directory,
            Filters = filters
        };

        string? result = await dialog.ShowAsync(MainWindowInstance);
        return result;
    }
}