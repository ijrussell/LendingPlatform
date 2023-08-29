namespace LoanApplicationConsoleTests;

public class LoanApplicationRiskDeciderTests
{
    [Theory]
    [InlineData(1_500_001)]
    public void Declined_LoanAmount_TooHigh(int loanAmount)
    {
        var status = RunRiskDecider(loanAmount, 60, 950);
        
        Assert.True(status is LoanApplicationStatus.Declined);
    }
    
    [Theory]
    [InlineData(99_999)]
    public void Declined_LoanAmount_TooLow(int loanAmount)
    {
        var status = RunRiskDecider(loanAmount, 59, 750);
        
        Assert.True(status is LoanApplicationStatus.Declined);
    }
    
    [Theory]
    [InlineData(1_500_000, 60, 950)]
    [InlineData(1_500_000, 60, 951)]
    [InlineData(1_500_000, 59, 950)]
    [InlineData(1_000_000, 60, 950)]
    [InlineData(1_000_000, 60, 951)]
    [InlineData(1_000_000, 59, 950)]
    public void LoanAmount_OneMillionOrGreater_Approved(int loanAmount, byte loanToValueRate, short creditScore)
    {
        var applicationStatus = RunRiskDecider(loanAmount, loanToValueRate, creditScore);
        
        Assert.True(applicationStatus is LoanApplicationStatus.Approved);
    }
    
    [Theory]
    [InlineData(1_500_000, 61, 950)]
    [InlineData(1_500_000, 60, 949)]
    [InlineData(1_500_000, 61, 949)]
    [InlineData(1_000_000, 61, 950)]
    [InlineData(1_000_000, 60, 949)]
    [InlineData(1_000_000, 61, 949)]
    public void LoanAmount_OneMillionOrGreater_Declined(int loanAmount, byte loanToValueRate, short creditScore)
    {
        var status = RunRiskDecider(loanAmount, loanToValueRate, creditScore);
        
        Assert.True(status is LoanApplicationStatus.Declined);
    }

    [Theory]
    [InlineData(999_999, 59, 750)]
    [InlineData(999_999, 60, 800)]
    [InlineData(999_999, 79, 800)]
    [InlineData(999_999, 80, 900)]
    [InlineData(999_999, 89, 900)]
    public void LoanAmount_LessThanOneMillion_Approved(int loanAmount, byte loanToValueRate, short creditScore)
    {
        var status = RunRiskDecider(loanAmount, loanToValueRate, creditScore);
        
        Assert.True(status is LoanApplicationStatus.Approved);
    }

    [Theory]
    [InlineData(999_999, 59, 749)]
    [InlineData(999_999, 79, 799)]
    [InlineData(999_999, 89, 899)]
    [InlineData(999_999, 90, 999)]
    public void LoanAmount_LessThanOneMillion_Declined(int loanAmount, byte loanToValueRate, short creditScore)
    {
        var status = RunRiskDecider(loanAmount, loanToValueRate, creditScore);
        
        Assert.True(status is LoanApplicationStatus.Declined);
    }

    private static LoanApplicationStatus RunRiskDecider(int loanAmount, byte loanToValueRate, short creditScore)
    {
        LoanApplicationRiskDecider riskDecider = new();
        
        return riskDecider.Run(
            new LoanAmount(loanAmount), 
            new LoanToValueRate(loanToValueRate), 
            new CreditScore(creditScore)
        );
    }
}