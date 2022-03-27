using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VocabularyTrainer.Models
{
    public class Lesson
    {
        public Lesson(string name, string description, IEnumerable<Word>? words)
        {
            this.Name = name;
            this.Description = description;
            
            if (words is null)
                return;
            this.Words = new ObservableCollection<Word>(words);
        }
        
        internal string Name { get; private set; }
        internal string Description { get; private set; }
        internal ObservableCollection<Word>? Words { get; }
    }
}