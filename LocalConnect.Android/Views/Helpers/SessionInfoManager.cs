using Android.Content;
using Android.Preferences;
using LocalConnect.Helpers;
using LocalConnect.Models;

namespace LocalConnect.Android.Views.Helpers
{
    public class SessionInfoManager : ISessionInfoManager
    {
        private const string AuthTokenKey = "auth_token";
        private const string PersonIdKey = "person_id";

        private readonly Context _context;

        public SessionInfoManager(Context context)
        {
            _context = context;
        }

        public void SaveSessionInfo(SessionInfo sessionInfo)
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
            var editor = prefs.Edit();
            editor.PutString(AuthTokenKey, sessionInfo.Token);
            editor.PutString(PersonIdKey, sessionInfo.UserId);
            editor.Apply();
        }

        public SessionInfo ReadSessionInfo()
        {
            return new SessionInfo(ReadAuthToken(), ReadPersonId());
        }

        public string ReadAuthToken()
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
            return prefs.GetString(AuthTokenKey, null);
        }

        public string ReadPersonId()
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
            return prefs.GetString(PersonIdKey, null);
        }

        public void DeleteSessionInfo()
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
            var editor = prefs.Edit();
            editor.Remove(AuthTokenKey);
            editor.Remove(PersonIdKey);
            editor.Apply();
        }
    }
}