using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly List<OutcomeMessage> _messagesWitingForSentConfirmation = new List<OutcomeMessage>();
        private readonly List<OutcomeMessage> _messagesWitingForDisplayedConfirmation = new List<OutcomeMessage>();
        private readonly ISessionInfoManager _sessionInfoManager;

        public SocketClient(ISessionInfoManager sessionInfoManager)
        {
            _sessionInfoManager = sessionInfoManager;
        }

        public event MessageReceivedEventHandler OnMessageReceived;

        public bool Connect()
        {
            var sessionInfo = _sessionInfoManager.ReadSessionInfo();
            if (string.IsNullOrEmpty(sessionInfo.UserId))
                return false;

            var socketQuery = new Dictionary<string, string>();
            socketQuery.Add("userId", sessionInfo.UserId);
            socketQuery.Add("authToken", sessionInfo.Token);
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
                string msgIdx = info["clientMessageId"].ToString();
                string msgId = info["messageId"].ToString();
                var msg = _messagesWitingForSentConfirmation.FirstOrDefault(m => m.MessageId == msgIdx);
                if (msg != null)
                {
                    msg.Sent = true;
                    msg.MessageId = msgId;
                    _messagesWitingForSentConfirmation.Remove(msg);
                }
            });
            _socket.On("message error", res =>
            {
                var info = res as JObject;
                string msgIdx = info["clientMessageId"].ToString();
                var msg = _messagesWitingForSentConfirmation.FirstOrDefault(m => m.MessageId == msgIdx);
                if (msg != null)
                {
                    msg.DeliverError = true;
                    _messagesWitingForSentConfirmation.Remove(msg);
                }
            });
            _socket.On("message displayed", res =>
            {
                var info = res as JObject;
                string msgId = info["messageId"].ToString();
                var msg = _messagesWitingForDisplayedConfirmation.FirstOrDefault(m => m.MessageId == msgId);
                if (msg != null)
                {
                    msg.Displayed = true;
                    _messagesWitingForDisplayedConfirmation.Remove(msg);
                }
            });
            _socket.On("disconnect", () => IsConnected = false);
            
            IsConnected = true;
            return true;
        }

        public void Disconnect()
        {
            _socket?.Disconnect();
            OnMessageReceived = null;
        }

        public void SendMessage(OutcomeMessage message, int messageIndex)
        {
            message.MessageId = message.ReceiverId + messageIndex;
            var msg = new JObject
            {
                { "receiver", message.ReceiverId },
                { "text", message.Text },
                { "clientMessageId", message.MessageId }
            };
            _socket.Emit("chat message", msg);
            _messagesWitingForSentConfirmation.Add(message);
            _messagesWitingForDisplayedConfirmation.Add(message);
        }

        public void MarkMessageAsDisplayed(IncomeMessage message)
        {
            _socket.Emit("message displayed", message.MessageId);
            message.Displayed = true;
        }

        public bool IsConnected { get; set; }
    }
}
