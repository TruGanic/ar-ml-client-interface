using OrganicTraceAR.Core;
using UnityEngine;

namespace OrganicTraceAR.Managers
{
    public class BottomNavController : MonoBehaviour
    {
        [SerializeField] private PanelManager panelManager;

        public void ShowHome() => panelManager.Show(AppPanels.Home);
        public void ShowHistory() => panelManager.Show(AppPanels.History);
        public void ShowProfile() => panelManager.Show(AppPanels.Profile);
        public void ShowSettings() => panelManager.Show(AppPanels.Settings);
    }
}
