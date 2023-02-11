using FluentAssertions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;

namespace SaleLib;

public class SaleServiceTest
{
    private SaleService _saleService;
    private User _user;
    private PhoneCheckService? _checkService;
    private ApplyNewContractResponse _rs;
    private ContractRepository? _repo;

    [SetUp]
    public void SetUp()
    {
        _checkService = Substitute.For<PhoneCheckService>();
        _repo = Substitute.For<ContractRepository>();
        GivenNotUsedNumber("0987654321");
        _saleService = new SaleService(_checkService, _repo);
    }

    [Test]
    public void Given使用者年齡大於18_When申辦手機_Then應可以申辦()
    {
        // arrange
        GivenUserAge(18);
        // act
        var rs = _saleService.ApplyNewContract(_user);
        // assert
        rs.Result.Should().Be("success");

        // arrange
        GivenUserAge(17);
        // act
        rs = _saleService.ApplyNewContract(_user);
        // assert
        rs.Result.Should().Be("fail");
    }

    [Test]
    public void 門號檢查服務 ()
    {
        // arrange
        GivenUserAgeAndPhoneNumber(18, "0987654321");
        GivenNotUsedNumber("0987654321");
        
        // act
        _rs = _saleService.ApplyNewContract(_user);
        
        // assert
        _rs.Result.Should().Be("success");
        CheckServiceShouldBeCalled();
        
        
        // arrange
        GivenUserAgeAndPhoneNumber(18, "0912345678");
        GivenUsedNumber("0912345678");
        
        // act
        _rs = _saleService.ApplyNewContract(_user);
        
        // assert
        _rs.Result.Should().Be("fail");
        CheckServiceShouldBeCalled();
    }

    [Test]
    public void 使用者層級()
    {
        // arrange
        _repo.ClearReceivedCalls();
        GivenUserAge(18);
        // act
        var rs = _saleService.ApplyNewContract(_user);
        // assert
        rs.Result.Should().Be("success");
        _repo.Received()
            .Save(Arg.Is<Contract>(c => c.User == _user && c.UserLevel == "L0"));
        
        // arrange
        _repo.ClearReceivedCalls();
        GivenUserAge(40);
        // act
        rs = _saleService.ApplyNewContract(_user);
        // assert
        rs.Result.Should().Be("success");
        _repo.Received()
            .Save(Arg.Is<Contract>(c => c.User == _user && c.UserLevel == "LMaster"));
        
        // arrange
        _repo.ClearReceivedCalls();
        GivenUserAge(60);
        // act
        rs = _saleService.ApplyNewContract(_user);
        // assert
        rs.Result.Should().Be("success");
        _repo.Received()
            .Save(Arg.Is<Contract>(c => c.User == _user && c.UserLevel == "LSage"));
        
    }

    private void CheckServiceShouldBeCalled()
    {
        _checkService.Received().Check(Arg.Any<string>());
    }

    private void GivenUsedNumber(string phoneNum)
    {
        _checkService.Check(phoneNum).Returns(new PhoneCheckResponse(PhoneNum: phoneNum, IsUsed: true));
    }

    private void GivenNotUsedNumber(string phoneNum)
    {
        _checkService.Check(phoneNum).Returns(new PhoneCheckResponse(PhoneNum: phoneNum, IsUsed: false));
    }

    private void GivenUserAgeAndPhoneNumber(int age, string phoneNum)
    {
        _user = new User {Age = age, PhoneNum = phoneNum};
    }

    private void GivenUserAge(int age)
    {
        _user = new User {Age = age, PhoneNum = "0987654321"};
    }
    
    [Test]
    public void 儲存相關測試 () {
        // arrange
        _repo.ClearReceivedCalls();
        GivenUserAge(18);
        // act
        var rs = _saleService.ApplyNewContract(_user);
        // assert
        rs.Result.Should().Be("success");
        _repo.Received()
            .Save(Arg.Any<Contract>());

        
        // arrange
        _repo.ClearReceivedCalls();
        GivenUserAge(17);
        // act
        rs = _saleService.ApplyNewContract(_user);
        // assert
        rs.Result.Should().Be("fail");
        _repo.DidNotReceive().Save(Arg.Any<Contract>());
    }
}