namespace GTiff2Tiles.Avalonia.Models;

/// <summary>
/// Model for ComboBox Avalonia control's item
/// </summary>
public class ComboBoxItemModel
{
    #region Properties

    /// <summary>
    /// Enumerator content
    /// </summary>
    public Enum Content { get; set; }

    /// <summary>
    /// Content's string representation
    /// </summary>
    public string ContentString => Content.ToString();

    #endregion

    #region Constructors

    /// <summary>
    /// Create new ComboBoxItem
    /// </summary>
    /// <param name="content"></param>
    public ComboBoxItemModel(Enum content) => Content = content;

    #endregion

    #region Methods

    /// <summary>
    /// Convert ComboBoxItem's content to real enumerator type
    /// </summary>
    /// <typeparam name="T">Enum</typeparam>
    /// <returns>Enum</returns>
    public T GetRealContent<T>() where T : struct, Enum => (T)Content;

    #endregion
}
