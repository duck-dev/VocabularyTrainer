using System.Collections;
using System.Collections.Generic;

namespace VocabularyTrainer.Models
{
    public abstract class VocabularyItem
    {
        public VocabularyItem(IList? containerCollection = null)
        {
            this.ContainerCollection = containerCollection;
        }

        private IList? ContainerCollection { get; }

        protected virtual void Remove()
            => ContainerCollection?.Remove(this);

        protected void Remove(ICollection<VocabularyItem> collection)
            => collection.Remove(this);
    }
}