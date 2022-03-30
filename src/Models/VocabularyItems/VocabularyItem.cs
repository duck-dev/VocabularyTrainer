using System.Collections;
using System.Collections.Generic;

namespace VocabularyTrainer.Models
{
    public abstract class VocabularyItem
    {
        private readonly IList? _containerCollection;
        
        protected VocabularyItem(IList? containerCollection = null)
        {
            _containerCollection = containerCollection;
        }
        
        public string? Definition { get; set; }

        protected virtual void Remove()
            => _containerCollection?.Remove(this);

        protected void Remove(ICollection<VocabularyItem> collection)
            => collection.Remove(this);
    }
}