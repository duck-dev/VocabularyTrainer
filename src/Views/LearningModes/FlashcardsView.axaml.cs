using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using VocabularyTrainer.ViewModels.LearningModes;

namespace VocabularyTrainer.Views.LearningModes;

public partial class FlashcardsView : UserControl
{
    public FlashcardsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnStarChecked(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SingleWordViewModelBase dataContext)
            dataContext.SetDifficultTerm(true);
    }

    private void OnStarUnchecked(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SingleWordViewModelBase dataContext)
            dataContext.SetDifficultTerm(false);
    }
}