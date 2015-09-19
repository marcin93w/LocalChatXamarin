using System;
using System.Collections.Generic;
using System.Text;
using LocalConnect2.Models;

namespace LocalConnect.Models
{
    public class Message
    {
        public Message(string text, DateTime time)
        {
            Text = text;
            Time = time;
        }

        public string Text { get; }
        public DateTime Time { get; }
    }

    public class IncomeMessage : Message
    {
        public IncomeMessage(string sender, string text, DateTime time) 
            : base(text, time)
        {
            SenderId = sender;
        }

        public string SenderId { get; }
    }

    public class OutcomeMessage : Message
    {
        public OutcomeMessage(string receiverId, string text, DateTime time)
            : base(text, time)
        {
            ReceiverId = receiverId;
        }

        public string ReceiverId { get; }
    }
}
