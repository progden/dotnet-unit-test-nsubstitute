using NUnit.Framework;

namespace SaleLib;

public class SaleService
{
    private readonly PhoneCheckService _phoneCheckService;
    private readonly ContractRepository _contractRepo;

    public SaleService(PhoneCheckService checkService, ContractRepository contractRepository)
    {
        this._phoneCheckService = checkService;
        this._contractRepo = contractRepository;
    }

    public ApplyNewContractResponse ApplyNewContract(User user)
    {
        if (user.Age < 18)
            return new ("fail");
        var rs = _phoneCheckService.Check(user.PhoneNum);
        if (!rs.IsUsed)
        {
            _contractRepo.Save(new Contract(user));
        }

        return rs.IsUsed ? new("fail") : new("success");
    }
}