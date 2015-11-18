using LocalConnect.Models;

namespace LocalConnect.Services
{
    public interface ISocketClient
    {
        bool Connect();
        event MessageReceivedEventHandler OnMessageReceived;
        void SendMessage(OutcomeMessage message, int messageIndex);
        void MarkMessageAsDisplayed(IncomeMessage message);
        bool IsConnected { get; set; }
    }
}
