using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VocabularyTrainer.ViewModels;

namespace VocabularyTrainer.Views
{
    public class LessonListView : UserControl
    {
        public LessonListView()
        {
            InitializeComponent();
            if (DataContext is ViewModelBase viewModel)
                viewModel.AssignedView = this;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}