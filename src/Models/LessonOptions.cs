using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.ViewModels.BaseClasses;

namespace VocabularyTrainer.Models;

public class LessonOptions : IEquatable<LessonOptions>, INotifyPropertyChangedHelper
{
    private int _correctionSteps;
    private bool _tolerateSwappedLetters;
    private bool _ignoreAccentMarks;
    private bool _ignoreHyphens;
    private bool _ignorePunctuation;
    private bool _ignoreCapitalization;
    
    public event PropertyChangedEventHandler? PropertyChanged;

    [JsonInclude]
    public int CorrectionSteps
    {
        get => _correctionSteps;
        set
        {
            if (_correctionSteps == value)
                return;
            _correctionSteps = value;
            ViewModel?.ChangeSettings();
            NotifyPropertyChanged();
        }
    }

    [JsonInclude]
    public bool TolerateSwappedLetters
    {
        get => _tolerateSwappedLetters;
        set
        {
            if (_tolerateSwappedLetters == value)
                return;
            _tolerateSwappedLetters= value;
            ViewModel?.ChangeSettings();
            NotifyPropertyChanged();
        }
    }

    [JsonInclude]
    public bool IgnoreAccentMarks
    {
        get => _ignoreAccentMarks;
        set
        {
            if (_ignoreAccentMarks == value)
                return;
            _ignoreAccentMarks = value;
            ViewModel?.ChangeSettings();
            NotifyPropertyChanged();
        }
    }

    [JsonInclude]
    public bool IgnoreHyphens
    {
        get => _ignoreHyphens;
        set
        {
            if (_ignoreHyphens == value)
                return;
            _ignoreHyphens = value;
            ViewModel?.ChangeSettings();
            NotifyPropertyChanged();
        }
    }

    [JsonInclude]
    public bool IgnorePunctuation
    {
        get => _ignorePunctuation;
        set
        {
            if (_ignorePunctuation == value)
                return;
            _ignorePunctuation = value;
            ViewModel?.ChangeSettings();
            NotifyPropertyChanged();
        }
    }

    [JsonInclude]
    public bool IgnoreCapitalization
    {
        get => _ignoreCapitalization;
        set
        {
            if (_ignoreCapitalization == value)
                return;
            _ignoreCapitalization = value;
            ViewModel?.ChangeSettings();
            NotifyPropertyChanged();
        }
    }

    internal ErrorTolerance CurrentTolerance
    {
        get
        {
            if (this.Equals(HighTolerance))
                return ErrorTolerance.High;
            if (this.Equals(BalancedTolerance))
                return ErrorTolerance.Balanced;
            return this.Equals(LowTolerance) ? ErrorTolerance.Low : ErrorTolerance.Custom;
        }
    }
    
    internal LessonViewModelBase? ViewModel { get; set; }

    internal static LessonOptions HighTolerance { get; } = new()
    {
        CorrectionSteps = 3,
        TolerateSwappedLetters = true,
        IgnoreAccentMarks = true,
        IgnoreHyphens = true,
        IgnorePunctuation = true,
        IgnoreCapitalization = true
    };
    
    internal static LessonOptions BalancedTolerance { get; } = new()
    {
        CorrectionSteps = 2,
        TolerateSwappedLetters = true,
        IgnoreAccentMarks = true,
        IgnoreHyphens = false,
        IgnorePunctuation = true,
        IgnoreCapitalization = false
    };

    internal static LessonOptions LowTolerance { get; } = new()
    {
        CorrectionSteps = 1,
        TolerateSwappedLetters = false,
        IgnoreAccentMarks = false,
        IgnoreHyphens = false,
        IgnorePunctuation = false,
        IgnoreCapitalization = false
    };
    
    internal static LessonOptions MatchTolerance(ErrorTolerance tolerance)
    {
        switch (tolerance)
        {
            case ErrorTolerance.High:
                return HighTolerance;
            case ErrorTolerance.Balanced:
                return BalancedTolerance;
            case ErrorTolerance.Low:
                return LowTolerance;
            case ErrorTolerance.Custom:
            default:
                throw new ArgumentOutOfRangeException(nameof(tolerance), tolerance, null);
        }
    }
    
    public void NotifyPropertyChanged(string propertyName = "") 
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public bool Equals(LessonOptions? other)
    {
        return other is not null && CorrectionSteps == other.CorrectionSteps 
                                 && TolerateSwappedLetters == other.TolerateSwappedLetters 
                                 && IgnoreAccentMarks == other.IgnoreAccentMarks 
                                 && IgnoreHyphens == other.IgnoreHyphens 
                                 && IgnorePunctuation == other.IgnorePunctuation 
                                 && IgnoreCapitalization == other.IgnoreCapitalization;
    }
    public override bool Equals(object? obj) => obj is LessonOptions other && Equals(other);
    public override int GetHashCode() 
        => HashCode.Combine(CorrectionSteps, TolerateSwappedLetters, IgnoreAccentMarks, IgnoreHyphens, IgnorePunctuation, IgnoreCapitalization);

    public static bool operator ==(LessonOptions left, LessonOptions right) => left.Equals(right);
    public static bool operator !=(LessonOptions left, LessonOptions right) => !(left == right);
}