using LocalConnect.Services;

namespace LocalConnect.ViewModel
{
    interface ISocketClientUsingViewModel
    {
        ISocketClient SocketClient { set; }
    }
}