using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Media;
using ReactiveUI;
using VocabularyTrainer.Models;
using VocabularyTrainer.ViewModels.Dialogs;

namespace VocabularyTrainer.ViewModels;

public class LessonListViewModel : ViewModelBase
{
    private ObservableCollection<Lesson>? _items;

    public LessonListViewModel(IEnumerable<Lesson> items)
    {
        UpdateLessons(items);
    }

    private ObservableCollection<Lesson>? Items
    {
        get => _items;
        set => this.RaiseAndSetIfChanged(ref _items, value);
    }
        
    private bool EmptyCollection => Items?.Count == 0;

    internal void UpdateLessons(IEnumerable<Lesson> items)
    {
        Items = new ObservableCollection<Lesson>(items);
        Items.CollectionChanged += (sender, args) => this.RaisePropertyChanged(nameof(EmptyCollection));
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

        MainWindowViewModel.CurrentLesson = lesson;
        MainViewModel.Content = new LessonViewModel(lesson);
    }

    private void RemoveLesson(Lesson lesson)
    {
        if (MainViewModel is null)
            return;
            
        Action confirmAction = () =>
        {
            Items?.Remove(lesson);
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