using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VocabularyTrainer.Models
{
    public class VocabularyItem
    {
        private string? _definition;
        
        public VocabularyItem(IList? containerCollection = null)
            => this.ContainerCollection = containerCollection;

        [JsonConstructor]
        public VocabularyItem(string definition)
            => this.Definition = definition;

        public string? Definition
        {
            get => _definition; 
            set => this.ChangedDefinition = _definition = value;
        }
        internal string? ChangedDefinition { get; set; }
        
        internal IList? ContainerCollection { get; set; }

        protected virtual void Remove()
            => ContainerCollection?.Remove(this);

        protected void Remove(ICollection<VocabularyItem> collection)
            => collection.Remove(this);
    }
}