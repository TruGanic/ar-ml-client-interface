using OrganicTraceAR.Mock;
using OrganicTraceAR.Models;
using OrganicTraceAR.UI.Common;
using TMPro;
using UnityEngine;

namespace OrganicTraceAR.UI.Auth
{
    public class ForgotPasswordFormController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private StatusMessageView statusMessageView;
        [SerializeField] private MockApiService mockApiService;

        public void Submit()
        {
            statusMessageView?.Clear();

            var request = new ForgotPasswordRequest
            {
                Email = emailInput != null ? emailInput.text.Trim() : string.Empty
            };

            StartCoroutine(mockApiService.ForgotPassword(request, response =>
            {
                statusMessageView?.ShowMessage(response.Message);
            }));
        }
    }
}
