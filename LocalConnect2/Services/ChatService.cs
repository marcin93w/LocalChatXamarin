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
        public MessageReceivedEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    public class ChatService
    {
        private Socket _socket;

        public event MessageReceivedEventHandler MessageReceived;

        public void Initialize()
        {
            _socket = IO.Socket("http://192.168.8.104:1338").Connect();
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
