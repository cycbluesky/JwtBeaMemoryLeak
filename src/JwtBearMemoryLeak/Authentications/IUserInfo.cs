namespace JwtBearMemoryLeak.Authentications
{
    public interface IUserInfo
    {
        int UserId { get; }

        string UserName { get; }

        void SetUser(int userId, string userName);
    }    
}
