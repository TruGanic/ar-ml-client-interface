using OrganicTraceAR.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OrganicTraceAR.Managers
{
    public class SceneNavigator : MonoBehaviour
    {
        public void GoToAuth() => SceneManager.LoadScene(AppScenes.AuthScene, LoadSceneMode.Single);
        public void GoToAR() => SceneManager.LoadScene(AppScenes.ARScene, LoadSceneMode.Single);
        public void GoToMain() => SceneManager.LoadScene(AppScenes.ARScene, LoadSceneMode.Single);
        public void GoToSplash() => SceneManager.LoadScene(AppScenes.SplashScene, LoadSceneMode.Single);

        public void LoadScene(string sceneName)
        {
            if (!string.IsNullOrWhiteSpace(sceneName))
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            }
        }
    }
}
