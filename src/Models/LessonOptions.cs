using System;
using System.Text.Json.Serialization;
using VocabularyTrainer.Enums;

namespace VocabularyTrainer.Models;

public struct LessonOptions : IEquatable<LessonOptions>
{
    [JsonInclude] public int CorrectionSteps { get; set; }
    [JsonInclude] public bool TolerateSwappedLetters { get; set; }
    [JsonInclude] public bool IgnoreAccentMarks { get; set; }
    [JsonInclude] public bool IgnoreHyphens { get; set; }
    [JsonInclude] public bool IgnorePunctuation { get; set; }
    [JsonInclude] public bool IgnoreCapitalization { get; set; }

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

    public bool Equals(LessonOptions other)
    {
        return CorrectionSteps == other.CorrectionSteps 
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