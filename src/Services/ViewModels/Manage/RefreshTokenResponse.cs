namespace Services.ViewModels.Manage
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public double ExpiresIn { get; set; }
    }
}
