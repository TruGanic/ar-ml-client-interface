using OrganicTraceAR.Managers;
using OrganicTraceAR.Mock;
using OrganicTraceAR.Models;
using OrganicTraceAR.UI.Common;
using TMPro;
using UnityEngine;

namespace OrganicTraceAR.UI.Auth
{
    public class LoginFormController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private StatusMessageView statusMessageView;
        [SerializeField] private MockApiService mockApiService;
        [SerializeField] private SceneNavigator sceneNavigator;

        public void Submit()
        {
            statusMessageView?.Clear();

            var request = new LoginRequest
            {
                Email = emailInput != null ? emailInput.text.Trim() : string.Empty,
                Password = passwordInput != null ? passwordInput.text : string.Empty
            };

            StartCoroutine(mockApiService.Login(request, response =>
            {
                statusMessageView?.ShowMessage(response.Message);
                if (response.IsSuccess)
                {
                    AuthSession.Save(response.Token, response.DisplayName);
                    sceneNavigator.GoToAR();
                }
            }));
        }
    }
}
