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

    protected abstract void ChangeTolerance(ErrorTolerance newTolerance);

    protected virtual void Initialize(Lesson? lesson = null)
    {
        var enumValues = Enum.GetValues<ErrorTolerance>();
        ErrorToleranceTemplates = new Tuple<string, ErrorTolerance>[enumValues.Length];
        for (int i = 0; i < enumValues.Length; i++)
        {
            ErrorTolerance currentValue = enumValues[i];
            ErrorToleranceTemplates[i] = new Tuple<string, ErrorTolerance>(currentValue.ToString(), currentValue);
            if (currentValue == DefaultTolerance)
                this.SelectedTolerance = i;
        }
    }
}