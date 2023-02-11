namespace SaleLib;

public record User
{
    public int Age { get; set; }
    public string PhoneNum { get; set; }

    public string getUserLevel()
    {
        return Age switch
        {
            < 30 => "L0",
            < 40 => "LMagician",
            < 60 => "LMaster",
            _ => "LSage"
        };
    }
}