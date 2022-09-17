using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    private ObservableCollection<Word> _exposedVocabularyItems = null!;
    
    public LessonViewModel(Lesson lesson) => Initialize(lesson);
    
    public bool DataChanged => CurrentLesson.DataChanged;

    private Lesson CurrentLesson { get; set; } = null!;

    private ObservableCollection<Word> ExposedVocabularyItems
    {
        get => _exposedVocabularyItems;
        set
        {
            this.RaiseAndSetIfChanged(ref _exposedVocabularyItems, value);
            this.RaisePropertyChanged(nameof(NoElementsFound));
        }
    }
    private string AdjustableItemsString => CurrentLesson.VocabularyItems.Count == 1 ? "item" : "items";

    private string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (_searchTerm.Equals(value))
                return;
            
            foreach (Word word in CurrentLesson.VocabularyItems)
            {
                if (string.IsNullOrEmpty(value) || word.ContainsTerm(value))
                {
                    if (ExposedVocabularyItems.Contains(word)) 
                        continue;
                        
                    if (ExposedVocabularyItems.Count <= 0)
                    {
                        ExposedVocabularyItems.Add(word);
                        continue;
                    }
                        
                    for (int i = 0; i < ExposedVocabularyItems.Count; i++)
                    {
                        if (i == 0 && word.Index <= ExposedVocabularyItems[i].Index)
                        {
                            ExposedVocabularyItems.Insert(i, word);
                            break;
                        }
                            
                        if (i < ExposedVocabularyItems.Count - 1 && (word.Index <= ExposedVocabularyItems[i].Index || word.Index >= ExposedVocabularyItems[i + 1].Index)) 
                            continue;
                        ExposedVocabularyItems.Insert(i + 1, word);
                        break;
                    }
                }
                else
                {
                    ExposedVocabularyItems.Remove(word);
                }
            }
            
            this.RaiseAndSetIfChanged(ref _searchTerm, value);
            this.RaisePropertyChanged(nameof(NoElementsFound));
        }
    }

    private bool NoElementsFound => ExposedVocabularyItems.Count <= 0;

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
        ExposedVocabularyItems = new ObservableCollection<Word>(CurrentLesson.VocabularyItems);
        CurrentLesson.VocabularyItems.CollectionChanged += (sender, args) =>
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add when args.NewItems != null:
                    foreach (var item in args.NewItems)
                    {
                        if(item is Word word)
                            ExposedVocabularyItems.Add(word);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove when args.OldItems != null:
                    foreach (var item in args.OldItems)
                    {
                        if(item is Word word)
                            ExposedVocabularyItems.Remove(word);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                default:
                    break;
            }
        };

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

    private void ClearSearchbar() => SearchTerm = string.Empty;
}