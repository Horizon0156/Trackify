using System.Windows.Input;

namespace Horizon.Framework.Mvvm
{
    /// <summary>
    /// A Message model used to provide a view model for common message boxes.
    /// </summary>
    public class MessageViewModel : ViewModel
    {
        /// <summary>
        /// Creates a new message mode.
        /// </summary>
        /// <param name="title"> The title of the message. </param>
        /// <param name="message"> The message, respectively question. </param>
        /// <param name="messageType"> The type of the message. </param>
        /// <param name="isCancelButtonVisible"> Flag wheather a cancel button should be available. </param>
        public MessageViewModel(string title, string message, MessageType messageType = MessageType.Information, bool isCancelButtonVisible = false)
        {
            Message = message;
            Title = title;
            Type = messageType;

            OkayCommand = CreateCommand(SetResultToOkayAndCloseMessageBox);
            YesCommand = CreateCommand(SetResulYesOkayAndCloseMessageBox);
            NoCommand = CreateCommand(SetResultToNoAndCloseMessageBox);
            CancelCommand = CreateCommand(SetResultToCancelAndCloseMessageBox);

            Result = MessageResult.Cancelled;
        }

        /// <summary>
        /// Gets the Cancel command.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Gets a flag indicating whether a cancel button should be visible.
        /// </summary>
        public bool isCancelButtonVisible { get; }

        /// <summary>
        /// Gets the Cancel command.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the No command.
        /// </summary>
        public ICommand NoCommand { get; }

        /// <summary>
        /// Gets the Okay command.
        /// </summary>
        public ICommand OkayCommand { get; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public MessageResult Result { get; private set; }

        /// <summary>
        /// Gets the Title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        public MessageType Type { get; }

        /// <summary>
        /// Gets the Yes command.
        /// </summary>
        public ICommand YesCommand { get; }

        private void SetResultToCancelAndCloseMessageBox()
        {
            Result = MessageResult.Cancelled;
            OnClosureRequested();
        }

        private void SetResultToNoAndCloseMessageBox()
        {
            Result = MessageResult.No;
            OnClosureRequested();
        }

        private void SetResultToOkayAndCloseMessageBox()
        {
            Result = MessageResult.Ok;
            OnClosureRequested();
        }

        private void SetResulYesOkayAndCloseMessageBox()
        {
            Result = MessageResult.Yes;
            OnClosureRequested();
        }
    }
}