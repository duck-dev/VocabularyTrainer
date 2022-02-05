using Avalonia.Controls;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Views;

namespace VocabularyTrainer.ViewModels
{
    public class LessonListViewModel : ViewModelBase
    {
        private void OpenAddPage()
        {
            var window = new AddLessonWindow
            {
                DataContext = new AddLessonViewModel()
            };
            
            if(AssignedView is Window parentWindow)
                window.ShowDialogSafe(parentWindow);
        }
    }
}