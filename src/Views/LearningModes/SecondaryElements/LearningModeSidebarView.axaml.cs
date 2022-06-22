using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VocabularyTrainer.Views.LearningModes
{
    public partial class LearningModeSidebarView : UserControl
    {
        public static readonly StyledProperty<object?> AdditionalContentProperty =
            AvaloniaProperty.Register<LearningModeSidebarView, object?>(nameof(AdditionalContent));
        
        public LearningModeSidebarView()
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
}