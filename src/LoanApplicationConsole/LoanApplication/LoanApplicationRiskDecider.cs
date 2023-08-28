namespace LoanApplicationConsole.LoanApplication;

public interface IRiskDecider
{
    LoanApplicationStatus Run(LoanAmount loanAmount, LoanToValueRate loanToValueRate, CreditScore creditScore);
}

public class LoanApplicationRiskDecider : IRiskDecider 
{
    public LoanApplicationStatus Run(LoanAmount loanAmount, LoanToValueRate loanToValueRate, CreditScore creditScore)
    {
        if (RequestedLoanAmountNotAllowed(loanAmount))
            return new LoanApplicationStatus.Declined("Loan amount requested is not allowed.");
            
        if (IsDeclinedForHighRisk(loanAmount, loanToValueRate, creditScore)) 
            return new LoanApplicationStatus.Declined(
                "LTV is too high and/or Credit Score is too low for the Loan amount requested.");

        return new LoanApplicationStatus.Approved();
    }
    
    private static bool RequestedLoanAmountNotAllowed(LoanAmount loanAmount) =>
        loanAmount.Value is < 100_000 or > 1_500_000;

    private static bool IsDeclinedForHighRisk(LoanAmount loanAmount, LoanToValueRate loanToValueRate, CreditScore creditScore) => 
        (loanAmount.Value, loanToValueRate.Value) switch
        {
            (>= 1_000_000, <= 60) => creditScore.Value < 950,
            (< 1_000_000, < 60) => creditScore.Value < 750,
            (< 1_000_000, < 80) => creditScore.Value < 800,
            (< 1_000_000, < 90) => creditScore.Value < 900,
            _ => true
        };
}
