using System;

namespace LocalConnect.Models
{
    public class Message
    {
        private bool _displayed;

        public Message(string messageId, string text, DateTime dateTime)
        {
            MessageId = messageId;
            Text = text;
            DateTime = dateTime;
        }

        public string MessageId { set; get; }
        public string Text { get; }
        public DateTime DateTime { get; }

        public bool Displayed
        {
            set
            {
                _displayed = value;
                RaiseStatusChanged();
            }
            get { return _displayed; }
        }

        public event EventHandler StatusChanged;

        protected void RaiseStatusChanged()
        {
            StatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class IncomeMessage : Message
    {
        public IncomeMessage(string msgId, string senderId, string text, DateTime dateTime) 
            : base(msgId, text, dateTime)
        {
            SenderId = senderId;
        }

        public string SenderId { get; }
    }

    public class OutcomeMessage : Message
    {
        private bool _sent;
        private bool _deliverError;

        public OutcomeMessage(string msgId, string receiverId, string text, DateTime dateTime)
            : base(msgId, text, dateTime)
        {
            ReceiverId = receiverId;
        }

        public string ReceiverId { get; }

        public bool Sent
        {
            set
            {
                _sent = value;
                RaiseStatusChanged();
            }
            get { return _sent; }
        }

        public bool DeliverError
        {
            set
            {
                _deliverError = value;
                RaiseStatusChanged();
            }
            get { return _deliverError; }
        }
    }
}
