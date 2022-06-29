using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VocabularyTrainer.Views.LearningModes;

public partial class ThesaurusView : UserControl
{
    public ThesaurusView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}