using LocalConnect.Models;

namespace LocalConnect.Services
{
    public interface ISocketClient
    {
        void Connect(string personId);
        event MessageReceivedEventHandler OnMessageReceived;
        void SendMessage(OutcomeMessage message, int messageIndex);
    }
}
