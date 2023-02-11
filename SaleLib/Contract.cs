namespace SaleLib;

public class Contract
{
    public Contract(User user)
    {
        User = user;
        UserLevel = user.getUserLevel();
    }

    public User? User { get; set; }
    public string UserLevel { get; set; }
}