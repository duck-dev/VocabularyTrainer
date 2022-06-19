using System;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.BaseClasses;

public abstract class LessonViewModelBase : ViewModelBase
{
    private const ErrorTolerance DefaultTolerance = ErrorTolerance.Balanced;
    
    private int _selectedTolerance;
    private Tuple<string, ErrorTolerance>[] _errorToleranceTemplates = null!;
    private LessonOptions _currentOptions = null!;
    
    protected Tuple<string, ErrorTolerance>[] ErrorToleranceTemplates
    {
        get => _errorToleranceTemplates; 
        private set => this.RaiseAndSetIfChanged(ref _errorToleranceTemplates, value);
    }
    
    protected internal int SelectedTolerance
    {
        get => _selectedTolerance;
        set
        {
            if (value == _selectedTolerance || value < 0 || value >= ErrorToleranceTemplates.Length)
                return;
            this.RaiseAndSetIfChanged(ref _selectedTolerance, value);
            ChangeTolerance(ErrorToleranceTemplates[_selectedTolerance].Item2);
        }
    }

    protected LessonOptions CurrentOptions
    {
        get => _currentOptions; 
        set => this.RaiseAndSetIfChanged(ref _currentOptions, value);
    }

    protected virtual void ChangeTolerance(ErrorTolerance newTolerance)
    {
        if (newTolerance == ErrorTolerance.Custom) 
            return;
        var newOptions = LessonOptions.MatchTolerance(newTolerance);
        newOptions.ViewModel = this;
        this.CurrentOptions = newOptions;
    }

    protected internal virtual void ChangeSettings() { }

    protected virtual void Initialize(Lesson? lesson = null)
    {
        var enumValues = Enum.GetValues<ErrorTolerance>();
        ErrorToleranceTemplates = new Tuple<string, ErrorTolerance>[enumValues.Length];
        for (int i = 0; i < enumValues.Length; i++)
        {
            ErrorTolerance currentValue = enumValues[i];
            ErrorToleranceTemplates[i] = new Tuple<string, ErrorTolerance>(currentValue.ToString(), currentValue);
            if (this is AddLessonViewModel && currentValue == DefaultTolerance)
                this.SelectedTolerance = i;
        }
    }
}