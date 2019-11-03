namespace ApplicationCore
{
    public class AccessToken
    {
        //Access token
        public string Token { get; set; }
        //In Seconds
        public double ExpiresIn { get; set; }

        public AccessToken(string token, double expiresIn)
        {
            Token = token;
            ExpiresIn = expiresIn;
        }
    }
}
