using System;
using System.Collections.Generic;
using LocalConnect.Helpers;
using LocalConnect.Models;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;


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

    public class SocketClient : ISocketClient
    {
        private readonly string _url = "wss://lc-fancydesign.rhcloud.com:8443";
        
        private Socket _socket;

        private readonly Dictionary<int, OutcomeMessage> _messagesWitingForConfirmation = new Dictionary<int, OutcomeMessage>();
        private readonly ISessionInfoManager _sessionInfoManager;

        public SocketClient(ISessionInfoManager sessionInfoManager)
        {
            _sessionInfoManager = sessionInfoManager;
        }

        public event MessageReceivedEventHandler OnMessageReceived;

        public bool Connect()
        {
            var personId = _sessionInfoManager.ReadPersonId();
            if (string.IsNullOrEmpty(personId))
                return false;

            var socketQuery = new Dictionary<string, string>();
            socketQuery.Add("personId", personId);
            _socket = IO.Socket(_url, new IO.Options {Query = socketQuery}).Connect();
            _socket.On("chat message", message =>
            {
                var msg = message as JObject;
                if (msg != null && OnMessageReceived != null)
                {
                    var incomeMessage = new IncomeMessage(
                        msg["id"].ToString(),
                        msg["sender"].ToString(),
                        msg["text"].ToString(), 
                        DateTime.Now);

                    OnMessageReceived(this, new MessageReceivedEventArgs(incomeMessage));
                }
            });
            _socket.On("message saved", res =>
            {
                var info = res as JObject;
                int msgIdx = int.Parse(info["clientMessageId"].ToString());
                string msgId = info["messageId"].ToString();
                var msg = _messagesWitingForConfirmation[msgIdx];
                msg.Sent = true;
                msg.MessageId = msgId;
                _messagesWitingForConfirmation.Remove(msgIdx);
            });
            _socket.On("message error", res =>
            {
                var info = res as JObject;
                int msgIdx = int.Parse(info["clientMessageId"].ToString());
                var msg = _messagesWitingForConfirmation[msgIdx];
                msg.DeliverError = true;
                _messagesWitingForConfirmation.Remove(msgIdx);
            });
            _socket.On("disconnect", () => IsConnected = false);
            
            IsConnected = true;
            return true;
        }

        public void Disconnect()
        {
            _socket.Disconnect();
        }

        public void SendMessage(OutcomeMessage message, int messageIndex)
        {
            var msg = new JObject
            {
                { "receiver", message.ReceiverId },
                { "text", message.Text },
                { "clientMessageId", messageIndex }
            };
            _messagesWitingForConfirmation.Add(messageIndex, message);
            _socket.Emit("chat message", msg);
        }

        public void MarkMessageAsDisplayed(IncomeMessage message)
        {
            _socket.Emit("message displayed", message.MessageId);
            message.Displayed = true;
        }

        public bool IsConnected { get; set; }
    }
}
