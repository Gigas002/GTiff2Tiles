using System.Windows;
using GTiff2Tiles.GUI.Constants;
using Prism.Mvvm;
using Prism.Commands;
using GTiff2Tiles.GUI.Localization;
using GTiff2Tiles.GUI.Views;
using MaterialDesignThemes.Wpf;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GTiff2Tiles.GUI.ViewModels;

/// <summary>
/// Custom message box dialog for <see cref="MessageBoxDialogView" />
/// </summary>
public sealed class MessageBoxDialogViewModel : BindableBase
{
    #region Properties and fields

    #region UI

    /// <summary>
    /// Copy to clipboard's button hint text
    /// </summary>
    public string CopyToClipboardToolTip { get; } = Strings.CopyToClipboardToolTip;

    /// <summary>
    /// Accept button's text
    /// </summary>
    public string AcceptButtonContent { get; } = Strings.AcceptButtonContent;

    /// <summary>
    /// Cancel button's text
    /// </summary>
    public string CancelButtonContent { get; } = Strings.CancelButtonContent;

    /// <summary>
    /// Dialog's width
    /// </summary>
    public int Width { get; } = Dialogs.Width;

    /// <summary>
    /// Dialog's height
    /// </summary>
    public int Height { get; } = Dialogs.Height;

    #endregion

    #region Backing fields

    private readonly string _message;

    private readonly Visibility _cancelButtonVisibility = Visibility.Collapsed;

    #endregion

    /// <summary>
    /// Text inside MessageBox
    /// </summary>
    public string Message
    {
        get => _message;
        init => SetProperty(ref _message, value);
    }

    /// <summary>
    /// Controls visibility of Cancel button on MessageBox
    /// </summary>
    public Visibility CancelButtonVisibility
    {
        get => _cancelButtonVisibility;
        init => SetProperty(ref _cancelButtonVisibility, value);
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Create a message box without any messages
    /// </summary>
    public MessageBoxDialogViewModel()
    {
        // Bind delegates with methods
        CopyToClipboardButtonCommand = new DelegateCommand(CopyToClipboardButton);
        AcceptButtonCommand = new DelegateCommand(AcceptButton);
        CancelButtonCommand = new DelegateCommand(CancelButton);
    }

    /// <summary>
    /// Create message box
    /// </summary>
    /// <param name="message">Text, that you want to see on message box</param>
    /// <param name="isCancelButtonVisible">Set to <see langword="true"/>,
    /// if you want to see Cancel button</param>
    public MessageBoxDialogViewModel(string message, bool isCancelButtonVisible = false) : this()
    {
        Message = message;
        CancelButtonVisibility = isCancelButtonVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    #endregion

    #region DelegateCommands

    /// <summary>
    /// CancelButton DelegateCommand
    /// </summary>
    public DelegateCommand CancelButtonCommand { get; }

    /// <summary>
    /// AcceptButton DelegateCommand
    /// </summary>
    public DelegateCommand AcceptButtonCommand { get; }

    /// <summary>
    /// CopyToClipboardButton DelegateCommand
    /// </summary>
    public DelegateCommand CopyToClipboardButtonCommand { get; }

    #endregion

    #region Buttons methods

    /// <summary>
    /// Method for Cancel button on <see cref="Views.MessageBoxDialogView"/>
    /// <remarks><para/>Closes the UserControl
    /// and returns <see langword="false"/>
    /// to the message box's caller</remarks>
    /// </summary>
    public static void CancelButton() => DialogHost.CloseDialogCommand.Execute(false, null);

    /// <summary>
    /// Method for Accept button on <see cref="Views.MessageBoxDialogView"/>.
    /// <remarks><para/>Closes the UserControl
    /// and returns <see langword="true"/>
    /// to the message box's caller</remarks>
    /// </summary>
    public static void AcceptButton() => DialogHost.CloseDialogCommand.Execute(true, null);

    /// <summary>
    /// Method for CopyToClipboard button on <see cref="Views.MessageBoxDialogView"/>.
    /// <remarks><para/>Copies the message to clipboard</remarks>
    /// </summary>
    public void CopyToClipboardButton() => Clipboard.SetText(Message);

    #endregion
}