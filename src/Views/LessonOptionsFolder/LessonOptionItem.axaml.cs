using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VocabularyTrainer.Views;

public partial class LessonOptionItem : UserControl
{
    public static readonly StyledProperty<Visual> MainContentProperty =
        AvaloniaProperty.Register<LessonOptionItem, Visual>(nameof(MainContent));
    public static readonly StyledProperty<string> TooltipTextProperty =
        AvaloniaProperty.Register<LessonOptionItem, string>(nameof(TooltipText));
    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<LessonOptionItem, string>(nameof(Description));
    
    public LessonOptionItem()
    {
        InitializeComponent();
    }
    
    public Visual MainContent
    {
        get => GetValue(MainContentProperty);
        set => SetValue(MainContentProperty, value);
    }
    
    public string TooltipText
    {
        get => GetValue(TooltipTextProperty);
        set => SetValue(TooltipTextProperty, value);
    }
    
    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}