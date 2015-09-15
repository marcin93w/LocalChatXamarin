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
        public IncomeMessage(Person sender, string text, DateTime time) 
            : base(text, time)
        {
            Sender = sender;
        }

        public Person Sender { get; }
    }
}
