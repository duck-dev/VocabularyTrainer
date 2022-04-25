using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using VocabularyTrainer.Interfaces;

namespace VocabularyTrainer.Models
{
    public class DualVocabularyItem : VocabularyItem, IContentVerification<DualVocabularyItem>
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

        public bool MatchesUnsavedContent(IEnumerable<DualVocabularyItem> collection, out DualVocabularyItem? identicalItem)
        {
            identicalItem = collection.FirstOrDefault(x => x.ChangedAction == NotifyCollectionChangedAction.Remove
                                                            && x.ChangedDefinition.Equals(this.ChangedDefinition)
                                                            && x.ChangedTerm.Equals(this.ChangedTerm));
            return identicalItem is not null && !ReferenceEquals(identicalItem, this);
        }

        public override void EqualizeChangedData()
        {
            base.EqualizeChangedData();
            this.ChangedTerm = this.Term;
        }

        protected internal override void SaveChanges()
        {
            base.SaveChanges();
            this.Term = ChangedTerm;
        }
    }
}