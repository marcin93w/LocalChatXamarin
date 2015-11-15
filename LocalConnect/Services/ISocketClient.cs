using LocalConnect.Models;

namespace LocalConnect.Services
{
    public interface ISocketClient
    {
        bool Connect();
        event MessageReceivedEventHandler OnMessageReceived;
        void SendMessage(OutcomeMessage message, int messageIndex);
        bool IsConnected { get; set; }
    }
}
