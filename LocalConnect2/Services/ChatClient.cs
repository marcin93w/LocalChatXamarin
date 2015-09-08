using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quobject.SocketIoClientDotNet.Client;
using WebSocket4Net;


namespace LocalConnect2.Services
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string Message { get; }

        public MessageReceivedEventArgs(string message)
        {
            Message = message;
        }
    }

    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    public class ChatClient
    {
        private readonly string _url = "wss://lc-fancydesign.rhcloud.com:8443";

        private Socket _socket;

        public event MessageReceivedEventHandler MessageReceived;

        public void Initialize()
        {
            _socket = IO.Socket(_url).Connect();
            _socket.On("chat message", message =>
            {
                if (MessageReceived != null)
                {
                    MessageReceived(this, new MessageReceivedEventArgs(message.ToString()));
                }
            });
        }

        public void SendMessage(string message)
        {
            _socket.Emit("chat message", message);
        }
    }
}
