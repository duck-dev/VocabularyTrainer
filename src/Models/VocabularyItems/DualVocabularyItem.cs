using System.Collections;

namespace VocabularyTrainer.Models
{
    public class DualVocabularyItem : VocabularyItem
    {
        private string? _term;
        
        public DualVocabularyItem(IList? containerCollection = null) : base(containerCollection) { }

        public string? Term
        {
            get => _term; 
            set => this.ChangedTerm = _term = value;
        }
        internal string? ChangedTerm { get; set; }

        protected internal override void SaveChanges()
        {
            base.SaveChanges();
            this.Term = ChangedTerm;
        }
    }
}