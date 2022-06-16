using System;
using VocabularyTrainer.Enums;

namespace VocabularyTrainer.ViewModels.BaseClasses;

public abstract class LessonViewModelBase : ViewModelBase
{
    private int _selectedTolerance;
    
    protected LessonViewModelBase()
    {
        var enumValues = Enum.GetValues<ErrorTolerance>();
        ErrorToleranceTemplates = new Tuple<string, ErrorTolerance>[enumValues.Length];
        for (int i = 0; i < enumValues.Length; i++)
            ErrorToleranceTemplates[i] = new Tuple<string, ErrorTolerance>(enumValues[i].ToString(), enumValues[i]);
    }
    
    protected Tuple<string, ErrorTolerance>[] ErrorToleranceTemplates { get; }

    protected int SelectedTolerance
    {
        get => _selectedTolerance;
        set
        {
            if (value == _selectedTolerance)
                return;
            _selectedTolerance = value;
            ChangeTolerance(ErrorToleranceTemplates[_selectedTolerance].Item2);
        }
    }

    protected abstract void ChangeTolerance(ErrorTolerance newTolerance);
}