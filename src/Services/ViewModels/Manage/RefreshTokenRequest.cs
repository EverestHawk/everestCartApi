namespace Services.ViewModels.Manage
{
    public class RefreshTokenRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string SigningKey { get; set; }
        public string RemoteIpAddress { get; set; }
    }
}
