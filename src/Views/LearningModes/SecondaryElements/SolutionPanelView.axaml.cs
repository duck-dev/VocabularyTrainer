using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using VocabularyTrainer.ViewModels.LearningModes;

namespace VocabularyTrainer.Views.LearningModes;

public class SolutionPanelView : UserControl
{
    public SolutionPanelView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private void OnStarChecked(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SolutionPanelViewModel dataContext)
            dataContext.AnswerViewModel?.SetDifficultTerm(true);
    }

    private void OnStarUnchecked(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SolutionPanelViewModel dataContext)
            dataContext.AnswerViewModel?.SetDifficultTerm(false);
    }
}