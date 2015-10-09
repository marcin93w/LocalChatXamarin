using System;
using System.Collections.Generic;
using System.Text;

namespace LocalConnect.Models
{
    public class Message
    {
        public Message(string text, DateTime dateTime)
        {
            Text = text;
            DateTime = dateTime;
        }

        public string Text { get; }
        public DateTime DateTime { get; }
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
        public OutcomeMessage(string receiverId, string text, DateTime dateTime)
            : base(text, dateTime)
        {
            ReceiverId = receiverId;
        }

        public string ReceiverId { get; }
    }
}
