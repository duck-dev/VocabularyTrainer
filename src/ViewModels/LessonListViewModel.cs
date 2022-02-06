using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using ReactiveUI;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Views;

namespace VocabularyTrainer.ViewModels
{
    public class LessonListViewModel : ViewModelBase
    {
        private readonly ObservableCollection<object> _items; // TODO: Change to lesson-type

        public LessonListViewModel(IEnumerable<object> items, Window parentWindow)
        {
            this.ParentWindow = parentWindow;
            _items = Items = new ObservableCollection<object>(items);
        }

        private ObservableCollection<object> Items // TODO: Change to lesson-type
        {
            get => _items;
            init => this.RaiseAndSetIfChanged(ref _items, value);
        }

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