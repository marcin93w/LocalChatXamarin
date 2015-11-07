using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LocalConnect.Helpers;

namespace LocalConnect.Android.Activities.Helpers
{
    public class AuthTokenManager : IAuthTokenManager
    {
        private const string AuthTokenKey = "auth_token";

        private readonly Context _context;

        public AuthTokenManager(Context context)
        {
            _context = context;
        }

        public void SaveAuthToken(string authToken)
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
            var editor = prefs.Edit();
            editor.PutString(AuthTokenKey, authToken);
            editor.Apply();
        }

        public string ReadAuthToken()
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
            return prefs.GetString(AuthTokenKey, null);
        }

        public void DeleteAuthToken()
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
            var editor = prefs.Edit();
            editor.Remove(AuthTokenKey);
            editor.Apply();
        }
    }
}