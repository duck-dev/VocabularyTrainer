using System;
using Avalonia.Controls;

namespace VocabularyTrainer.Extensions;

public static partial class Extensions
{
    public static void ShowDialogSafe(this Window window, Window? parentWindow)
    {
        if (parentWindow is null)
            throw new ArgumentNullException(nameof(parentWindow));

        window.ShowDialog(parentWindow);
    }
}