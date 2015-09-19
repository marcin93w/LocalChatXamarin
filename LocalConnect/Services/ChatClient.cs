using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using LocalConnect.Models;
using LocalConnect.Services;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using WebSocket4Net;


namespace LocalConnect2.Services
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public IncomeMessage Message { get; }

        public MessageReceivedEventArgs(IncomeMessage message)
        {
            Message = message;
        }
    }

    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    public class ChatClient
    {
        private readonly string _url = "wss://lc-fancydesign.rhcloud.com:8443";

        private static ChatClient _instance;

        public static ChatClient Instance => _instance ?? (_instance = new ChatClient());

        private ChatClient()
        {
        }

        private Socket _socket;

        public event MessageReceivedEventHandler MessageReceived;

        public void Connect(string userId)
        {
            var socketQuery = new Dictionary<string, string>();
            socketQuery.Add("userId", userId);
            _socket = IO.Socket(_url, new IO.Options {Query = socketQuery}).Connect();
            _socket.On("chat message", message =>
            {
                var msg = message as JObject;
                if (msg != null && MessageReceived != null)
                {
                    var incomeMessage = new IncomeMessage(
                        msg["sender"].ToString(),
                        msg["text"].ToString(), 
                        DateTime.Now);

                    MessageReceived(this, new MessageReceivedEventArgs(incomeMessage));
                }
            });
        }

        public void SendMessage(OutcomeMessage message)
        {
            var msg = new JObject
            {
                { "receiver", message.ReceiverId },
                { "text", message.Text }
            };
            _socket.Emit("chat message", msg);
        }
    }
}
