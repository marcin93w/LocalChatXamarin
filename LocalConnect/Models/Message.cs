using System;
using System.Collections.Generic;
using System.Text;

namespace LocalConnect.Models
{
    public class Message
    {
        private bool _displayed;

        public Message(string text, DateTime dateTime)
        {
            Text = text;
            DateTime = dateTime;
        }

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
        public IncomeMessage(string senderId, string text, DateTime dateTime) 
            : base(text, dateTime)
        {
            SenderId = senderId;
        }

        public string SenderId { get; }
    }

    public class OutcomeMessage : Message
    {
        private bool _delivered;
        private bool _deliverError;

        public OutcomeMessage(string receiverId, string text, DateTime dateTime)
            : base(text, dateTime)
        {
            ReceiverId = receiverId;
        }

        public string MessageId { set; get; }
        public string ReceiverId { get; }

        public bool Delivered
        {
            set
            {
                _delivered = value;
                RaiseStatusChanged();
            }
            get { return _delivered; }
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
