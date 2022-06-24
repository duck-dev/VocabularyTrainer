using System;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Metadata;

namespace VocabularyTrainer.Views.Shapes;

public class StarShape : Shape
{
    private EventHandler? _geometryChangedHandler;
        
    public static readonly StyledProperty<double> PathWidthProperty =
        AvaloniaProperty.Register<StarShape, double>(nameof(PathWidth));
        
    public static readonly StyledProperty<double> PathHeightProperty =
        AvaloniaProperty.Register<StarShape, double>(nameof(PathHeight));
        
    public static readonly StyledProperty<double> CanvasLeftProperty =
        AvaloniaProperty.Register<StarShape, double>(nameof(CanvasLeft));
        
    public static readonly StyledProperty<double> CanvasTopProperty =
        AvaloniaProperty.Register<StarShape, double>(nameof(CanvasTop));

    public static readonly StyledProperty<Geometry?> DataProperty =
        AvaloniaProperty.Register<Path, Geometry?>(nameof(Data));
        
    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<StarShape, object?>(nameof(Content));
        
    static StarShape()
    {
        AffectsGeometry<StarShape>(DataProperty);
        DataProperty.Changed.AddClassHandler<StarShape>((o, e) => o.DataChanged(e));
    }

    public StarShape()
    {
        InitializeComponent();
    }

    public double PathWidth
    {
        get => GetValue(PathWidthProperty);
        set => SetValue(PathWidthProperty, value);
    }
        
    public double PathHeight
    {
        get => GetValue(PathHeightProperty);
        set => SetValue(PathHeightProperty, value);
    }

    public double CanvasLeft
    {
        get => GetValue(CanvasLeftProperty);
        set => SetValue(CanvasLeftProperty, value);
    }
        
    public double CanvasTop
    {
        get => GetValue(CanvasTopProperty);
        set => SetValue(CanvasTopProperty, value);
    }
        
    public Geometry? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }
        
    private EventHandler? GeometryChangedHandler => _geometryChangedHandler ??= GeometryChanged;

    [Content]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }
        
    protected override Geometry? CreateDefiningGeometry() => Data;

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
        
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (Data is not null)
            Data.Changed += GeometryChangedHandler;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (Data is not null)
            Data.Changed -= GeometryChangedHandler;
    }

    private void DataChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (VisualRoot is null)
            return;

        var oldGeometry = (Geometry?)e.OldValue;
        var newGeometry = (Geometry?)e.NewValue;

        if (oldGeometry is not null)
            oldGeometry.Changed -= GeometryChangedHandler;
        if (newGeometry is not null)
            newGeometry.Changed += GeometryChangedHandler;
    }

    private void GeometryChanged(object? sender, EventArgs e) => InvalidateGeometry();
}