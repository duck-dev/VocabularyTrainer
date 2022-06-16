using System;
using VocabularyTrainer.Enums;

namespace VocabularyTrainer.Models;

public struct LessonOptions : IEquatable<LessonOptions>
{
    internal int CorrectionSteps { get; set; }
    internal bool TolerateSwappedLetters { get; set; }
    internal bool IgnoreAccentMarks { get; set; }
    internal bool IgnoreHyphens { get; set; }
    internal bool IgnorePunctuation { get; set; }
    internal bool IgnoreCapitalization { get; set; }

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