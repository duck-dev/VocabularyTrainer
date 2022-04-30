using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using VocabularyTrainer.Models;
using VocabularyTrainer.ViewModels;

namespace VocabularyTrainer.Views.OpenLesson
{
    public class LearningModesView : UserControl
    {
        public LearningModesView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnPointerEnter(object? sender, PointerEventArgs e) 
            => InvokeTriggerDescription(sender);

        private void OnPointerLeave(object? sender, PointerEventArgs e)
            => InvokeTriggerDescription(sender);

        private void InvokeTriggerDescription(object? sender)
        {
            if (this.DataContext is LearningModesViewModel dataContext && sender is StyledElement {DataContext: LearningModeItem hoveredItem})
                hoveredItem.DescriptionEnabled ^= true; // Set bool to opposite state
        }
    }
}