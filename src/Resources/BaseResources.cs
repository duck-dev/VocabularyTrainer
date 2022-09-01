using Avalonia;
using Avalonia.Media;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.ResourcesNamespace;

public static partial class Resources
{
    public const int StyleIndex = 2;
    
    // SolidColorBrush
    
    public static readonly SolidColorBrush FullyTransparentBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "FullyTransparent", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#0000FFFF"));
    public static readonly SolidColorBrush AppBlueBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "AppBlue", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#5F8BB0"));
    public static readonly SolidColorBrush DarkAppBlueBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "DarkAppBlue", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#557D9E"));
    public static readonly SolidColorBrush LightAppBlueBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "LightAppBlue", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#83A8C6"));
    public static readonly SolidColorBrush VeryLightBlueBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "VeryLightBlue", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#ABBECE"));
    
    public static readonly SolidColorBrush OppositeAccentBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "OppositeAccent", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#0A0A0A"));
    public static readonly SolidColorBrush SameAccentBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "SameAccent", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#FFFFFF"));
    
    public static readonly SolidColorBrush LightMainBackgroundBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "LightMainBackground", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#D8DDE6"));
    public static readonly SolidColorBrush MainGreyBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "MainGrey", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#808080"));
    public static readonly SolidColorBrush VariantMainGreyBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "VariantMainGrey", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#707070"));
    public static readonly SolidColorBrush LightGreyBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "LightGrey", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#A8ADB5"));
    public static readonly SolidColorBrush VeryLightGreyBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "VeryLightGrey", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#BFC6C9"));
    public static readonly SolidColorBrush DarkenedDialogBackgroundBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "DarkenedDialogBackground", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#66000000"));
    
    public static readonly SolidColorBrush MainGreenBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "MainGreen", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#0CA079"));
    public static readonly SolidColorBrush MainRedBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "MainRed", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#FF0000"));
    public static readonly SolidColorBrush MainYellowBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "MainYellow", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#FCC100"));
    public static readonly SolidColorBrush OrangeBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "Orange", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#DD9A25"));
    
    public static readonly SolidColorBrush SoftRedBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "SoftRed", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#AA4949"));
    public static readonly SolidColorBrush SoftGreenBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "SoftGreen", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#49AA7F"));
    
    public static readonly SolidColorBrush LightGreenBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "LightGreen", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#91C669"));
    public static readonly SolidColorBrush LightBrownBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "LightBrown", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#A8744F"));
    
    public static readonly SolidColorBrush LightRedContextMenuBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "LightRedContextMenu", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#D4735B"));
    public static readonly SolidColorBrush DarkerLightRedContextMenuBrush = 
        Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "DarkerLightRedContextMenu", StyleIndex) 
        ?? new SolidColorBrush(Color.Parse("#C96047"));
    
    // Colors

    public static readonly Color FullyTransparent = FullyTransparentBrush.Color;
    public static readonly Color AppBlue = AppBlueBrush.Color;
    public static readonly Color DarkAppBlue = DarkAppBlueBrush.Color;
    public static readonly Color LightAppBlue = LightAppBlueBrush.Color;
    public static readonly Color VeryLightBlue = VeryLightBlueBrush.Color;
    
    public static readonly Color OppositeAccent = OppositeAccentBrush.Color;
    public static readonly Color SameAccent = SameAccentBrush.Color;
    
    public static readonly Color LightMainBackground = LightMainBackgroundBrush.Color;
    public static readonly Color MainGrey = MainGreyBrush.Color;
    public static readonly Color VariantMainGrey = VariantMainGreyBrush.Color;
    public static readonly Color LightGrey = LightGreyBrush.Color;
    public static readonly Color VeryLightGrey = VeryLightGreyBrush.Color;
    public static readonly Color DarkenedDialogBackground = DarkenedDialogBackgroundBrush.Color;
    
    public static readonly Color MainGreen = MainGreenBrush.Color;
    public static readonly Color MainRed = MainRedBrush.Color;
    public static readonly Color MainYellow = MainYellowBrush.Color;
    public static readonly Color Orange = OrangeBrush.Color;
    
    public static readonly Color SoftRed = SoftRedBrush.Color;
    public static readonly Color SoftGreen = SoftGreenBrush.Color;
    
    public static readonly Color LightGreen = LightGreenBrush.Color;
    public static readonly Color LightBrown = LightBrownBrush.Color;
    
    public static readonly Color LightRedContextMenu = LightRedContextMenuBrush.Color;
    public static readonly Color DarkerLightRedContextMenu = DarkerLightRedContextMenuBrush.Color;
}