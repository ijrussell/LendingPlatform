namespace LoanApplicationConsoleTests;

public class LoanToValueRateTests
{
    [Theory]
    [InlineData(100_000, 200_000, 50)] // 50.00
    [InlineData(100_000, 300_000, 33)] // 33.33
    [InlineData(123_456, 175_000, 70)] // 70.54
    public void LoanToValueRate_Calculations_ReturnRate(int loanAmount, int assetValue, byte expected)
    {
        var loanToValueRate = LoanToValueRate.Calculate(
            new LoanAmount(loanAmount), 
            new AssetValue(assetValue)
        );
        
        Assert.True(loanToValueRate.HasValue && expected == loanToValueRate.Value.Value);
    }

    [Theory]
    [InlineData(200_000, 200_000)]
    [InlineData(200_000, 199_999)]
    [InlineData(0, 0)]
    public void LoanToValueRate_Calculations_ReturnNull(int loanAmount, int assetValue)
    {
        var loanToValueRate = LoanToValueRate.Calculate(
            new LoanAmount(loanAmount), 
            new AssetValue(assetValue)
        );
        
        Assert.True(!loanToValueRate.HasValue);
    }
}