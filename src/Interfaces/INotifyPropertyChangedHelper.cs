using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VocabularyTrainer.Interfaces;

public interface INotifyPropertyChangedHelper : INotifyPropertyChanged
{
    void NotifyPropertyChanged([CallerMemberName] string propertyName = "");
}