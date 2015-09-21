using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using LocalConnect.Models;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using WebSocket4Net;


namespace LocalConnect.Services
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

        public event MessageReceivedEventHandler OnMessageReceived;

        public void Connect(string personId)
        {
            var socketQuery = new Dictionary<string, string>();
            socketQuery.Add("personId", personId);
            _socket = IO.Socket(_url, new IO.Options {Query = socketQuery}).Connect();
            _socket.On("chat message", message =>
            {
                var msg = message as JObject;
                if (msg != null && OnMessageReceived != null)
                {
                    var incomeMessage = new IncomeMessage(
                        msg["sender"].ToString(),
                        msg["text"].ToString(), 
                        DateTime.Now);

                    OnMessageReceived(this, new MessageReceivedEventArgs(incomeMessage));
                }
            });
        }

        public void SendMessage(OutcomeMessage message, int messageIndex)
        {
            var msg = new JObject
            {
                { "receiver", message.ReceiverId },
                { "text", message.Text },
                { "clientMessageId", messageIndex }
            };
            _socket.Emit("chat message", msg);
        }
    }
}
