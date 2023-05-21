using DialogHostAvalonia;
using ReactiveUI;
using GTiff2Tiles.Avalonia.Enums;

namespace GTiff2Tiles.Avalonia.ViewModels;

/// <inheritdoc/>
public class MessageBoxViewModel : ViewModelBase //, IDisposable
{
    #region Properties

    /// <summary>
    /// Ok button text
    /// </summary>
    public string OkButtonText
    {
        get => _okButtonText;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            this.RaiseAndSetIfChanged(ref _okButtonText, value);
        }
    }

    /// <summary>
    /// Cancel button text
    /// </summary>
    public string CancelButtonText
    {
        get => _cancelButtonText;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            this.RaiseAndSetIfChanged(ref _cancelButtonText, value);
        }
    }

    private bool _isCancelButtonEnabled;

    /// <summary>
    /// Controls if cancel button is enabled
    /// </summary>
    public bool IsCancelButtonEnabled
    {
        get => _isCancelButtonEnabled;
        set => this.RaiseAndSetIfChanged(ref _isCancelButtonEnabled, value);
    }

    private string _message;

    /// <summary>
    /// Message box's message
    /// </summary>
    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }

    /// <summary>
    /// Id of DialogHost to present MessageBox
    /// </summary>
    public string DialogHostId { get; set; }

    /// <summary>
    /// DialogResult on close
    /// </summary>
    public DialogResult DialogResult { get; set; }

    #region Localizable

    private string _okButtonText = "OK";

    private string _cancelButtonText = "Cancel";

    #endregion

    #endregion

    #region Constructors

    /// <inheritdoc/>
    public MessageBoxViewModel() => DialogHostId = MainWindowViewModel.MainDialogHostId;

    /// <inheritdoc/>
    public MessageBoxViewModel(string message, string dialogHostId = null, bool isCancelButtonEnabled = true)
    {
        Message = message;
        DialogHostId = string.IsNullOrWhiteSpace(dialogHostId) ? MainWindowViewModel.MainDialogHostId : dialogHostId;
        IsCancelButtonEnabled = isCancelButtonEnabled;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Ok
    /// </summary>
    public void OkButton()
    {
        DialogResult = DialogResult.Ok;

        CloseDialog();
    }

    /// <summary>
    /// Cancel
    /// </summary>
    public void CancelButton()
    {
        DialogResult = DialogResult.Cancel;

        CloseDialog();
    }

    private void CloseDialog()
    {
        var isOpen = DialogHost.IsDialogOpen(DialogHostId);

        if (isOpen) DialogHost.Close(DialogHostId, DialogResult);
    }

    /// <summary>
    /// Show dialog
    /// </summary>
    /// <returns></returns>
    public async Task<DialogResult> ShowAsync()
    {
        var dialogResult = await DialogHost.Show(this, DialogHostId).ConfigureAwait(true);

        if (dialogResult == null) throw new ArgumentOutOfRangeException(nameof(dialogResult));

        return (DialogResult)dialogResult;
    }

    /// <summary>
    /// Show dialog
    /// </summary>
    /// <param name="message">Message to show</param>
    /// <param name="isCancelButtonEnabled">Do you need cancel button?</param>
    /// <param name="okButtonText">Ok button text</param>
    /// <param name="cancelButtonText">Cancel button text</param>
    /// <param name="dialogHostId">Parent DialogHost to show</param>
    /// <returns>DialogResult</returns>
    public static Task<DialogResult> ShowAsync(string message, bool isCancelButtonEnabled = true,
                                               string okButtonText = null, string cancelButtonText = null, string dialogHostId = null)
    {
        var messageBox = new MessageBoxViewModel(message, dialogHostId, isCancelButtonEnabled)
        {
            OkButtonText = okButtonText,
            CancelButtonText = cancelButtonText
        };

        return messageBox.ShowAsync();
    }

    /// <summary>
    /// Show dialog
    /// </summary>
    /// <param name="exception">Exception</param>
    /// <param name="dialogHostId">Parent DialogHost to show</param>
    public static Task ShowAsync(Exception exception, string dialogHostId = null)
    {
        if (exception == null) throw new ArgumentNullException(nameof(exception));

        return ShowAsync(exception.Message, false, dialogHostId: dialogHostId);
    }

    #endregion
}
