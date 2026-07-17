using OrganicTraceAR.Core;
using OrganicTraceAR.Managers;
using UnityEngine;

namespace OrganicTraceAR.UI.Auth
{
    public class AuthViewController : MonoBehaviour
    {
        [SerializeField] private PanelManager panelManager;

        public void ShowLogin() => panelManager.Show(AppPanels.Login);
        public void ShowSignup() => panelManager.Show(AppPanels.Signup);
        public void ShowForgotPassword() => panelManager.Show(AppPanels.ForgotPassword);
    }
}
