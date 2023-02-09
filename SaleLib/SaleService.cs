using NUnit.Framework;

namespace SaleLib;

public class SaleService
{
    private readonly PhoneCheckService _phoneCheckService;

    public SaleService(PhoneCheckService checkService)
    {
        this._phoneCheckService = checkService;
    }

    public ApplyNewContractResponse ApplyNewContract(User user)
    {
        if (user.Age < 18)
            return new ("fail");
        var rs = _phoneCheckService.Check(user.PhoneNum);

        return rs.IsUsed ? new("fail") : new("success");
    }
}