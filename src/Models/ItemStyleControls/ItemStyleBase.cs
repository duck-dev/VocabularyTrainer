namespace VocabularyTrainer.Models.ItemStyleControls
{
    public abstract class ItemStyleBase<T> where T : class
    {
        protected ItemStyleBase(T element) => ElementRef = element;

        protected internal T ElementRef { get; }
    }
}