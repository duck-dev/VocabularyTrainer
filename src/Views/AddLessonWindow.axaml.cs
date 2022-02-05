using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VocabularyTrainer.Views
{
    public class AddLessonWindow : Window
    {
        public AddLessonWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}