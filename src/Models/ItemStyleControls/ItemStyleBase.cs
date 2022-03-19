namespace VocabularyTrainer.Models.ItemStyleControls
{
    public abstract class ItemStyleBase<T> where T : class
    {
        protected ItemStyleBase(T element) => ElementRef = element;

        protected T ElementRef { get; }
    }
}