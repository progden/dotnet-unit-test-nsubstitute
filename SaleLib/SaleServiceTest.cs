using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace SaleLib;

public class SaleServiceTest
{
    private SaleService _saleService;
    private User _user;
    private PhoneCheckService? _checkService;

    [SetUp]
    public void SetUp()
    {
        _checkService = Substitute.For<PhoneCheckService>();
        GivenNotUsedNumber("0987654321");
        _saleService = new SaleService(_checkService);
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
        var rs = _saleService.ApplyNewContract(_user);
        
        // assert
        rs.Result.Should().Be("success");
        _checkService.Received().Check(Arg.Any<string>());
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
}