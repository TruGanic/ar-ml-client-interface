using System.Collections;
using OrganicTraceAR.Models;
using UnityEngine;

namespace OrganicTraceAR.Mock
{
    public class MockApiService : MonoBehaviour
    {
        [SerializeField] private float simulatedDelaySeconds = 0.8f;

        public IEnumerator Login(LoginRequest request, System.Action<AuthResponse> onComplete)
        {
            yield return new WaitForSeconds(simulatedDelaySeconds);

            var success = !string.IsNullOrWhiteSpace(request.Email) &&
                          !string.IsNullOrWhiteSpace(request.Password) &&
                          request.Password.Length >= 6;

            onComplete?.Invoke(new AuthResponse
            {
                IsSuccess = success,
                Message = success ? "Login successful." : "Enter a valid email and password (6+ chars).",
                Token = success ? "mock_token_organic_trace_ar" : string.Empty,
                DisplayName = success ? "OrganicTraceAR User" : string.Empty
            });
        }

        public IEnumerator Signup(SignupRequest request, System.Action<AuthResponse> onComplete)
        {
            yield return new WaitForSeconds(simulatedDelaySeconds);

            var success = !string.IsNullOrWhiteSpace(request.Name) &&
                          !string.IsNullOrWhiteSpace(request.Email) &&
                          !string.IsNullOrWhiteSpace(request.Password) &&
                          request.Password.Length >= 6;

            onComplete?.Invoke(new AuthResponse
            {
                IsSuccess = success,
                Message = success ? "Signup successful." : "Complete all fields and use a password with at least 6 characters.",
                Token = success ? "mock_token_signup_organic_trace_ar" : string.Empty,
                DisplayName = success ? request.Name : string.Empty
            });
        }

        public IEnumerator ForgotPassword(ForgotPasswordRequest request, System.Action<AuthResponse> onComplete)
        {
            yield return new WaitForSeconds(simulatedDelaySeconds);

            var success = !string.IsNullOrWhiteSpace(request.Email);

            onComplete?.Invoke(new AuthResponse
            {
                IsSuccess = success,
                Message = success
                    ? "If this email exists, a reset link has been sent."
                    : "Please enter your email address.",
                Token = string.Empty,
                DisplayName = string.Empty
            });
        }
    }
}
