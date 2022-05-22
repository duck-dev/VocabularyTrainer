using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VocabularyTrainer.Views.LearningModes
{
    public partial class WriteView : UserControl
    {
        public WriteView()
        {
            InitializeComponent();
            var textbox = this.Get<TextBox>("MainAnswerTextbox");
            textbox.Initialized += (sender, args) => textbox.Focus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}