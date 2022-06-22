using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Media;
using ReactiveUI;
using VocabularyTrainer.Models;
using VocabularyTrainer.ViewModels.Dialogs;

namespace VocabularyTrainer.ViewModels
{
    public class LessonListViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Lesson> _items;

        public LessonListViewModel(IEnumerable<Lesson> items)
        {
            _items = Items = new ObservableCollection<Lesson>(items);
        }

        private ObservableCollection<Lesson> Items
        {
            get => _items;
            init => this.RaiseAndSetIfChanged(ref _items, value);
        }

        private void OpenAddPage()
        {
            if(MainViewModel is not null)
                MainViewModel.Content = new AddLessonViewModel();
        }

        private void OpenLesson(Lesson lesson)
        {
            if (MainViewModel is null)
                return;

            var lessonViewModel = new LessonViewModel(lesson);
            MainViewModel.Content = lessonViewModel;
            lessonViewModel.SelectedTolerance = (int)lesson.Options.CurrentTolerance;
            MainWindowViewModel.CurrentLesson = lesson;
        }

        private void RemoveLesson(Lesson lesson)
        {
            if (MainViewModel is null)
                return;
            
            Action confirmAction = () =>
            {
                Items.Remove(lesson);
                DataManager.Lessons.Remove(lesson);
                DataManager.SaveData();
            };
            string dialogTitle = $"Do you really want to remove the lesson \"{lesson.Name}\"?";
            MainViewModel.CurrentDialog = new ConfirmationDialogViewModel(dialogTitle,
                new [] { Color.Parse("#D64045"), Color.Parse("#808080") },
                new[] { Colors.White, Colors.White },
                new[] { "Remove", "Cancel" },
                confirmAction);
        }
    }
}