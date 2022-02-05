using System;
using Avalonia.Controls;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Views;

namespace VocabularyTrainer.ViewModels
{
    public class LessonListViewModel : ViewModelBase
    {
        public LessonListViewModel(Window parentWindow) => this.ParentWindow = parentWindow;
        
        private void OpenAddPage()
        {
            if (ParentWindow is not { } parentWindow)
                return;
            
            var window = new AddLessonWindow
            {
                DataContext = new AddLessonViewModel()
            };
            window.ShowDialogSafe(parentWindow);
        }
    }
}