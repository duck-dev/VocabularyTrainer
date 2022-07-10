using ReactiveUI;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models;
using VocabularyTrainer.ViewModels.BaseClasses;
using VocabularyTrainer.ViewModels.LearningModes;

namespace VocabularyTrainer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _content;
    private DialogViewModelBase? _currentDialog;
    private string _currentLearningMode = string.Empty;

    public MainWindowViewModel()
    {
        Instance = this;
        DataManager.LoadData();
        _content = Content = NewLessonList;
    } 
        
    internal static MainWindowViewModel? Instance { get; private set; }
    internal static Lesson? CurrentLesson { get; set; }
    internal static LessonListViewModel NewLessonList => new(DataManager.Lessons);

    internal ViewModelBase Content
    {
        get => _content;
        set
        {
            this.RaiseAndSetIfChanged(ref _content, value);
            
            if (value is LearningModeViewModelBase) 
                return;
            CurrentLearningMode = string.Empty;
        }
    }
        
    internal DialogViewModelBase? CurrentDialog
    {
        get => _currentDialog;
        set => this.RaiseAndSetIfChanged(ref _currentDialog, value);
    }
    
    internal string CurrentLearningMode
    {
        get => _currentLearningMode; 
        set => this.RaiseAndSetIfChanged(ref _currentLearningMode, value);
    }

    internal void ReturnHome(bool discardChanges = true)
    {
        switch (_content)
        {
            case LessonListViewModel:
                return;
            case IDiscardableChanges discardable when discardChanges:
            {
                if (discardable.DataChanged)
                    discardable.ConfirmDiscarding();
                else
                    this.Content = NewLessonList;
                break;
            }
            default:
                this.Content = NewLessonList;
                break;
        }

        CurrentLesson = null;
    }
}