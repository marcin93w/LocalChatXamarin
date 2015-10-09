using LocalConnect.Services;

namespace LocalConnect.ViewModel
{
    interface IChatClientUsingViewModel
    {
        IChatClient ChatClient { set; }
    }
}