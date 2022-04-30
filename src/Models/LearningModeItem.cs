using System;
using System.ComponentModel;
using System.IO;
using System.Reactive;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;
using VocabularyTrainer.Interfaces;
using Path = System.IO.Path;

namespace VocabularyTrainer.Models
{
    public class LearningModeItem : INotifyPropertyChangedHelper
    {
        private bool _descriptionEnabled;
        private static readonly string _assetsPath =
            Path.Combine(("avares:" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar + "VocabularyTrainer"), "Assets");
        
        public event PropertyChangedEventHandler? PropertyChanged;

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

        internal bool DescriptionEnabled
        {
            get => _descriptionEnabled;
            set
            {
                if (_descriptionEnabled == value)
                    return;
                _descriptionEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "") 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
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