using System;
using Avalonia.Media;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models;
using VocabularyTrainer.ViewModels.BaseClasses;
using VocabularyTrainer.ViewModels.Dialogs;

namespace VocabularyTrainer.ViewModels;

public class LessonViewModel : LessonViewModelBase, IDiscardableChanges
{
    public LessonViewModel(Lesson lesson) => Initialize(lesson);

    private Lesson CurrentLesson { get; set; } = null!;
    private string AdjustableItemsString => CurrentLesson.VocabularyItems.Count == 1 ? "item" : "items";

    public bool DataChanged => CurrentLesson.DataChanged;

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