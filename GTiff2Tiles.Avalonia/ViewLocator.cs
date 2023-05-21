using Avalonia.Controls;
using Avalonia.Controls.Templates;
using GTiff2Tiles.Avalonia.ViewModels;
using GTiff2Tiles.Avalonia.Views;

namespace GTiff2Tiles.Avalonia;

/// <inheritdoc/>
public class ViewLocator : IDataTemplate
{
    /// <inheritdoc/>
    public Control Build(object param)
    {
        if (param == null) throw new ArgumentNullException(nameof(param));

        var paramType = param.GetType();
        var name = paramType.FullName!.Replace("ViewModel", "View", StringComparison.InvariantCulture);

        // This way we bind all DataRunnerViewModel's children to DataRunnerView instead of their own views
        var type = paramType.IsSubclassOf(typeof(DataRunnerViewModel))
            ? typeof(DataRunnerView)
            : Type.GetType(name);

        if (type == null) { return new TextBlock { Text = "Not Found: " + name }; }

        return (Control)Activator.CreateInstance(type)!;
    }

    /// <inheritdoc/>
    public bool Match(object data) => data is ViewModelBase;
}
