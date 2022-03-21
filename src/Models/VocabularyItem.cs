using System.Collections.Generic;

namespace VocabularyTrainer.Models
{
    public class VocabularyItem
    {
        public VocabularyItem(ICollection<VocabularyItem>? containerCollection = null)
        {
            this.ContainerCollection = containerCollection;
        }
        
        internal string? Term { get; set; }
        internal string? Definition { get; set; }
        
        private ICollection<VocabularyItem>? ContainerCollection { get; }

        protected virtual void Remove()
            => ContainerCollection?.Remove(this);

        protected void Remove(ICollection<VocabularyItem> collection)
            => collection.Remove(this);
    }
}