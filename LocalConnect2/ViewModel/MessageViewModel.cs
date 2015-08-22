using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalConnect2.ViewModel
{
    public class MessageViewModel : MainViewModel
    {
        public string Content { get; }
        public string Author { get; }
        public DateTime Date { get; }

        public MessageViewModel(string content, string author = "", DateTime? date = null)
        {
            Content = content;
            Author = author;
            Date = date ?? DateTime.Now;
        }
    }
}