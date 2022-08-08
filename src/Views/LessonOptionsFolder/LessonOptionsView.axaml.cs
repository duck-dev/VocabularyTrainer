using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VocabularyTrainer.Views;

public partial class LessonOptionsView : UserControl
{
    public LessonOptionsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}