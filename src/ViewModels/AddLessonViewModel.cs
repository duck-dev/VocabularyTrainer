using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models;
using VocabularyTrainer.ViewModels.BaseClasses;
using VocabularyTrainer.ViewModels.Dialogs;

namespace VocabularyTrainer.ViewModels;

public sealed class AddLessonViewModel : LessonViewModelBase, IVocabularyContainer<Word>, IDiscardableChanges
{
    public AddLessonViewModel()
    {
        VocabularyItems.CalculateIndexReactive(this, false, nameof(AdjustableItemsString));
        Initialize();
    }

    public ObservableCollection<Word> VocabularyItems { get; } = new() { new Word() };

    public bool DataChanged => (!string.IsNullOrEmpty(CurrentName) && !string.IsNullOrWhiteSpace(CurrentName)) 
                               || (!string.IsNullOrEmpty(CurrentDescription) && !string.IsNullOrWhiteSpace(CurrentDescription))
                               || (VocabularyItems.Count > 0 && VocabularyItems.Any(x => x.IsFilled)) 
                               || (!CurrentOptions.Equals(LessonOptions.BalancedTolerance));

    private string CurrentName { get; set; } = string.Empty;
    private string CurrentDescription { get; set; } = string.Empty;
    private string AdjustableItemsString => VocabularyItems.Count == 1 ? "item" : "items";
    
    public void ConfirmDiscarding()
    {
        if (MainViewModel is null)
            return;

        if (!DataChanged)
        {
            MainViewModel.ReturnHome(false);
            return;
        }
        
        Action confirmAction = () => { MainViewModel.Content = MainWindowViewModel.NewLessonList; };
        const string dialogTitle = "Do you really want to return without creating the lesson with the data filled in?";
        MainViewModel.CurrentDialog = new ConfirmationDialogViewModel(dialogTitle,
            new [] { Color.Parse("#D64045"), Color.Parse("#808080") },
            new[] { Colors.White, Colors.White },
            new[] { "Yes, proceeding without creating the lesson!", "Cancel" },
            confirmAction);
    }

    private void AddWord()
    {
        var newWord = new Word();
        VocabularyItems.Add(newWord);
    }

    private void CreateLesson()
    {
        var lesson = new Lesson(CurrentName, CurrentDescription, VocabularyItems, Lesson.InitShuffledDictionary(), 
            CurrentOptions, new LearningModeOptions(), false);
        DataManager.AddData(lesson);
        MainViewModel?.ReturnHome();
    }
}