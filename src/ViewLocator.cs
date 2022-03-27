using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using VocabularyTrainer.ViewModels;

namespace VocabularyTrainer
{
    public class ViewLocator : IDataTemplate
    {
        public IControl Build(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type == null || Activator.CreateInstance(type) is not Control view)
                return new TextBlock {Text = "Not Found: " + name};
            
            view.DataContext = data;
            return view;
        }

        public bool Match(object data) => data is ViewModelBase;
    }
}