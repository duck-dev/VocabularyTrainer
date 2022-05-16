using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VocabularyTrainer.Views.LearningModes
{
    public partial class WriteView : UserControl
    {
        public WriteView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}