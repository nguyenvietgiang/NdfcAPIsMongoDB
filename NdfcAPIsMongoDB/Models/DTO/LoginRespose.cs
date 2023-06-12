namespace NdfcAPIsMongoDB.Models.DTO
{
    public class LoginResponse
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }

        public string Email { get; set; }

    }
}
