using System.Collections;

namespace VocabularyTrainer.Models
{
    public class DualVocabularyItem : VocabularyItem
    {
        public DualVocabularyItem(IList? containerCollection = null) : base(containerCollection) { }
        
        public string? Term { get; set; }
    }
}