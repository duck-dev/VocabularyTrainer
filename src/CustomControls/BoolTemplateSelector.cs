using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace VocabularyTrainer.CustomControls;

public class BoolTemplateSelector : IDataTemplate
{
    [Content]
    public Dictionary<bool, IDataTemplate> Templates { get; } = new();
    public IControl Build(object data) => Templates[(bool)data].Build(data);
    public bool Match(object data) => data is bool;
}