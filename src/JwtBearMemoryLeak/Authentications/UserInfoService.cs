namespace JwtBearMemoryLeak.Authentications
{
    public class UserInfo : IUserInfo
    {
        public int UserId { get; private set; } = 0;

        public string UserName { get; private set; } = "system";

        public void SetUser(int userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
    }
}
