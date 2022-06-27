using System;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.BaseClasses;

public abstract class LessonViewModelBase : ViewModelBase
{
    private const ErrorTolerance DefaultTolerance = ErrorTolerance.Balanced;
    private const int HigherSettingsOpacity = 1;
    private const float LowerSettingsOpacity = 0.6f;
    
    private int _selectedTolerance;
    private Tuple<string, ErrorTolerance>[] _errorToleranceTemplates = null!;
    private LessonOptions _currentOptions = null!;
    private bool _individualSettingsEnabled;
    private float _individualSettingsOpacity;
    private bool _initializedTolerance;
    
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
            
            ErrorTolerance newTolerance = ErrorToleranceTemplates[_selectedTolerance].Item2;
            if (!_initializedTolerance && MainWindowViewModel.CurrentLesson is { } lesson && newTolerance != lesson.Options.CurrentTolerance)
            {
                _initializedTolerance = true;
                SelectedTolerance = (int)lesson.Options.CurrentTolerance;
                return;
            }
            ChangeTolerance(newTolerance);
            IndividualSettingsEnabled = newTolerance == ErrorTolerance.Custom;
        }
    }

    protected LessonOptions CurrentOptions
    {
        get => _currentOptions;
        set => this.RaiseAndSetIfChanged(ref _currentOptions, value);
    }

    protected bool IndividualSettingsEnabled
    {
        get => _individualSettingsEnabled;
        set
        {
            this.RaiseAndSetIfChanged(ref _individualSettingsEnabled, value);
            IndividualSettingsOpacity = value == true ? HigherSettingsOpacity : LowerSettingsOpacity;
        }
    }

    protected float IndividualSettingsOpacity
    {
        get => _individualSettingsOpacity;
        set => this.RaiseAndSetIfChanged(ref _individualSettingsOpacity, value);
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