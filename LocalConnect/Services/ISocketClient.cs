using LocalConnect.Models;

namespace LocalConnect.Services
{
    public interface ISocketClient
    {
        bool Connect();
        void Disconnect();
        event MessageReceivedEventHandler OnMessageReceived;
        void SendMessage(OutcomeMessage message, int messageIndex);
        void MarkMessageAsDisplayed(IncomeMessage message);
        bool IsConnected { get; set; }
    }
}
