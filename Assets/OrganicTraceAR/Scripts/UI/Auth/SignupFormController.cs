using OrganicTraceAR.Managers;
using OrganicTraceAR.Mock;
using OrganicTraceAR.Models;
using OrganicTraceAR.UI.Common;
using TMPro;
using UnityEngine;

namespace OrganicTraceAR.UI.Auth
{
    public class SignupFormController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private StatusMessageView statusMessageView;
        [SerializeField] private MockApiService mockApiService;
        [SerializeField] private SceneNavigator sceneNavigator;

        public void Submit()
        {
            statusMessageView?.Clear();

            var request = new SignupRequest
            {
                Name = nameInput != null ? nameInput.text.Trim() : string.Empty,
                Email = emailInput != null ? emailInput.text.Trim() : string.Empty,
                Password = passwordInput != null ? passwordInput.text : string.Empty
            };

            StartCoroutine(mockApiService.Signup(request, response =>
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
