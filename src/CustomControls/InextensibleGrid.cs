using System;
using Avalonia.Controls;
using Avalonia.Styling;
using VocabularyTrainer.Views;

namespace VocabularyTrainer.CustomControls;

public class InextensibleGrid : Grid, IStyleable
{
    public InextensibleGrid()
    {
        this.LayoutUpdated += (sender, args) => { MaxWidth = Bounds.Width; };
        
        if (MainWindow.Instance is not { } window)
            return;
        window.PropertyChanged += (sender, args) =>
        {
            if (args.Property == TopLevel.ClientSizeProperty)
                MaxWidth = double.MaxValue;
        };
    }
    
    Type IStyleable.StyleKey => typeof(Grid);
}