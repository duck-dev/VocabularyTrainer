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
                MaxWidth = double.MaxValue; // Will trigger the `LayoutUpdated` event and needs to be as big as possible
        };
    }
    
    Type IStyleable.StyleKey => typeof(Grid);
}