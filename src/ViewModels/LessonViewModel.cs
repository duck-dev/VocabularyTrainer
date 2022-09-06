using System;
using System.Linq;
using Avalonia.Media;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models;
using VocabularyTrainer.ViewModels.BaseClasses;
using VocabularyTrainer.ViewModels.Dialogs;

namespace VocabularyTrainer.ViewModels;

public class LessonViewModel : LessonViewModelBase, IDiscardableChanges
{
    private string _searchTerm = string.Empty;
    private Word[] _exposedVocabularyItems = null!;
    
    public LessonViewModel(Lesson lesson) => Initialize(lesson);
    
    public bool DataChanged => CurrentLesson.DataChanged;

    private Lesson CurrentLesson { get; set; } = null!;

    private Word[] ExposedVocabularyItems
    {
        get => _exposedVocabularyItems;
        set
        {
            this.RaisePropertyChanged(nameof(NoElementsFound));
            this.RaiseAndSetIfChanged(ref _exposedVocabularyItems, value);
        }
    }
    private string AdjustableItemsString => CurrentLesson.VocabularyItems.Count == 1 ? "item" : "items";

    private string SearchTerm
    {
        get => _searchTerm;
        set
        {
            ExposedVocabularyItems = CurrentLesson.VocabularyItems.Where(x => x.ContainsTerm(value)).ToArray();
            _searchTerm = value;
        }
    }

    private bool NoElementsFound => ExposedVocabularyItems.Length <= 0;

    public void ConfirmDiscarding()
    {
        if (MainViewModel is null)
            return;

        if (!DataChanged)
        {
            MainViewModel.ReturnHome(false);
            return;
        }

        Action confirmAction = () =>
        {
            DiscardChanges();
            MainViewModel.Content = MainWindowViewModel.NewLessonList;
        };
        const string dialogTitle = "Do you really want to return without saving the changes?";
        MainViewModel.CurrentDialog = new ConfirmationDialogViewModel(dialogTitle,
            new [] { Color.Parse("#D64045"), Color.Parse("#808080") },
            new[] { Colors.White, Colors.White },
            new[] { "Yes, don't save my changes!", "Cancel" },
            confirmAction);
    }

    protected internal override void ChangeSettings()
    {
        base.ChangeSettings();
        CurrentLesson.ChangedOptions = CurrentOptions;
    }

    protected override void ChangeTolerance(ErrorTolerance newTolerance)
    {
        base.ChangeTolerance(newTolerance);
        CurrentLesson.ChangedOptions = CurrentOptions;
    }

    protected sealed override void Initialize(Lesson? lesson = null)
    {
        if (lesson is null)
            return;
        this.CurrentLesson = lesson;
        lesson.VocabularyItems.CalculateIndexReactive(this, true, nameof(AdjustableItemsString));
        ExposedVocabularyItems = CurrentLesson.VocabularyItems.ToArray();
        
        var newOptions = lesson.Options;
        newOptions.ViewModel = this;
        CurrentOptions = newOptions;
        base.Initialize(lesson); // Must be at the end
    }
        
    private void DiscardChanges()
    {
        CurrentLesson.DiscardChanges();
        MainViewModel?.ReturnHome(false);
    }

    private void SaveChanges()
    {
        CurrentLesson.SaveChanges();
        DataManager.SaveData();
    }
}