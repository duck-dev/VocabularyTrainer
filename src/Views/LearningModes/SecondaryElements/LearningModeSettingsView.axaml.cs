using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VocabularyTrainer.Views.LearningModes.SecondaryElements;

public partial class LearningModeSettingsView : UserControl
{
    public static readonly StyledProperty<object?> AdditionalContentProperty =
        AvaloniaProperty.Register<LearningModeSettingsView, object?>(nameof(AdditionalContent));
    
    public LearningModeSettingsView()
    {
        InitializeComponent();
    }
    
    public object? AdditionalContent
    {
        get => GetValue(AdditionalContentProperty);
        set => SetValue(AdditionalContentProperty, value);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}