using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using VocabularyTrainer.ViewModels.LearningModes;

namespace VocabularyTrainer.Views.LearningModes.SecondaryElements;

public partial class AnswerView : UserControl
{
    public static readonly StyledProperty<Visual?> AdditionalTitleProperty =
        AvaloniaProperty.Register<LessonOptionItem, Visual?>(nameof(AdditionalTitle));
    
    public AnswerView()
    {
        InitializeComponent();
        var textbox = this.Get<TextBox>("MainAnswerTextbox");
        textbox.Initialized += (_, _) =>
        {
            textbox.Focus();
            if (this.DataContext is AnswerViewModelBase dataContext)
                dataContext.ReadyToFocus += (_, _) => textbox.Focus();
        };
    }
    
    public Visual? AdditionalTitle
    {
        get => GetValue(AdditionalTitleProperty);
        set => SetValue(AdditionalTitleProperty, value);
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