using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

namespace LocalConnect2.ViewModel
{
    public class MessageViewModel : ViewModelBase
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