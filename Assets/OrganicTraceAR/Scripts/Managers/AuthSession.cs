using UnityEngine;

namespace OrganicTraceAR.Managers
{
    public static class AuthSession
    {
        private const string TokenKey = "auth_token";
        private const string DisplayNameKey = "display_name";

        public static void Save(string token, string displayName)
        {
            PlayerPrefs.SetString(TokenKey, token);
            PlayerPrefs.SetString(DisplayNameKey, displayName);
            PlayerPrefs.Save();
        }

        public static void Clear()
        {
            PlayerPrefs.DeleteKey(TokenKey);
            PlayerPrefs.DeleteKey(DisplayNameKey);
            PlayerPrefs.Save();
        }

        public static string GetToken() => PlayerPrefs.GetString(TokenKey, string.Empty);
        public static string GetDisplayName() => PlayerPrefs.GetString(DisplayNameKey, "User");
        public static bool IsLoggedIn() => !string.IsNullOrWhiteSpace(GetToken());
    }
}
