using LocalConnect.Models;

namespace LocalConnect.Helpers
{
    public interface ISessionInfoManager
    {
        void SaveSessionInfo(SessionInfo sessionInfo);
        SessionInfo ReadSessionInfo();
        string ReadAuthToken();
        string ReadPersonId();
        void DeleteSessionInfo();
    }
}
