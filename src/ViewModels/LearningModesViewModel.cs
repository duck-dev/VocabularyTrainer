using System;
using System.IO;
using System.Reactive;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;

namespace VocabularyTrainer.ViewModels
{
    public sealed class LearningModesViewModel : ViewModelBase
    {
        private static readonly string _assetsPath =
            Path.Combine(("avares:" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar + "VocabularyTrainer"), "Assets");

        private Tuple<Bitmap, string, ReactiveCommand<Unit,Unit>>[] LearningModes { get; } =
        {
            new (CreateImage("avalonia-logo.ico"), "Flashcards", 
                ReactiveCommand.Create(() => UtilityCollection.Utilities.Log("Flashcards"))),
            new (CreateImage("avalonia-logo.ico"), "Write", 
                ReactiveCommand.Create(() => UtilityCollection.Utilities.Log("Write"))),
            new (CreateImage("avalonia-logo.ico"), "Multiple Choice", 
                ReactiveCommand.Create(() => UtilityCollection.Utilities.Log("Multiple Choice"))),
            new (CreateImage("avalonia-logo.ico"), "Synonyms and Antonyms", 
                ReactiveCommand.Create(() => UtilityCollection.Utilities.Log("Synonyms and Antonyms"))),
            new (CreateImage("avalonia-logo.ico"), "Vocabulary list", 
                ReactiveCommand.Create(() => UtilityCollection.Utilities.Log("Vocabulary list"))),
        };

        private static Bitmap CreateImage(string fileName)
        {
            string path = Path.Combine(_assetsPath, fileName);
            var uri = new Uri(path);
            
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var asset = assets?.Open(uri);
            
            if (asset is null)
                throw new InvalidDataException("The asset doesn't exist!");
            return new Bitmap(asset);
        }
    }
}