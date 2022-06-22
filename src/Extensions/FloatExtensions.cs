using System;

namespace VocabularyTrainer.Extensions;

public static partial class Extensions
{
    /// <summary>
    /// Linear interpolation between two float's.
    /// </summary>
    /// <param name="start">Start value (a)</param>
    /// <param name="end">End value (b)</param>
    /// <param name="amount">Amount (t)</param>
    /// <returns>Linearly interpolated value.</returns>
    public static float Lerp(this float start, float end, float amount)
    {
        amount = Math.Clamp(amount, 0, 1);
        return start + (end - start) * amount;
    }
}