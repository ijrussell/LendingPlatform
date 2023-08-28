using LoanApplicationConsole.LoanApplication;

namespace LoanApplicationConsoleTests;

public class LoanApplicationRequestTests
{
    [Fact]
    public void Valid_LoanApplicationRequest()
    {
        const int loanAmount = 200_000;
        const int assetValue = 400_000;
        const short creditScore = 750;
        
        var request = new LoanApplicationRequest(loanAmount, assetValue, creditScore);
        
        Assert.Equal(loanAmount, request.LoanAmount.Value);
        Assert.Equal(assetValue, request.AssetValue.Value);
        Assert.Equal(creditScore, request.CreditScore.Value);
        Assert.True(request.IsValid);
    }
    
    [Fact]
    public void Invalid_LoanApplicationRequest_LoanAmount_ZeroOrBelow()
    {
        var request = new LoanApplicationRequest(0, 100_000, 500);
        
        Assert.False(request.IsValid);
    }
    
    [Fact]
    public void Invalid_LoanApplicationRequest_AssetValue_ZeroOrBelow()
    {
        var request = new LoanApplicationRequest(100_000, 0, 500);
        
        Assert.False(request.IsValid);
    }

    [Fact]
    public void Invalid_LoanApplicationRequest_CreditScore_TooLow()
    {
        var request = new LoanApplicationRequest(100_000, 200_000, 0);
        
        Assert.False(request.IsValid);
    }

    [Fact]
    public void Invalid_LoanApplicationRequest_CreditScore_TooHigh()
    {
        var request = new LoanApplicationRequest(100_000, 200_000, 1000);
        
        Assert.False(request.IsValid);
    }
}