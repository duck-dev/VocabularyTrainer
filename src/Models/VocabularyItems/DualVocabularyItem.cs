using System.Collections;

namespace VocabularyTrainer.Models
{
    public class DualVocabularyItem : VocabularyItem
    {
        private string _term = string.Empty;
        private string _changedTerm = string.Empty;
        
        public DualVocabularyItem(IList? containerCollection = null) : base(containerCollection) { }

        public string Term
        {
            get => _term; 
            set => this.ChangedTerm = _term = value.Trim();
        }

        internal string ChangedTerm
        {
            get => _changedTerm;
            set
            {
                _changedTerm = value.Trim();
                InvokeNotifyChanged();
            }
        }

        protected internal override void SaveChanges()
        {
            base.SaveChanges();
            this.Term = ChangedTerm;
        }
    }
}