using System;
using System.IO;
using System.Reactive;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;
using Path = System.IO.Path;

namespace VocabularyTrainer.Models
{
    public class LearningModeItem
    {
        private static readonly string _assetsPath =
            Path.Combine(("avares:" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar + "VocabularyTrainer"), "Assets");

        public LearningModeItem(string iconFileName, string name, string description, Action clickAction)
        {
            this.ModeIcon = CreateImage(iconFileName);
            this.Name = name;
            this.Description = description;
            this.ClickCommand = ReactiveCommand.Create(clickAction);
        }
        
        internal Bitmap ModeIcon { get; }
        internal string Name { get; }
        internal string Description { get; }
        internal ReactiveCommand<Unit,Unit> ClickCommand { get; }

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