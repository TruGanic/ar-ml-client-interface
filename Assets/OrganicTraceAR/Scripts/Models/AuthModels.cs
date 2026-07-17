namespace OrganicTraceAR.Models
{
    public class LoginRequest
    {
        public string Email;
        public string Password;
    }

    public class SignupRequest
    {
        public string Name;
        public string Email;
        public string Password;
    }

    public class ForgotPasswordRequest
    {
        public string Email;
    }

    public class AuthResponse
    {
        public bool IsSuccess;
        public string Message;
        public string Token;
        public string DisplayName;
    }
}
