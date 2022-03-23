using System.Collections;

namespace VocabularyTrainer.Models
{
    public class DualVocabularyItem : VocabularyItem
    {
        public DualVocabularyItem(IList? containerCollection = null) : base(containerCollection) { }
        
        internal string? Term { get; set; }
        internal string? Definition { get; set; }
    }
}