using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;

namespace VocabularyTrainer.UtilityCollection;

public static partial class Utilities
{
    /// <summary>
    /// Create a <see cref="LinearGradientBrush"/> with variable start-, endpoints and gradient stops.
    /// </summary>
    /// <param name="startPoint">The start point of the gradient as a <see cref="RelativePoint"/></param>
    /// <param name="endPoint">The end point of the gradient as a <see cref="RelativePoint"/></param>
    /// <param name="gradientStopInfos">A collection of <see cref="KeyValuePair"/>, each of them containing
    /// the <see cref="Color"/> and Offset of the <see cref="GradientStop"/> it's intended for.</param>
    /// <returns>LinearGradientBrush</returns>
    public static LinearGradientBrush CreateLinearGradientBrush(RelativePoint startPoint, RelativePoint endPoint,
        IEnumerable<KeyValuePair<Color, double>> gradientStopInfos)
    {
        var gradientStops = new GradientStops();
        foreach (var (key, value) in gradientStopInfos)
            gradientStops.Add(new GradientStop(key, value));

        return CreateLinearGradientBrush(startPoint, endPoint, gradientStops);
    }

    /// <summary>
    /// Create a <see cref="LinearGradientBrush"/> with variable start-, endpoints and gradient stops.
    /// </summary>
    /// <param name="startPoint">The start point of the gradient as a <see cref="RelativePoint"/></param>
    /// <param name="endPoint">The end point of the gradient as a <see cref="RelativePoint"/></param>
    /// <param name="colors">The color of each <see cref="GradientStop"/></param>
    /// <param name="offsets">The offset of each <see cref="GradientStop"/></param>
    /// <returns>LinearGradientBrush</returns>
    public static LinearGradientBrush CreateLinearGradientBrush(RelativePoint startPoint, RelativePoint endPoint,
        Color[] colors, double[] offsets)
    {
        var gradientStops = new GradientStops();
        for (int i = 0; i < colors.Length; i++)
        {
            if (i < offsets.Length)
                gradientStops.Add(new GradientStop(colors[i], offsets[i]));
        }

        return CreateLinearGradientBrush(startPoint, endPoint, gradientStops);
    }

    /// <summary>
    /// Create a <see cref="LinearGradientBrush"/> with variable start-, endpoints and gradient stops.
    /// </summary>
    /// <param name="startPoint">The start point of the gradient as a <see cref="RelativePoint"/></param>
    /// <param name="endPoint">The end point of the gradient as a <see cref="RelativePoint"/></param>
    /// <param name="gradientStops">The Gradient Stops used for the gradient.</param>
    /// <returns>LinearGradientBrush</returns>
    public static LinearGradientBrush CreateLinearGradientBrush(RelativePoint startPoint, RelativePoint endPoint,
        GradientStops gradientStops)
    {
        return new LinearGradientBrush
        {
            StartPoint = startPoint,
            EndPoint = endPoint,
            GradientStops = gradientStops
        };
    }
}