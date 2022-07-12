using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.Views.LearningModes;

public class VocabularyListView : UserControl
{
    public VocabularyListView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private void OnStarChecked(object? sender, RoutedEventArgs e)
    {
        SetDifficultTerm(sender, true);
    }

    private void OnStarUnchecked(object? sender, RoutedEventArgs e)
    {
        SetDifficultTerm(sender, false);
    }

    private static void SetDifficultTerm(object? sender, bool difficult)
    {
        if (sender is not StyledElement {DataContext: VocabularyItem word})
            return;

        word.IsDifficult = difficult;
        DataManager.SaveData();
    }
}