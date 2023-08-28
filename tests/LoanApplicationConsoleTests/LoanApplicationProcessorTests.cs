using LoanApplicationConsole.LoanApplication;

namespace LoanApplicationConsoleTests;

public class LoanApplicationProcessorTests
{
    private static readonly string Approved = new LoanApplicationStatus.Approved().ToString();
    private static readonly string Declined = new LoanApplicationStatus.Declined("").ToString();

    private readonly LoanApplicationProcessor _processor = new(new LoanApplicationRiskDecider());
    
    [Theory]
    [InlineData(100_000, 100_000)]
    [InlineData(1_500_000, 1_200_000)]
    [InlineData(0, 0)]
    public void LoanToValueRate_Calculations_ReturnNull(int loanAmount, int assetValue)
    {
        var response = _processor.Process(
            new LoanApplicationRequest(loanAmount, assetValue, 999)
        );

        Assert.True(response is LoanApplicationResponse.UnableToProcess);
    }
    
    [Theory]
    [InlineData(100_000, 200_000, 50)]
    [InlineData(100_000, 300_000, 33)]
    [InlineData(123_456, 175_000, 70)]
    public void LoanToValueRate_Calculations_ReturnRate(int loanAmount, int assetValue, short loanToValueRate)
    {
        var response = _processor.Process(
            new LoanApplicationRequest(loanAmount, assetValue, 999)
        );
        
        Assert.True(response is LoanApplicationResponse.Processed p && p.LoanToValueRate == loanToValueRate);
    }
    
    [Theory]
    [InlineData(200_000, 400_000, 900)]
    [InlineData(100_000, 120_000, 975)]
    public void LoanApplication_Approved(int loanAmount, int assetValue, short creditScore)
    {
        var response = _processor.Process(
            new LoanApplicationRequest(loanAmount, assetValue, creditScore)
        );
        
        Assert.True(
            response is LoanApplicationResponse.Processed p 
            && p.Status.Equals(Approved, StringComparison.InvariantCultureIgnoreCase)
        );
    }

    [Theory]
    [InlineData(50_000, 1_000_000, 999)]
    [InlineData(5_000_000, 10_000_000, 999)]
    [InlineData(200_000, 400_000, 700)]
    [InlineData(100_000, 120_000, 750)]
    public void LoanApplication_Declined(int loanAmount, int assetValue, short creditScore)
    {
        LoanApplicationRequest request = new(loanAmount, assetValue, creditScore);

        var response = _processor.Process(request);
        
        Assert.True(
            response is LoanApplicationResponse.Processed p 
            && p.Status.Equals(Declined, StringComparison.InvariantCultureIgnoreCase)
        );
    }

    [Theory]
    [InlineData(0, 1_000_000, 999)]
    [InlineData(500_000, 0, 999)]
    [InlineData(500_000, 1_000_000, 0)]
    [InlineData(500_000, 1_000_000, 1000)]
    public void LoanApplication_Invalid(int loanAmount, int assetValue, short creditScore)
    {
        LoanApplicationRequest request = new(loanAmount, assetValue, creditScore);

        var response = _processor.Process(request);
        
        Assert.True(response is LoanApplicationResponse.UnableToProcess);
    }
}