using System.Collections;
using OrganicTraceAR.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OrganicTraceAR.Managers
{
    public class SplashController : MonoBehaviour
    {
        [SerializeField] private float splashDurationSeconds = 2f;
        [SerializeField] private bool goToARIfLoggedIn = true;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(splashDurationSeconds);

            var token = PlayerPrefs.GetString("auth_token", string.Empty);
            var nextScene = goToARIfLoggedIn && !string.IsNullOrWhiteSpace(token)
                ? AppScenes.ARScene
                : AppScenes.AuthScene;

            SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
        }
    }
}
